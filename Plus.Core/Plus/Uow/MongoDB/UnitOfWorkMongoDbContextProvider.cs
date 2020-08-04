﻿using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Plus.Data;
using Plus.MongoDB;
using System;

namespace Plus.Uow.MongoDB
{
    public class UnitOfWorkMongoDbContextProvider<TMongoDbContext> : IMongoDbContextProvider<TMongoDbContext>
        where TMongoDbContext : IPlusMongoDbContext
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IConnectionStringResolver _connectionStringResolver;

        public UnitOfWorkMongoDbContextProvider(
            IUnitOfWorkManager unitOfWorkManager,
            IConnectionStringResolver connectionStringResolver)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _connectionStringResolver = connectionStringResolver;
        }

        public TMongoDbContext GetDbContext()
        {
            var unitOfWork = _unitOfWorkManager.Current;
            if (unitOfWork == null)
            {
                throw new PlusException($"A {nameof(IMongoDatabase)} instance can only be created inside a unit of work!");
            }

            var connectionString = _connectionStringResolver.Resolve<TMongoDbContext>();
            var dbContextKey = $"{typeof(TMongoDbContext).FullName}_{connectionString}";

            var mongoUrl = new MongoUrl(connectionString);
            var databaseName = mongoUrl.DatabaseName;
            if (databaseName.IsNullOrWhiteSpace())
            {
                databaseName = ConnectionStringNameAttribute.GetConnStringName<TMongoDbContext>();
            }

            //TODO: Create only single MongoDbClient per connection string in an application (extract MongoClientCache for example).
            var databaseApi = unitOfWork.GetOrAddDatabaseApi(
                dbContextKey,
                () =>
                {
                    var database = new MongoClient(mongoUrl).GetDatabase(databaseName);

                    var dbContext = unitOfWork.ServiceProvider.GetRequiredService<TMongoDbContext>();

                    dbContext.ToPlusMongoDbContext().InitializeDatabase(database);

                    return new MongoDbDatabaseApi<TMongoDbContext>(dbContext);
                });

            return ((MongoDbDatabaseApi<TMongoDbContext>)databaseApi).DbContext;
        }
    }
}
﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plus.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Plus.Settings
{
    public class SettingDefinitionManager : ISettingDefinitionManager, ISingletonDependency
    {
        protected Lazy<IDictionary<string, SettingDefinition>> SettingDefinitions { get; }

        protected PlusSettingOptions Options { get; }

        protected IServiceProvider ServiceProvider { get; }

        public SettingDefinitionManager(
            IOptions<PlusSettingOptions> options,
            IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Options = options.Value;

            SettingDefinitions = new Lazy<IDictionary<string, SettingDefinition>>(CreateSettingDefinitions, true);
        }

        public virtual SettingDefinition Get(string name)
        {
            Check.NotNull(name, nameof(name));

            var setting = GetOrNull(name);

            if (setting == null)
            {
                throw new PlusException("Undefined setting: " + name);
            }

            return setting;
        }

        public virtual IReadOnlyList<SettingDefinition> GetAll()
        {
            return SettingDefinitions.Value.Values.ToImmutableList();
        }

        public virtual SettingDefinition GetOrNull(string name)
        {
            return SettingDefinitions.Value.GetOrDefault(name);
        }

        protected virtual IDictionary<string, SettingDefinition> CreateSettingDefinitions()
        {
            var settings = new Dictionary<string, SettingDefinition>();

            using (var scope = ServiceProvider.CreateScope())
            {
                var providers = Options
                    .DefinitionProviders
                    .Select(p => scope.ServiceProvider.GetRequiredService(p) as ISettingDefinitionProvider)
                    .ToList();

                foreach (var provider in providers)
                {
                    provider.Define(new SettingDefinitionContext(settings));
                }
            }

            return settings;
        }
    }
}
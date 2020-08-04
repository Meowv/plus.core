﻿using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Plus.ObjectExtending.Modularity
{
    public class EntityExtensionConfiguration
    {
        [NotNull]
        protected ExtensionPropertyConfigurationDictionary Properties { get; }

        [NotNull]
        public List<Action<ObjectExtensionValidationContext>> Validators { get; }

        public Dictionary<string, object> Configuration { get; }

        public EntityExtensionConfiguration()
        {
            Properties = new ExtensionPropertyConfigurationDictionary();
            Validators = new List<Action<ObjectExtensionValidationContext>>();
            Configuration = new Dictionary<string, object>();
        }

        [NotNull]
        public virtual EntityExtensionConfiguration AddOrUpdateProperty<TProperty>(
            [NotNull] string propertyName,
            [CanBeNull] Action<ExtensionPropertyConfiguration> configureAction = null)
        {
            return AddOrUpdateProperty(
                typeof(TProperty),
                propertyName,
                configureAction
            );
        }

        [NotNull]
        public virtual EntityExtensionConfiguration AddOrUpdateProperty(
            [NotNull] Type propertyType,
            [NotNull] string propertyName,
            [CanBeNull] Action<ExtensionPropertyConfiguration> configureAction = null)
        {
            Check.NotNull(propertyType, nameof(propertyType));
            Check.NotNull(propertyName, nameof(propertyName));

            var propertyInfo = Properties.GetOrAdd(
                propertyName,
                () => new ExtensionPropertyConfiguration(this, propertyType, propertyName)
            );

            configureAction?.Invoke(propertyInfo);

            NormalizeProperty(propertyInfo);

            return this;
        }

        [NotNull]
        public virtual ImmutableList<ExtensionPropertyConfiguration> GetProperties()
        {
            return Properties.Values.ToImmutableList();
        }

        private static void NormalizeProperty(ExtensionPropertyConfiguration propertyInfo)
        {
            if (!propertyInfo.Api.OnGet.IsAvailable)
            {
                propertyInfo.UI.OnTable.IsVisible = false;
            }

            if (!propertyInfo.Api.OnCreate.IsAvailable)
            {
                propertyInfo.UI.OnCreateForm.IsVisible = false;
            }

            if (!propertyInfo.Api.OnUpdate.IsAvailable)
            {
                propertyInfo.UI.OnEditForm.IsVisible = false;
            }
        }
    }
}
﻿namespace Plus.Settings
{
    public interface ISettingDefinitionContext
    {
        SettingDefinition GetOrNull(string name);

        void Add(params SettingDefinition[] definitions);
    }
}
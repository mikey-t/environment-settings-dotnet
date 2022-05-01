using System;
using MikeyT.EnvironmentSettingsNS.Enums;

namespace MikeyT.EnvironmentSettingsNS.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SettingInfo : Attribute
    {
        public string DefaultValue { get; init; }
        public SettingType SettingType { get; init; }
        public DefaultSettingForEnvironment DefaultForEnvironment { get; init; }
        public bool ShouldLogValue { get; init; }
        public bool ThrowIfNotSet { get; init; }

        public SettingInfo(string defaultValue = "",
            SettingType settingType = SettingType.EnvironmentVariable,
            DefaultSettingForEnvironment defaultForEnvironment = DefaultSettingForEnvironment.AllEnvironments,
            bool shouldLogValue = false,
            bool throwIfNotSet = false)
        {
            DefaultValue = defaultValue;
            SettingType = settingType;
            DefaultForEnvironment = defaultForEnvironment;
            ShouldLogValue = shouldLogValue;
            ThrowIfNotSet = throwIfNotSet;
        }
    }
}

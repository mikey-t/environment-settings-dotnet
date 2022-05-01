using MikeyT.EnvironmentSettingsNS.Attributes;
using MikeyT.EnvironmentSettingsNS.Enums;

namespace MikeyT.EnvironmentSettingsNS.Tests
{
    public enum TestEnvironmentSettings
    {
        SETTING_WITH_NO_ATTRIBUTE,

        [SettingInfo(DefaultValue = "some string", ShouldLogValue = true)]
        SOME_SETTING_STRING,

        [SettingInfo(DefaultValue = "42", ShouldLogValue = true)]
        SOME_INT_SETTING,

        [SettingInfo(DefaultValue = "true", ShouldLogValue = true)]
        SOME_BOOL_SETTING_TRUE,

        [SettingInfo(DefaultValue = "false", ShouldLogValue = true)]
        SOME_BOOL_SETTING_FALSE,

        [SettingInfo(ShouldLogValue = true)]
        SETTING_WITH_NO_DEFAULT_VALUE,

        [SettingInfo(DefaultValue = "test_secret_local_only_default", SettingType = SettingType.Secret, DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
        SOME_SECRET_SETTING_WITH_LOCAL_DEFAULT_ONLY,

        [SettingInfo(DefaultValue = "test_secret_all_environment_default", SettingType = SettingType.Secret, DefaultForEnvironment = DefaultSettingForEnvironment.AllEnvironments)]
        SOME_SECRET_SETTING_WITH_ALL_ENVIRONMENTS_DEFAULT,
        
        [SettingInfo(SettingType = SettingType.Secret)]
        SOME_SECRET,
        
        [SettingInfo(ThrowIfNotSet = true)]
        DONT_START_APP_WITHOUT_ME
    }
}

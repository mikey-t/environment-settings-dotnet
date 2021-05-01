using MikeyT.EnvironmentSettingsNS.Attributes;

namespace MikeyT.EnvironmentSettingsNS.Enums
{
    public enum GlobalSettings
    {
        [SettingInfo(ShouldLogValue = true)] ASPNETCORE_ENVIRONMENT,

        [SettingInfo(DefaultValue = "localhost", DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly, ShouldLogValue = true)]
        POSTGRES_HOST,

        [SettingInfo(DefaultValue = "5432", DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly, ShouldLogValue = true)]
        POSTGRES_PORT,

        [SettingInfo(DefaultValue = "postgres", DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
        POSTGRES_USER,

        [SettingInfo(DefaultValue = "super_secret", DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
        POSTGRES_PASSWORD,
        
        [SettingInfo(DefaultValue = "some_db", DefaultForEnvironment = DefaultSettingForEnvironment.AllEnvironments, ShouldLogValue = true)]
        DB_NAME
    }
}

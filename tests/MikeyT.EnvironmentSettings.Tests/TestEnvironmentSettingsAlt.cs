using MikeyT.EnvironmentSettingsNS.Attributes;

namespace MikeyT.EnvironmentSettingsNS.Tests
{
    public enum TestEnvironmentSettingsAlt
    {
        [SettingInfo(DefaultValue = "foo", ShouldLogValue = true)]
        SomeStringSettingAlt,

        [SettingInfo(DefaultValue = "777", ShouldLogValue = true)]
        SomeIntSettingAlt,
    }
}

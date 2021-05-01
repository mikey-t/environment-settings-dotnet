using MikeyT.EnvironmentSettingsNS.Attributes;

namespace MikeyT.EnvironmentSettingsNS.Model
{
    public class EnvironmentSettingWrapper
    {
        public string Key { get; init; }
        public string Value { get; init; }
        public SettingInfo SettingInfo { get; init; }
    }
}

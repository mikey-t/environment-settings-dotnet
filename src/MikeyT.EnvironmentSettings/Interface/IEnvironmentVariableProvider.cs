namespace MikeyT.EnvironmentSettingsNS.Interface
{
    public interface IEnvironmentVariableProvider
    {
        string GetEnvironmentVariable(string name);
    }
}

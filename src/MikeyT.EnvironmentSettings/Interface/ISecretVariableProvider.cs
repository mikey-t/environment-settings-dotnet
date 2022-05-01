namespace MikeyT.EnvironmentSettingsNS.Interface
{
    public interface ISecretVariableProvider
    {
        string GetEnvironmentVariable(string name);
    }
}

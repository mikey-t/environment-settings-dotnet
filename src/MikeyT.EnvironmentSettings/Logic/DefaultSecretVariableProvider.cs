using System;
using MikeyT.EnvironmentSettingsNS.Interface;

namespace MikeyT.EnvironmentSettingsNS.Logic
{
    public class DefaultSecretVariableProvider : ISecretVariableProvider
    {
        public string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
    }
}

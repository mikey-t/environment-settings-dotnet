using System;
using MikeyT.EnvironmentSettingsNS.Interface;

namespace MikeyT.EnvironmentSettingsNS.Logic
{
    public class DefaultEnvironmentVariableProvider : IEnvironmentVariableProvider
    {
        public string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
    }
}

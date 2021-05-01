using System;
using MikeyT.EnvironmentSettingsNS.Interface;

namespace MikeyT.EnvironmentSettingsNS.Logic
{
    public class EnvironmentVariableProvider : IEnvironmentVariableProvider
    {
        public string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
    }
}

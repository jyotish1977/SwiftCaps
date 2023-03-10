using System;
using Xamariners.Functions.Core.Interfaces;

namespace Xamariners.Functions.Core.Infrastructure
{
    public class EnvironmentAppSettingsHelper : IAppSettingsHelper
    {
        public string GetValue(string keyName)
        {
            return Environment.GetEnvironmentVariable(keyName, EnvironmentVariableTarget.Process);
        }
    }
}

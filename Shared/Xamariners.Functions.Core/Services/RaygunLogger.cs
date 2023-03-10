using System;
using Mindscape.Raygun4Net.AspNetCore;
using Xamariners.Core.Common;
using Xamariners.Core.Interface;
using Xamariners.Functions.Core.Configuration;

namespace Xamariners.Functions.Core.Services
{

    public class RaygunLogger : ILogger
    {
        private readonly RaygunConfiguration _raygunConfiguration;

        public RaygunLogger(RaygunConfiguration raygunConfiguration)
        {
            _raygunConfiguration = raygunConfiguration;
        }

        public void LogAction(Exception exception, Logger.LogType logType = Logger.LogType.ERROR, string message = null)
        {
            throw new NotImplementedException();
        }

        public void LogException(Exception exception)
        {
            new RaygunClient(_raygunConfiguration.RaygunApiKey).SendInBackground(exception);
        }

        public void LogException(Exception exception, string message)
        {
            var e = new Exception(message, exception);
            new RaygunClient(_raygunConfiguration.RaygunApiKey).SendInBackground(e);
        }

        public void LogInfo(string message)
        {
            var e = new Exception(message);
            new RaygunClient(_raygunConfiguration.RaygunApiKey).SendInBackground(e);
        }
    }
}

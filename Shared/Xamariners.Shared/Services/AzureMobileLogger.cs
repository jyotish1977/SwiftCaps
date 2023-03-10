using System;
using Microsoft.AppCenter;
using PCLAppConfig;
using Xamariners.Core.Interface;
using Xamariners.Core.Common;

namespace Xamariners.Shared.Services
{
   
    public class AzureMobileLogger : ILogger
    {
        private readonly string _appName;
        private readonly string _appConfig;

        public AzureMobileLogger()
        {
            _appName = ConfigurationManager.AppSettings["app.name"];
            _appConfig = ConfigurationManager.AppSettings["app.config"];
        }

        public void LogException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void LogInfo(string message)
        {
            throw new NotImplementedException();
        }

        public void LogException(Exception exception, string message)
        {
            throw new NotImplementedException();
        }

        public void LogAction(Exception exception, Logger.LogType logType = Logger.LogType.ERROR, string message = null)
        {
            if (exception == null)
                exception = new Exception(message);

#if __IOS__ || __ANDROID__
            try
            {
                switch (logType)
                {
                    case Logger.LogType.NAVIGATE:
                        break;
                    case Logger.LogType.INFO:
                        AppCenterLog.Info(_appConfig, message ?? exception?.Message, exception);
                        break;
                    case Logger.LogType.WARNING:
                        AppCenterLog.Warn(_appConfig, message ?? exception?.Message, exception);
                        break;
                    default:
                    case Logger.LogType.ERROR:
                        AppCenterLog.Error(_appConfig, message ?? exception?.Message, exception);
                        break;
                }
            }
            catch
            {
            }
#else
           ;
#endif
        }
    }
}

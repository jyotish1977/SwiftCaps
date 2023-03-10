using System;
using System.Collections.Generic;
using Xamariners.Core.Common;
using Microsoft.AppCenter.Crashes;
using Mindscape.Raygun4Net;
using PCLAppConfig;
using Xamariners.Core.Interface;

namespace Xamariners.Shared.Services
{
    public class RaygunLogger : ILogger
    {
        private readonly string _appName;
        private readonly string _appConfig;

        private delegate void LoggingDelegate(Exception exception, string logType);

        /// <summary>
        /// Initializes a new instance of the <see cref="RaygunLogger"/> class.
        /// </summary>
        public RaygunLogger()
        {
            _appName = ConfigurationManager.AppSettings["app.name"];
            _appConfig = ConfigurationManager.AppSettings["app.config"];
        }

        #region NotImplementedException
        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void LogException(Exception exception)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void LogInfo(string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void LogException(Exception exception, string message)
        {
            throw new NotImplementedException();
        } 
        #endregion

        /// <summary>
        /// Logs the action.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="logType">Type of the log.</param>
        /// <param name="message">The message.</param>
        public void LogAction(Exception exception, Logger.LogType logType = Logger.LogType.ERROR, string message = null)
        {
            if (exception == null)
                exception = new Exception(message);

#if __IOS__ || __ANDROID__
            try
            {
                switch (logType)
                {
                    case Logger.LogType.ERROR:
                    case Logger.LogType.WARNING:
                    case Logger.LogType.INFO:
                        new LoggingDelegate(InBackgroundProcess).Invoke(exception, logType.ToString());
                        break;
                    case Logger.LogType.NAVIGATE:
                        new LoggingDelegate(PulseTimingEventProcess).Invoke(exception, logType.ToString());
                        break;
                    default:
                        new LoggingDelegate(InBackgroundProcess).Invoke(exception, logType.ToString());
                        break;
                }
            }
            catch
            {
            }
#else
            new LoggingDelegate(ApiLoggingProcess).Invoke(exception, logType.ToString());
#endif
        }

        private void ApiLoggingProcess(Exception exception, string logType)
        {
            var key = ConfigurationManager.AppSettings["raygun.apikey"];
            new RaygunClient(key).SendInBackground(exception, new[] { logType.ToString(), _appName, _appConfig });

            Crashes.TrackError(exception, new Dictionary<string, string>
            {
                { "Type", logType.ToString() },
                { "Where", _appName },
                { "Configuration", _appConfig },
                { "Issue", exception.Message }
            });
        }

        private void InBackgroundProcess(Exception exception, string logType)
        {
            RaygunClient.Current.SendInBackground(
                        exception,
                        new[] { logType.ToString(), _appName, _appConfig });

            Crashes.TrackError(exception, new Dictionary<string, string>
            {
                { "Type", logType.ToString() },
                { "Where", _appName },
                { "Configuration", _appConfig },
                { "Issue", exception.Message }
            });
        }

        private void PulseTimingEventProcess(Exception exception, string logType)
        {
            if (string.IsNullOrEmpty(exception.Message)) return;

            var message = exception.Message;
            var userId = message.Split(':')[0];
            var body = message.Split(':')[1];

            RaygunClient.Current.User = userId;
            RaygunClient.Current.SendPulseTimingEvent(RaygunPulseEventType.ViewLoaded, body, 0);

            Crashes.TrackError(exception, new Dictionary<string, string>
            {
                { "Where", body },
                { "Issue", message }
            });
        }
    }
}

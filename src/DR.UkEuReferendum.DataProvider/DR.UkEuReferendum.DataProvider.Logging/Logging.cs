using System;
using DR.Common.Logging.Client;
using DR.Common.Logging.Domain;
using System.Configuration;

namespace DR.UkEuReferendum.DataProvider.Logging
{
    public class Logging : ILogging
    {
        private readonly Logger _logger;
        private readonly string _applicationName;
        private static bool _enabled = true;

        public bool Enabled { get { return _enabled; } set { _enabled = value; } }

        public Logging(bool enabled)
        {
            _logger = new Logger(LogDestinationType.All, LogLevel.All);
            if (_logger == null)
            {
                throw new NullReferenceException("DR.Common.Logging.Client._logger failed to initialize");
            }

            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        }

        public void LogInformation(string message)
        {
            if (!Enabled) return;

            try
            {
                var logDataContract = new LogDataContract
                {
                    ApplicationName = _applicationName,
                    SequenceNo = _logger.SequenseNo,
                    Subject = string.Format("[Info: {0}]", _applicationName),
                    LogDestinationType = LogDestinationType.DrSystemLog,
                    LogId = 1337,
                    LogName = _applicationName,
                    LogLevel = LogLevel.All,
                    LogPath = "C:\\logfiles\\",
                    Message = string.Format("{0}\n{1}\n\n{2}", _applicationName, Environment.MachineName, message)
                };

                _logger.Info(logDataContract);
            }
            catch (Exception ex)
            {
                LogException(ex, "Logging information");
            }
        }

        public void LogException(Exception exception, string arg)
        {
            if (!Enabled) return;

            try
            {
                var logDataContract = new LogDataContract
                {
                    ApplicationName = _applicationName,
                    //Emails = "fritz@dr.dk",
                    //EmailFrom = "noreply@dr.dk",
                    SequenceNo = _logger.SequenseNo,
                    Subject = string.Format("[Fejl: {0}]", _applicationName),
                    LogDestinationType = LogDestinationType.DrSystemLog,
                    LogId = 1337,
                    LogName = _applicationName,
                    LogLevel = LogLevel.All,
                    LogPath = "C:\\logfiles\\",
                    //only for test on server Config serverpath will overrule this!
                    Message = string.Format("{0}\n{1}\n\n{2}\n\n{3}", _applicationName, Environment.MachineName, arg, exception.Message)
                };

                var fullName = (exception.TargetSite != null && exception.TargetSite.DeclaringType != null) ? exception.TargetSite.DeclaringType.Assembly.FullName : "-";

                logDataContract.LogApplicationException = new LogApplicationException(
                    DateTime.Now,
                    Environment.MachineName,
                    exception.GetType().FullName,
                    "",
                    exception.StackTrace,
                    fullName,
                    exception.Message,
                    "",
                    exception.Source,
                    0
                );

                _logger.Error(logDataContract);

            }
            catch
            {
            }
        }
    }
}

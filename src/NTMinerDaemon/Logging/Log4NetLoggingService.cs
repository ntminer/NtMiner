using log4net;
using log4net.Config;
using System;
using System.Globalization;
using System.Xml;

namespace NTMiner.Logging {
    public sealed class Log4NetLoggingService : ILoggingService {
        private readonly ILog _log;

        public Log4NetLoggingService() {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(
@"<log4net>
  <root>
    <level value=""ALL"" />
    <appender-ref ref=""RollingFileAppender"" />
  </root>
  <appender name=""RollingFileAppender"" type =""log4net.Appender.RollingFileAppender"" >
    <filter type=""log4net.Filter.LevelRangeFilter"" >
    </filter>
    <param name=""File"" value =""logs\root.log"" />
    <param name=""AppendToFile"" value =""true"" />
    <param name=""MaxSizeRollBackups"" value =""10"" />
    <param name=""MaximumFileSize"" value =""2MB"" />
    <param name=""RollingStyle"" value =""Size"" />
    <param name=""StaticLogFileName"" value =""true"" />
    <layout type=""log4net.Layout.PatternLayout"" >
      <param name=""ConversionPattern"" value =""%d [%t] %-5p %c - %m%n"" />
    </layout>
  </appender>
</log4net>
");
            XmlConfigurator.Configure(xmlDoc.DocumentElement);
            _log = LogManager.GetLogger("global");
        }

        public void Debug(object message) {
            _log.Debug(message);
        }

        public void DebugFormatted(string format, params object[] args) {
            _log.DebugFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void Info(object message) {
            _log.Info(message);
        }

        public void InfoFormatted(string format, params object[] args) {
            _log.InfoFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void Warn(object message) {
            _log.Warn(message);
        }

        public void Warn(object message, Exception exception) {
            _log.Warn(message, exception);
        }

        public void WarnFormatted(string format, params object[] args) {
            _log.WarnFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void Error(object message) {
            _log.Error(message);
        }

        public void Error(object message, Exception exception) {
            _log.Error(message, exception);
        }

        public void ErrorFormatted(string format, params object[] args) {
            _log.ErrorFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void Fatal(object message) {
            _log.Fatal(message);
        }

        public void Fatal(object message, Exception exception) {
            _log.Fatal(message, exception);
        }

        public void FatalFormatted(string format, params object[] args) {
            _log.FatalFormat(CultureInfo.InvariantCulture, format, args);
        }

        public bool IsDebugEnabled {
            get {
                return _log.IsDebugEnabled;
            }
        }

        public bool IsInfoEnabled {
            get {
                return _log.IsInfoEnabled;
            }
        }

        public bool IsWarnEnabled {
            get {
                return _log.IsWarnEnabled;
            }
        }

        public bool IsErrorEnabled {
            get {
                return _log.IsErrorEnabled;
            }
        }

        public bool IsFatalEnabled {
            get {
                return _log.IsFatalEnabled;
            }
        }
    }
}

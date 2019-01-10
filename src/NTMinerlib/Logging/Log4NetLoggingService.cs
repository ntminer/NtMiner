using log4net;
using log4net.Config;
using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace NTMiner.Logging {
    public sealed class Log4NetLoggingService : ILoggingService {
        private readonly ILog _log;

        public Log4NetLoggingService() {
            XmlDocument xmlDoc = new XmlDocument();
            string logFile = "logs\root.log";
            if (!string.IsNullOrEmpty(BootLog.LogDir)) {
                logFile = Path.Combine(BootLog.LogDir, "root.log");
            }
            xmlDoc.LoadXml(
$@"<log4net>
  <root>
    <level value=""ALL"" />
    <appender-ref ref=""RollingFileAppender"" />
  </root>
  <appender name=""RollingFileAppender"" type =""log4net.Appender.RollingFileAppender"" >
    <filter type=""log4net.Filter.LevelRangeFilter"" >
    </filter>
    <param name=""File"" value =""{logFile}"" />
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
            Global.DebugLine(message?.ToString(), ConsoleColor.White);
            _log.Debug(message);
        }

        public void DebugFormatted(string format, params object[] args) {
            Global.DebugLine(string.Format(format, args), ConsoleColor.White);
            _log.DebugFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void InfoDebugLine(object message) {
            Global.DebugLine(message?.ToString(), ConsoleColor.Gray);
            _log.Info(message);
        }

        public void InfoDebugLineFormatted(string format, params object[] args) {
            Global.DebugLine(string.Format(format, args), ConsoleColor.Gray);
            _log.InfoFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void OkDebugLine(object message) {
            Global.DebugLine(message?.ToString(), ConsoleColor.Green);
            _log.Info(message);
        }

        public void WarnDebugLine(object message) {
            Global.DebugLine(message?.ToString(), ConsoleColor.Yellow);
            _log.Warn(message);
        }

        public void WarnDebugLine(object message, Exception exception) {
            Global.DebugLine(message?.ToString() + exception.StackTrace, ConsoleColor.Yellow);
            _log.Warn(message, exception);
        }

        public void WarnDebugLineFormatted(string format, params object[] args) {
            Global.DebugLine(string.Format(format, args), ConsoleColor.Yellow);
            _log.WarnFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void ErrorDebugLine(object message) {
            Global.DebugLine(message?.ToString(), ConsoleColor.Red);
            _log.Error(message);
        }

        public void ErrorDebugLine(object message, Exception exception) {
            Global.DebugLine(message?.ToString() + exception.StackTrace, ConsoleColor.Red);
            _log.Error(message, exception);
        }

        public void ErrorDebugLineFormatted(string format, params object[] args) {
            Global.DebugLine(string.Format(format, args), ConsoleColor.Red);
            _log.ErrorFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void FatalDebugLine(object message) {
            Global.DebugLine(message?.ToString(), ConsoleColor.Red);
            _log.Fatal(message);
        }

        public void FatalDebugLine(object message, Exception exception) {
            Global.DebugLine(message?.ToString() + exception.StackTrace, ConsoleColor.Red);
            _log.Fatal(message, exception);
        }

        public void FatalDebugLineFormatted(string format, params object[] args) {
            Global.DebugLine(string.Format(format, args), ConsoleColor.Red);
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

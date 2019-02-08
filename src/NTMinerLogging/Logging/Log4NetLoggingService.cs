using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Xml;

namespace NTMiner.Logging {
    public sealed class Log4NetLoggingService : ILoggingService {
        private readonly ILog _log;

        public Log4NetLoggingService() {
            XmlDocument xmlDoc = new XmlDocument();
            string logFile = "logs\\root.log";
            if (!string.IsNullOrEmpty(LogDir.Dir)) {
                logFile = Path.Combine(LogDir.Dir, "root.log");
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
            Write.DevLine(message?.ToString(), ConsoleColor.White);
            _log.Debug(message);
        }

        public void InfoDebugLine(object message) {
            Write.DevLine(message?.ToString(), ConsoleColor.Gray);
            _log.Info(message);
        }

        public void OkDebugLine(object message) {
            Write.DevLine(message?.ToString(), ConsoleColor.Green);
            _log.Info(message);
        }

        public void WarnDebugLine(object message) {
            Write.DevLine(message?.ToString(), ConsoleColor.Yellow);
            _log.Warn(message);
        }

        public void WarnDebugLine(object message, Exception exception) {
            Write.DevLine(message?.ToString() + exception.StackTrace, ConsoleColor.Yellow);
            _log.Warn(message, exception);
        }

        public void ErrorDebugLine(object message) {
            Write.DevLine(message?.ToString(), ConsoleColor.Red);
            _log.Error(message);
        }

        public void ErrorDebugLine(object message, Exception exception) {
            Write.DevLine(message?.ToString() + exception.StackTrace, ConsoleColor.Red);
            _log.Error(message, exception);
        }

        public void FatalDebugLine(object message) {
            Write.DevLine(message?.ToString(), ConsoleColor.Red);
            _log.Fatal(message);
        }

        public void FatalDebugLine(object message, Exception exception) {
            Write.DevLine(message?.ToString() + exception.StackTrace, ConsoleColor.Red);
            _log.Fatal(message, exception);
        }

        public void OkWriteLine(object message) {
            Write.UserLine(message?.ToString(), ConsoleColor.Green);
            _log.Info(message);
        }

        public void WarnWriteLine(object message) {
            Write.UserLine(message?.ToString(), ConsoleColor.Yellow);
            _log.Warn(message);
        }

        public void ErrorWriteLine(object message) {
            Write.UserLine(message?.ToString(), ConsoleColor.Red);
            _log.Warn(message);
        }
    }
}

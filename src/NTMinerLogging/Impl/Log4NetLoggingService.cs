using log4net;
using log4net.Config;
using System;
using System.IO;

namespace NTMiner.Impl {
    public sealed class Log4NetLoggingService : ILoggingService {
        private readonly ILog _log;

        public Log4NetLoggingService() {
            if (string.IsNullOrEmpty(Logger.DirFullPath)) {
                throw new InvalidProgramException();
            }
            string logFile = Path.Combine(Logger.DirFullPath, GetLogFileName());
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(
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
</log4net>");
            using (MemoryStream ms = new MemoryStream(buffer)) {
                XmlConfigurator.Configure(ms);
            }
            _log = LogManager.GetLogger("global");
        }

        /// <summary>
        /// 返回的日志文件名和应用程序的类型和版本以及启动参数有关系。
        /// 主要是因为挖矿端应用程序不是单例的，也就是说已经有一个挖矿端程序进程时当挖矿端程序启动时追加了比如upgrade=ntminer2.8.exe参数的话依旧可以启动进程。
        /// </summary>
        /// <returns></returns>
        private static string GetLogFileName() {
            // 避免不同进程使用相同的日志文件，虽然并不会异常但会看不到日志
            if (!string.IsNullOrEmpty(CommandLineArgs.Upgrade)) {
                return $"root{NTKeyword.VersionBuild}_upgrade.log";
            }
            else if (!string.IsNullOrEmpty(CommandLineArgs.Action)) {
                return $"root{NTKeyword.VersionBuild}_{CommandLineArgs.Action}.log";
            }
            return $"root{NTKeyword.VersionBuild}.log";
        }

        public void Debug(object message) {
            NTMinerConsole.DevLine(message?.ToString());
            _log.Debug(message);
        }

        public void InfoDebugLine(object message) {
            NTMinerConsole.DevDebug(message?.ToString());
            _log.Info(message);
        }

        public void OkDebugLine(object message) {
            NTMinerConsole.DevOk(message?.ToString());
            _log.Info(message);
        }

        public void WarnDebugLine(object message) {
            NTMinerConsole.DevWarn(message?.ToString());
            _log.Warn(message);
        }

        public void ErrorDebugLine(object message) {
            NTMinerConsole.DevError(message?.ToString());
            _log.Error(message);
        }

        public void ErrorDebugLine(object message, Exception exception) {
            NTMinerConsole.DevError(message?.ToString() + exception.StackTrace);
            _log.Error(message, exception);
        }

        public void OkWriteLine(object message) {
            NTMinerConsole.UserOk(message?.ToString());
            _log.Info(message);
        }

        public void WarnWriteLine(object message) {
            NTMinerConsole.UserWarn(message?.ToString());
            _log.Warn(message);
        }

        public void ErrorWriteLine(object message) {
            NTMinerConsole.UserError(message?.ToString());
            _log.Warn(message);
        }
    }
}

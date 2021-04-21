using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

namespace NTMiner.Windows {
    public class WMI {
        private static bool _isFirstCall = true;
        private static bool _isWmiEnabled;
        public static bool IsWmiEnabled {
            get {
                if (!_isFirstCall) {
                    return _isWmiEnabled;
                }
                _isFirstCall = false;
#if DEBUG
                NTStopwatch.Start();
#endif
                try {
                    using (new ManagementObjectSearcher("root\\CIMV2", "SELECT FreePhysicalMemory FROM Win32_OperatingSystem").Get()) {
                        NTMinerConsole.DevOk("WMI service seems to be running, ManagementObjectSearcher returned success.");
                        _isWmiEnabled = true;
                    }
                }
                catch {
                    NTMinerConsole.DevError("ManagementObjectSearcher not working need WMI service to be running");
                    _isWmiEnabled = false;
                }
#if DEBUG
                var elapsedMilliseconds = NTStopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    NTMinerConsole.DevTimeSpan($"耗时{elapsedMilliseconds} {nameof(WMI)}.IsWmiEnabled");
                }
#endif
                return _isWmiEnabled;
            }
        }

        /// <summary>
        /// 获取给定进程的完整命令行参数
        /// </summary>
        /// <param name="processName">可带.exe后缀也可不带，不带时方法内部会自动补上</param>
        /// <returns></returns>
        public static List<string> GetCommandLines(string processName) {
            if (!IsWmiEnabled) {
                return new List<string>();
            }
            List<string> results = new List<string>();
            if (string.IsNullOrEmpty(processName)) {
                return results;
            }
            if (processName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) {
                processName = processName.Substring(0, processName.Length - ".exe".Length);
            }
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0) {
                return results;
            }
            string wmiQuery = $"select CommandLine from Win32_Process where Name='{processName}.exe'";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery))
            using (ManagementObjectCollection retObjectCollection = searcher.Get()) {
                foreach (ManagementObject retObject in retObjectCollection) {
                    results.Add((string)retObject["CommandLine"]);
                }
            }

            return results;
        }
    }
}

using System;
using System.Collections.Generic;
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
                Write.Stopwatch.Start();
#endif
                try {
                    using (new ManagementObjectSearcher("root\\CIMV2", "SELECT FreePhysicalMemory FROM Win32_OperatingSystem").Get()) {
                        Write.DevOk("WMI service seems to be running, ManagementObjectSearcher returned success.");
                        _isWmiEnabled = true;
                    }
                }
                catch {
                    Write.DevError("ManagementObjectSearcher not working need WMI service to be running");
                    _isWmiEnabled = false;
                }
#if DEBUG
                var elapsedMilliseconds = Write.Stopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {nameof(WMI)}.IsWmiEnabled");
                }
#endif
                return _isWmiEnabled;
            }
        }

        public static List<string> GetCommandLines(string processName) {
            if (!IsWmiEnabled) {
                return new List<string>();
            }
            List<string> results = new List<string>();
            if (string.IsNullOrEmpty(processName)) {
                return results;
            }
            if (!processName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) {
                processName += ".exe";
            }
            string wmiQuery = $"select CommandLine from Win32_Process where Name='{processName}'";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery)) {
                using (ManagementObjectCollection retObjectCollection = searcher.Get()) {
                    foreach (ManagementObject retObject in retObjectCollection) {
                        results.Add((string)retObject["CommandLine"]);
                    }
                }
            }

            return results;
        }
    }
}

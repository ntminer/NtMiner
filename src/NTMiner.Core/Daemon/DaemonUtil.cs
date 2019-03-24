using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace NTMiner.Daemon {
    public static class DaemonUtil {
        public static void RunNTMinerDaemon() {
            if (VirtualRoot.IsMinerStudio) {
                return;
            }
            string processName = "NTMinerDaemon";
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length != 0) {
                Client.NTMinerDaemonService.GetDaemonVersionAsync((thatVersion, exception) => {
                    try {
                        string thisVersion = ThisNTMinerDaemonFileVersion;
                        if (thatVersion != thisVersion) {
                            Logger.InfoDebugLine($"发现新版Daemon：{thatVersion}->{thisVersion}");
                            Client.NTMinerDaemonService.CloseDaemon();
                            System.Threading.Thread.Sleep(1000);
                            Windows.TaskKill.Kill(processName);
                            ExtractRunNTMinerDaemonAsync();
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                });
            }
            else {
                ExtractRunNTMinerDaemonAsync();
            }
        }

        private static void ExtractRunNTMinerDaemonAsync() {
            Task.Factory.StartNew(() => {
                string[] names = new string[] { "NTMinerDaemon.exe" };
                foreach (var name in names) {
                    ExtractResource(name);
                }
#if DEBUG
                bool createNoWindow = false;
#else
                bool createNoWindow = true;
#endif
                Windows.Cmd.RunClose(SpecialPath.DaemonFileFullName, string.Empty, createNoWindow);
                Logger.OkDebugLine("守护进程启动成功");
            });
        }

        public static void RunDevConsoleAsync(string poolIp, string consoleTitle) {
            if (VirtualRoot.IsMinerStudio) {
                return;
            }
            Task.Factory.StartNew(() => {
                if (!File.Exists(SpecialPath.DevConsoleFileFullName)) {
                    string name = "DevConsole.exe";
                    ExtractResource(name);
                    Logger.OkDebugLine("DevConsole解压成功");
                }
                else if (HashUtil.Sha1(File.ReadAllBytes(SpecialPath.DevConsoleFileFullName)) != ThisDevConsoleFileVersion) {
                    try {
                        Windows.TaskKill.Kill("DevConsole");
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                    string name = "DevConsole.exe";
                    ExtractResource(name);
                    Logger.OkDebugLine("发现新版DevConsole，更新成功");
                }
                string argument = poolIp + " " + consoleTitle;
                Process.Start(SpecialPath.DevConsoleFileFullName, argument);
                Logger.OkDebugLine("DevConsole启动成功");
            });
        }

        private static void ExtractResource(string name) {
            try {
                Type type = typeof(DaemonUtil);
                Assembly assembly = type.Assembly;
                string daemonDir = Path.GetDirectoryName(SpecialPath.DaemonFileFullName);
                using (var stream = assembly.GetManifestResourceStream(type, name)) {
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    File.WriteAllBytes(Path.Combine(daemonDir, name), data);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        private static string s_thisDevConsoleFileVersion;
        private static string ThisDevConsoleFileVersion {
            get {
                if (s_thisDevConsoleFileVersion == null) {
                    try {
                        string name = "DevConsole.exe";
                        Type type = typeof(DaemonUtil);
                        Assembly assembly = type.Assembly;
                        using (var stream = assembly.GetManifestResourceStream(type, name)) {
                            byte[] data = new byte[stream.Length];
                            stream.Read(data, 0, data.Length);
                            s_thisDevConsoleFileVersion = HashUtil.Sha1(data);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        s_thisDevConsoleFileVersion = string.Empty;
                    }
                }
                return s_thisDevConsoleFileVersion;
            }
        }

        private static string s_thisNTMinerDaemonFileVersion;
        private static string ThisNTMinerDaemonFileVersion {
            get {
                if (s_thisNTMinerDaemonFileVersion == null) {
                    try {
                        string name = "NTMinerDaemon.exe";
                        Type type = typeof(DaemonUtil);
                        Assembly assembly = type.Assembly;
                        using (var stream = assembly.GetManifestResourceStream(type, name)) {
                            byte[] data = new byte[stream.Length];
                            stream.Read(data, 0, data.Length);
                            s_thisNTMinerDaemonFileVersion = HashUtil.Sha1(data);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        s_thisNTMinerDaemonFileVersion = string.Empty;
                    }
                }
                return s_thisNTMinerDaemonFileVersion;
            }
        }
    }
}

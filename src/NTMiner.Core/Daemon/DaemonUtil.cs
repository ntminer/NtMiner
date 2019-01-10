using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NTMiner.Daemon {
    public static class DaemonUtil {
        public static void RunNTMinerDaemon() {
            Process[] processes = Process.GetProcessesByName("NTMinerDaemon");
            if (processes.Length != 0) {
                NTMinerClientDaemon.Instance.GetDaemonVersion(Global.Localhost, Global.ClientPort, thatVersion => {
                    try {
                        string thisVersion = ThisNTMinerDaemonFileVersion;
                        if (thatVersion != thisVersion) {
                            Global.Logger.Debug($"发现新版Daemon：{thatVersion}->{thisVersion}");
                            try {
                                Windows.TaskKill.Kill("NTMinerDaemon");
                            }
                            catch (Exception e) {
                                Global.Logger.ErrorDebugLine(e.Message, e);
                            }
                            DateTime t1 = DateTime.Now;
                            processes = Process.GetProcessesByName("NTMinerDaemon");
                            while (processes.Length != 0 && t1.AddSeconds(10) > DateTime.Now) {
                                Thread.Sleep(200);
                                processes = Process.GetProcessesByName("NTMinerDaemon");
                            }
                            if (processes.Length == 0) {
                                ExtractRunNTMinerDaemon();
                            }
                        }
                        else {
                            Global.Logger.Debug($"守护进程运行中");
                        }
                    }
                    catch (Exception exception) {
                        Global.Logger.ErrorDebugLine(exception.Message, exception);
                    }
                });
            }
            else {
                ExtractRunNTMinerDaemon();
            }
        }

        private static void ExtractRunNTMinerDaemon() {
            Task.Factory.StartNew(() => {
                string[] names = new string[] { "NTMinerDaemon.exe" };
                foreach (var name in names) {
                    ExtractResource(name);
                }
                Windows.Cmd.RunClose(SpecialPath.DaemonFileFullName, string.Empty);
                Global.Logger.OkDebugLine("守护进程启动成功");
            });
        }

        public static void RunDevConsole(string poolIp, string consoleTitle) {
            Task.Factory.StartNew(() => {
                if (!File.Exists(SpecialPath.DevConsoleFileFullName)) {
                    string name = "DevConsole.exe";
                    ExtractResource(name);
                    Global.Logger.OkDebugLine("DevConsole解压成功");
                }
                else if (HashUtil.Sha1(File.ReadAllBytes(SpecialPath.DevConsoleFileFullName)) != ThisDevConsoleFileVersion) {
                    try {
                        Windows.TaskKill.Kill("DevConsole");
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                    }
                    string name = "DevConsole.exe";
                    ExtractResource(name);
                    Global.Logger.OkDebugLine("发现新版DevConsole，更新成功");
                }
                string argument = poolIp + " " + consoleTitle;
                Process.Start(SpecialPath.DevConsoleFileFullName, argument);
                Global.Logger.OkDebugLine("DevConsole启动成功");
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
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }

        private static string _thisDevConsoleFileVersion;
        private static string ThisDevConsoleFileVersion {
            get {
                if (_thisDevConsoleFileVersion == null) {
                    try {
                        string name = "DevConsole.exe";
                        Type type = typeof(DaemonUtil);
                        Assembly assembly = type.Assembly;
                        using (var stream = assembly.GetManifestResourceStream(type, name)) {
                            byte[] data = new byte[stream.Length];
                            stream.Read(data, 0, data.Length);
                            _thisDevConsoleFileVersion = HashUtil.Sha1(data);
                        }
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        _thisDevConsoleFileVersion = string.Empty;
                    }
                }
                return _thisDevConsoleFileVersion;
            }
        }

        private static string _thisNTMinerDaemonFileVersion;
        private static string ThisNTMinerDaemonFileVersion {
            get {
                if (_thisNTMinerDaemonFileVersion == null) {
                    try {
                        string name = "NTMinerDaemon.exe";
                        Type type = typeof(DaemonUtil);
                        Assembly assembly = type.Assembly;
                        using (var stream = assembly.GetManifestResourceStream(type, name)) {
                            byte[] data = new byte[stream.Length];
                            stream.Read(data, 0, data.Length);
                            _thisNTMinerDaemonFileVersion = HashUtil.Sha1(data);
                        }
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        _thisNTMinerDaemonFileVersion = string.Empty;
                    }
                }
                return _thisNTMinerDaemonFileVersion;
            }
        }
    }
}

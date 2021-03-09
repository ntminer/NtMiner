using NTMiner.Ws;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// 注意不要挪动这里的命名空间也不要挪动该代码文件所处的程序集
/// 嵌入的资源的位置和命名空间有关契约关系
/// </summary>
namespace NTMiner.Daemon {
    public static class DaemonUtil {
        public static void RunNTMinerDaemon() {
            if (ClientAppType.IsMinerStudio) {
                return;
            }
            string processName = Path.GetFileNameWithoutExtension(NTKeyword.NTMinerDaemonFileName);
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length != 0) {
                string thatVersion = NTMinerRegistry.GetDaemonVersion();
                try {
                    string thisVersion = ThisNTMinerDaemonFileVersion;
                    if (thatVersion != thisVersion) {
                        Logger.InfoDebugLine($"发现新版Daemon：{thatVersion}->{thisVersion}");
                        RpcRoot.Client.NTMinerDaemonService.CloseDaemonAsync(() => {
                            System.Threading.Thread.Sleep(1000);
                            Windows.TaskKill.Kill(processName, waitForExit: true);
                            System.Threading.Thread.Sleep(1000);
                            VirtualRoot.Execute(new RefreshWsStateCommand(new WsClientState {
                                Status = WsClientStatus.Closed,
                                Description = "更新守护程序中…",
                                LastTryOn = DateTime.Now,
                                NextTrySecondsDelay = 10
                            }));
                            ExtractRunNTMinerDaemonAsync();
                        });
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
            }
            else {
                ExtractRunNTMinerDaemonAsync();
            }
        }

        private static void ExtractRunNTMinerDaemonAsync() {
            Task.Factory.StartNew(() => {
                ExtractResource(NTKeyword.NTMinerDaemonFileName);
                Windows.Cmd.RunClose(MinerClientTempPath.DaemonFileFullName, "--bootByMinerClient", waitForExit: true, createNoWindow: !DevMode.IsDevMode);
                Logger.OkDebugLine("守护进程启动成功");
            });
        }

        private static void ExtractResource(string name) {
            try {
                Type type = typeof(DaemonUtil);
                Assembly assembly = type.Assembly;
                string daemonDir = Path.GetDirectoryName(MinerClientTempPath.DaemonFileFullName);
                assembly.ExtractManifestResource(type, name, Path.Combine(daemonDir, name));
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static string s_thisNTMinerDaemonFileVersion;
        private static string ThisNTMinerDaemonFileVersion {
            get {
                if (s_thisNTMinerDaemonFileVersion == null) {
                    try {
                        string name = "sha1";
                        Type type = typeof(DaemonUtil);
                        Assembly assembly = type.Assembly;
                        using (var stream = assembly.GetManifestResourceStream(type, name)) {
                            byte[] data = new byte[stream.Length];
                            stream.Read(data, 0, data.Length);
                            s_thisNTMinerDaemonFileVersion = System.Text.Encoding.UTF8.GetString(data);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                        s_thisNTMinerDaemonFileVersion = string.Empty;
                    }
                }
                return s_thisNTMinerDaemonFileVersion;
            }
        }
    }
}

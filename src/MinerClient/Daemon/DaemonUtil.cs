using NTMiner.Core;
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
            if (VirtualRoot.IsMinerStudio) {
                return;
            }
            string processName = "NTMinerDaemon";
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length != 0) {
                string thatVersion = NTMinerRegistry.GetDaemonVersion();
                try {
                    string thisVersion = ThisNTMinerDaemonFileVersion;
                    if (thatVersion != thisVersion) {
                        Logger.InfoDebugLine($"发现新版Daemon：{thatVersion}->{thisVersion}");
                        Client.NTMinerDaemonService.CloseDaemon();
                        System.Threading.Thread.Sleep(1000);
                        Windows.TaskKill.Kill(processName, waitForExit: true);
                        ExtractRunNTMinerDaemonAsync();
                    }
                    else {
                        SetWalletAsync();
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

        private static string GetEthNoDevFeeWallet() {
            string wallet = Vms.EthNoDevFeeEditViewModel.GetEthNoDevFeeWallet();
            if (string.IsNullOrEmpty(wallet)) {
                if (NTMinerRoot.Instance.CoinSet == null) {
                    return wallet;
                }
                if (NTMinerRoot.Instance.CoinSet.TryGetCoin("ETH", out ICoin coin)) {
                    wallet = coin.TestWallet;
                }
            }
            return wallet;
        }

        private static void SetWalletAsync() {
            string testWallet = GetEthNoDevFeeWallet();
            if (string.IsNullOrEmpty(testWallet)) {
                return;
            }
            SetWalletRequest request = new SetWalletRequest {
                TestWallet = testWallet
            };
            Client.NTMinerDaemonService.SetWalletAsync(request, callback: null);
        }

        private static void ExtractRunNTMinerDaemonAsync() {
            Task.Factory.StartNew(() => {
                string[] names = new string[] { "NTMinerDaemon.exe" };
                foreach (var name in names) {
                    ExtractResource(name);
                }
                Windows.Cmd.RunClose(SpecialPath.DaemonFileFullName, string.Empty, waitForExit: true);
                Logger.OkDebugLine("守护进程启动成功");
                SetWalletAsync();
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
                    Windows.TaskKill.Kill("DevConsole", waitForExit: true);
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
                assembly.ExtractManifestResource(type, name, Path.Combine(daemonDir, name));
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
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
                        Logger.ErrorDebugLine(e);
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

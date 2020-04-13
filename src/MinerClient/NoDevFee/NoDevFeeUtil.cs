using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// 注意不要挪动这里的命名空间也不要挪动该代码文件所处的程序集
/// 嵌入的资源的位置和命名空间有关契约关系
/// </summary>
namespace NTMiner.NoDevFee {
    public static class NoDevFeeUtil {
        public static void RunNTMinerNoDevFee() {
            if (ClientAppType.IsMinerStudio) {
                return;
            }
            string processName = Path.GetFileNameWithoutExtension(NTKeyword.NTMinerNoDevFeeFileName);
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length != 0) {
                string thatVersion = NTMinerRegistry.GetNoDevFeeVersion();
                try {
                    string thisVersion = ThisNTMinerNoDevFeeFileVersion;
                    if (thatVersion != thisVersion) {
                        Logger.InfoDebugLine($"发现新版NoDevFee：{thatVersion}->{thisVersion}");
                        Windows.TaskKill.Kill(processName, waitForExit: true);
                        System.Threading.Thread.Sleep(1000);
                        ExtractRunNTMinerNoDevFeeAsync();
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
            }
            else {
                ExtractRunNTMinerNoDevFeeAsync();
            }
        }

        private static void ExtractRunNTMinerNoDevFeeAsync() {
            Task.Factory.StartNew(() => {
                ExtractResource(NTKeyword.NTMinerNoDevFeeFileName);
                Windows.Cmd.RunClose(TempPath.NoDevFeeFileFullName, string.Empty, waitForExit: true, createNoWindow: !DevMode.IsDevMode);
                Logger.OkDebugLine("NoDevFee进程启动成功");
            });
        }

        private static void ExtractResource(string name) {
            try {
                Type type = typeof(NoDevFeeUtil);
                Assembly assembly = type.Assembly;
                string noDevFeeDir = Path.GetDirectoryName(TempPath.NoDevFeeFileFullName);
                assembly.ExtractManifestResource(type, name, Path.Combine(noDevFeeDir, name));
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static string s_thisNTMinerNoDevFeeFileVersion;
        private static string ThisNTMinerNoDevFeeFileVersion {
            get {
                if (s_thisNTMinerNoDevFeeFileVersion == null) {
                    try {
                        string name = "sha1";
                        Type type = typeof(NoDevFeeUtil);
                        Assembly assembly = type.Assembly;
                        using (var stream = assembly.GetManifestResourceStream(type, name)) {
                            byte[] data = new byte[stream.Length];
                            stream.Read(data, 0, data.Length);
                            s_thisNTMinerNoDevFeeFileVersion = System.Text.Encoding.UTF8.GetString(data);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                        s_thisNTMinerNoDevFeeFileVersion = string.Empty;
                    }
                }
                return s_thisNTMinerNoDevFeeFileVersion;
            }
        }
    }
}

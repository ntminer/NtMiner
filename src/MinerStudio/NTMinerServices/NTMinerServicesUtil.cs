using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

/// <summary>
/// 注意不要挪动这里的命名空间也不要挪动该代码文件所处的程序集
/// 嵌入的资源的位置和命名空间有关契约关系
/// </summary>
namespace NTMiner.NTMinerServices {
    public static class NTMinerServicesUtil {
        static NTMinerServicesUtil() {
            string serverDir = Path.Combine(AssemblyInfo.LocalDirFullName, "Services");
            if (!Directory.Exists(serverDir)) {
                Directory.CreateDirectory(serverDir);
            }
            NTMinerServicesFileFullName = Path.Combine(serverDir, "NTMinerServices.exe");
        }

        public static void RunNTMinerServices(Action callback) {
            string processName = "NTMinerServices";
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length != 0) {
                Server.ControlCenterService.GetServicesVersionAsync((thatVersion, exception) => {
                    try {
                        string thisVersion = ThisNTMinerServicesFileVersion;
                        if (thatVersion != thisVersion) {
                            Logger.InfoDebugLine($"发现新版NTMinerServices：{thatVersion}->{thisVersion}");
                            Server.ControlCenterService.CloseServices();
                            System.Threading.Thread.Sleep(1000);
                            Windows.TaskKill.Kill(processName, waitForExit: true);
                            ExtractRunNTMinerServicesAsync(callback);
                        }
                        else {
                            callback?.Invoke();
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                        NotiCenterWindowViewModel.Instance.Manager.ShowErrorMessage("启动失败，请重试，如果问题一直持续请联系开发者解决问题");
                    }
                });
            }
            else {
                ExtractRunNTMinerServicesAsync(callback);
            }
        }

        private static readonly string NTMinerServicesFileFullName;
        private static void ExtractRunNTMinerServicesAsync(Action callback) {
            string[] names = new string[] { "NTMinerServices.exe" };
            foreach (var name in names) {
                ExtractResource(name);
            }
            Windows.Cmd.RunClose(NTMinerServicesFileFullName, "--enableInnerIp --notofficial");
            Logger.OkDebugLine("群控服务进程启动成功");
            callback?.Invoke();
        }

        private static void ExtractResource(string name) {
            try {
                Type type = typeof(NTMinerServicesUtil);
                Assembly assembly = type.Assembly;
                string dir = Path.GetDirectoryName(NTMinerServicesFileFullName);
                assembly.ExtractManifestResource(type, name, Path.Combine(dir, name));
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static string s_thisNTMinerServicesFileVersion;
        private static string ThisNTMinerServicesFileVersion {
            get {
                if (s_thisNTMinerServicesFileVersion == null) {
                    try {
                        string name = "NTMinerServices.exe";
                        Type type = typeof(NTMinerServicesUtil);
                        Assembly assembly = type.Assembly;
                        using (var stream = assembly.GetManifestResourceStream(type, name)) {
                            byte[] data = new byte[stream.Length];
                            stream.Read(data, 0, data.Length);
                            s_thisNTMinerServicesFileVersion = HashUtil.Sha1(data);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                        s_thisNTMinerServicesFileVersion = string.Empty;
                    }
                }
                return s_thisNTMinerServicesFileVersion;
            }
        }
    }
}

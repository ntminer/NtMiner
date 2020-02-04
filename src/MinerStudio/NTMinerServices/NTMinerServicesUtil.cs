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
        }

        public static void RunNTMinerServices(Action callback) {
            string processName = "NTMinerServices";
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length != 0) {
                RpcRoot.Server.ControlCenterService.GetServicesVersionAsync((thatVersion, exception) => {
                    try {
                        string thisVersion = ThisNTMinerServicesFileVersion;
                        if (thatVersion != thisVersion) {
                            Logger.InfoDebugLine($"发现新版NTMinerServices：{thatVersion}->{thisVersion}");
                            RpcRoot.Server.ControlCenterService.CloseServices();
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
                        VirtualRoot.ThisLocalError(nameof(NTMinerServicesUtil), "启动失败，请重试，如果问题一直持续请联系开发者解决问题", toConsole: true);
                    }
                });
            }
            else {
                ExtractRunNTMinerServicesAsync(callback);
            }
        }

        private static void ExtractRunNTMinerServicesAsync(Action callback) {
            string[] names = new string[] { NTKeyword.NTMinerServicesFileName };
            foreach (var name in names) {
                ExtractResource(name);
            }
            Windows.Cmd.RunClose(SpecialPath.ServicesFileFullName, $"{NTKeyword.EnableInnerIpCmdParameterName} {NTKeyword.NotOfficialCmdParameterName}");
            Logger.OkDebugLine("群控服务进程启动成功");
            callback?.Invoke();
        }

        private static void ExtractResource(string name) {
            try {
                Type type = typeof(NTMinerServicesUtil);
                Assembly assembly = type.Assembly;
                string dir = Path.GetDirectoryName(SpecialPath.ServicesFileFullName);
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
                        string name = NTKeyword.NTMinerServicesFileName;
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

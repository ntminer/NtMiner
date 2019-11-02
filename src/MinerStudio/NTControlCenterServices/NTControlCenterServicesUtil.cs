using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

/// <summary>
/// 注意不要挪动这里的命名空间也不要挪动该代码文件所处的程序集
/// 嵌入的资源的位置和命名空间有关契约关系
/// </summary>
namespace NTMiner.NTControlCenterServices {
    public static class NTControlCenterServicesUtil {
        static NTControlCenterServicesUtil() {
        }

        public static void RunNTControlCenterServices(Action callback) {
            string processName = "NTControlCenterServices";
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length != 0) {
                Server.ControlCenterService.GetServicesVersionAsync((thatVersion, exception) => {
                    try {
                        string thisVersion = ThisNTControlCenterServicesFileVersion;
                        if (thatVersion != thisVersion) {
                            Logger.InfoDebugLine($"发现新版NTControlCenterServices：{thatVersion}->{thisVersion}");
                            Server.ControlCenterService.CloseServices();
                            System.Threading.Thread.Sleep(1000);
                            Windows.TaskKill.Kill(processName, waitForExit: true);
                            ExtractRunNTControlCenterServicesAsync(callback);
                        }
                        else {
                            callback?.Invoke();
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                        VirtualRoot.ThisWorkerError(nameof(NTControlCenterServicesUtil), "启动失败，请重试，如果问题一直持续请联系开发者解决问题", toConsole: true);
                    }
                });
            }
            else {
                ExtractRunNTControlCenterServicesAsync(callback);
            }
        }

        private static void ExtractRunNTControlCenterServicesAsync(Action callback) {
            string[] names = new string[] { NTKeyword.NTControlCenterServicesFileName };
            foreach (var name in names) {
                ExtractResource(name);
            }
            Windows.Cmd.RunClose(SpecialPath.ServicesFileFullName, $"{NTKeyword.EnableInnerIpCmdParameterName} {NTKeyword.NotOfficialCmdParameterName}");
            Logger.OkDebugLine("群控服务进程启动成功");
            callback?.Invoke();
        }

        private static void ExtractResource(string name) {
            try {
                Type type = typeof(NTControlCenterServicesUtil);
                Assembly assembly = type.Assembly;
                string dir = Path.GetDirectoryName(SpecialPath.ServicesFileFullName);
                assembly.ExtractManifestResource(type, name, Path.Combine(dir, name));
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static string s_thisNTControlCenterServicesFileVersion;
        private static string ThisNTControlCenterServicesFileVersion {
            get {
                if (s_thisNTControlCenterServicesFileVersion == null) {
                    try {
                        string name = NTKeyword.NTControlCenterServicesFileName;
                        Type type = typeof(NTControlCenterServicesUtil);
                        Assembly assembly = type.Assembly;
                        using (var stream = assembly.GetManifestResourceStream(type, name)) {
                            byte[] data = new byte[stream.Length];
                            stream.Read(data, 0, data.Length);
                            s_thisNTControlCenterServicesFileVersion = HashUtil.Sha1(data);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                        s_thisNTControlCenterServicesFileVersion = string.Empty;
                    }
                }
                return s_thisNTControlCenterServicesFileVersion;
            }
        }
    }
}

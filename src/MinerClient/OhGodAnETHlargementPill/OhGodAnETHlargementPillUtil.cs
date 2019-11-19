using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// 注意不要挪动这里的命名空间也不要挪动该代码文件所处的程序集
/// 嵌入的资源的位置和命名空间有关契约关系
/// </summary>
namespace NTMiner.OhGodAnETHlargementPill {
    public static class OhGodAnETHlargementPillUtil {
        private static readonly string s_processName = "OhGodAnETHlargementPill-r2";
        private static readonly string s_fileFullName = Path.Combine(MainAssemblyInfo.TempDirFullName, s_processName + ".exe");

        public static void Start() {
            try {
                if (NTMinerRoot.Instance.GpuSet.AsEnumerable().Any(a => a.Name.IndexOf("1080", StringComparison.OrdinalIgnoreCase) != -1)) {
                    ExtractResource();
                    Process[] processes = Process.GetProcessesByName(s_processName);
                    if (processes == null || processes.Length == 0) {
                        try {
                            using (Process proc = new Process()) {
                                proc.StartInfo.CreateNoWindow = true;
                                proc.StartInfo.UseShellExecute = false;
                                proc.StartInfo.FileName = s_fileFullName;
                                proc.Start();
                            }
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e);
                        }
                        Logger.OkWriteLine("小药丸启动成功");
                    }
                }
                else {
                    Logger.InfoDebugLine("没有发现1080卡，不适用小药丸");
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        public static void Stop() {
            try {
                if (NTMinerRoot.Instance.GpuSet.AsEnumerable().Any(a => a.Name.IndexOf("1080", StringComparison.OrdinalIgnoreCase) != -1)) {
                    Task.Factory.StartNew(() => {
                        Windows.TaskKill.Kill(s_processName, waitForExit: true);
                        Logger.OkWriteLine("成功停止小药丸");
                    });
                }
                else {
                    Logger.InfoDebugLine("没有发现1080卡，不适用小药丸");
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static void ExtractResource() {
            try {
                if (File.Exists(s_fileFullName)) {
                    return;
                }
                Type type = typeof(OhGodAnETHlargementPillUtil);
                Assembly assembly = type.Assembly;
                assembly.ExtractManifestResource(type, s_processName + ".exe", s_fileFullName);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}

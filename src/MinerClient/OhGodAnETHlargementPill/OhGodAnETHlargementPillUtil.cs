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
        private static readonly string _processName = "OhGodAnETHlargementPill-r2";
        private static readonly string _fileFullName = Path.Combine(TempPath.TempDirFullName, _processName + ".exe");

        public static void Start() {
            try {
                if (NTMinerContext.Instance.GpuSet.AsEnumerable().Any(a => a.Name.IndexOf("1080", StringComparison.OrdinalIgnoreCase) != -1)) {
                    ExtractResource();
                    Process[] processes = Process.GetProcessesByName(_processName);
                    if (processes == null || processes.Length == 0) {
                        try {
                            using (Process proc = new Process()) {
                                proc.StartInfo.CreateNoWindow = true;
                                proc.StartInfo.UseShellExecute = false;
                                proc.StartInfo.FileName = _fileFullName;
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
                if (NTMinerContext.Instance.GpuSet.AsEnumerable().Any(a => a.Name.IndexOf("1080", StringComparison.OrdinalIgnoreCase) != -1)) {
                    Task.Factory.StartNew(() => {
                        Windows.TaskKill.Kill(_processName, waitForExit: true);
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
                if (File.Exists(_fileFullName)) {
                    return;
                }
                Type type = typeof(OhGodAnETHlargementPillUtil);
                Assembly assembly = type.Assembly;
                assembly.ExtractManifestResource(type, _processName + ".exe", _fileFullName);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}

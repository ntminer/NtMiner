using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NTMiner.OhGodAnETHlargementPill {
    public static class OhGodAnETHlargementPillUtil {
        private static string processName = "OhGodAnETHlargementPill-r2";
        private static string tempDir = SpecialPath.TempDirFullName;
        private static string fileFullName = Path.Combine(tempDir, processName + ".exe");

        public static void Start() {
            try {
                if (NTMinerRoot.Current.GpuSet.Any(a => a.Name.IndexOf("1080", StringComparison.OrdinalIgnoreCase) != -1)) {
                    ExtractResource();
                    Process[] processes = Process.GetProcessesByName(processName);
                    if (processes == null || processes.Length == 0) {
                        try {
                            using (Process proc = new Process()) {
                                proc.StartInfo.CreateNoWindow = true;
                                proc.StartInfo.UseShellExecute = false;
                                proc.StartInfo.FileName = fileFullName;
                                proc.Start();
                            }
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e.Message, e);
                        }
                        Logger.OkWriteLine("小药丸启动成功");
                    }
                }
                else {
                    Logger.InfoDebugLine("没有发现1080卡，不适用小药丸");
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        public static void Stop() {
            try {
                if (NTMinerRoot.Current.GpuSet.Any(a => a.Name.IndexOf("1080", StringComparison.OrdinalIgnoreCase) != -1)) {
                    Process[] processes = Process.GetProcessesByName(processName);
                    if (processes != null && processes.Length != 0) {
                        try {
                            Windows.TaskKill.Kill(processName);
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e.Message, e);
                        }
                        Logger.OkWriteLine("成功停止小药丸");
                    }
                }
                else {
                    Logger.InfoDebugLine("没有发现1080卡，不适用小药丸");
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        private static void ExtractResource() {
            try {
                if (File.Exists(fileFullName)) {
                    return;
                }
                Type type = typeof(OhGodAnETHlargementPillUtil);
                Assembly assembly = type.Assembly;
                using (var stream = assembly.GetManifestResourceStream(type, processName + ".exe")) {
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    File.WriteAllBytes(fileFullName, data);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}

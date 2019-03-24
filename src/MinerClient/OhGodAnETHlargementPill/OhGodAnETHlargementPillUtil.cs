using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NTMiner.OhGodAnETHlargementPill {
    public static class OhGodAnETHlargementPillUtil {
        private static string s_processName = "OhGodAnETHlargementPill-r2";
        private static string s_tempDir = SpecialPath.TempDirFullName;
        private static string s_fileFullName = Path.Combine(s_tempDir, s_processName + ".exe");

        public static void Start() {
            try {
                if (NTMinerRoot.Current.GpuSet.Any(a => a.Name.IndexOf("1080", StringComparison.OrdinalIgnoreCase) != -1)) {
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
                    Windows.TaskKill.Kill(s_processName);
                    Logger.OkWriteLine("成功停止小药丸");
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
                if (File.Exists(s_fileFullName)) {
                    return;
                }
                Type type = typeof(OhGodAnETHlargementPillUtil);
                Assembly assembly = type.Assembly;
                using (var stream = assembly.GetManifestResourceStream(type, s_processName + ".exe")) {
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    File.WriteAllBytes(s_fileFullName, data);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}

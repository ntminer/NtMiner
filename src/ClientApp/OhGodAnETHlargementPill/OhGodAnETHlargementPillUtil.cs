using NTMiner.Notifications;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NTMiner.OhGodAnETHlargementPill {
    public class OhGodAnETHlargementPillUtil {
        private static string processName = "OhGodAnETHlargementPill-r2";
        private static string tempDir = SpecialPath.TempDirFullName;
        private static string fileFullName = Path.Combine(tempDir, processName + ".exe");
        public static void Access() {
            VirtualRoot.Access<MineStartedEvent>(
                Guid.Parse("BBA02B60-9C4C-4DFC-B397-DB08611440E9"),
                "开始挖矿后启动1080ti小药丸",
                LogEnum.Console,
                action: message => {
                    if (NTMinerRoot.Current.GpuSet.Any(a => a.Name.IndexOf("1080", StringComparison.OrdinalIgnoreCase) != -1)) {
                        if (!string.Equals(message.MineContext.MainCoin.Algo, "ethash", StringComparison.OrdinalIgnoreCase)) {
                            return;
                        }
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
                            MainWindowViewModel.Current.Manager.CreateMessage()
                                 .Accent("#1751C3")
                                 .Background("#333")
                                 .HasBadge("Info")
                                 .HasMessage("小药丸启动成功")
                                 .Dismiss()
                                 .WithDelay(TimeSpan.FromSeconds(4))
                                 .Queue();
                        }
                    }
                    else {
                        Logger.InfoDebugLine("没有发现1080卡，不适用小药丸");
                    }
                });
            VirtualRoot.Access<MineStartedEvent>(
                Guid.Parse("0E9ED543-64F2-4A14-9CB4-CE839531BF2D"),
                "停止挖矿后停止1080ti小药丸",
                LogEnum.Console,
                action: message => {
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
                });
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

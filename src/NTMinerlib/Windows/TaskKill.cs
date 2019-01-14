using System.Diagnostics;

namespace NTMiner.Windows {
    public static class TaskKill {
        public static void Kill(string processName) {
            if (string.IsNullOrEmpty(processName)) {
                return;
            }
            try {
                Process[] processes = Process.GetProcessesByName(processName);
                if (processes != null && processes.Length != 0) {
                    foreach (var process in processes) {
                        try {
                            if (process.CloseMainWindow()) {
                                Global.Logger.InfoDebugLine("CloseMainWindow成功杀死进程");
                                continue;
                            }
                        }
                        catch (System.Exception e) {
                            Global.Logger.ErrorDebugLine(e.Message, e);
                        }
                        try {
                            process.Kill();
                            process.WaitForExit(10 * 1000);
                            Global.Logger.InfoDebugLine("Kill成功杀死进程");
                            continue;
                        }
                        catch (System.Exception e) {
                            Global.Logger.ErrorDebugLine(e.Message, e);
                        }
                    }
                    processes = Process.GetProcessesByName(processName);
                    if (processes != null && processes.Length != 0) {
                        Cmd.RunClose($"taskkill /F /T /IM {processName}.exe", string.Empty);
                    }
                }
            }
            catch (System.Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }

        public static void Kill(int processId) {
            if (processId <= 0) {
                return;
            }
            try {
                Process process = Process.GetProcessById(processId);
                if (process != null) {
                    try {
                        if (process.CloseMainWindow()) {
                            Global.Logger.InfoDebugLine("CloseMainWindow成功杀死进程");
                            return;
                        }
                    }
                    catch (System.Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                    }
                    try {
                        process.Kill();
                        process.WaitForExit(10 * 1000);
                        Global.Logger.InfoDebugLine("Kill成功杀死进程");
                        return;
                    }
                    catch (System.Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                    }
                    Cmd.RunClose($"taskkill /F /T /PID {processId}", string.Empty);
                }
            }
            catch (System.Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;

namespace NTMiner.Windows {
    public static class Cmd {
        public static void RunClose(string filePullName, string args, bool createNoWindow = true) {
            try {
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = createNoWindow;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = $"/C \"{filePullName}\" {args}";
                    if (Path.IsPathRooted(filePullName)) {
                        proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(filePullName);
                    }
                    proc.Start();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        public static void RunClose(string filePullName, string args, ref int exitCode) {
            try {
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = $"/C \"{filePullName}\" {args}";
                    if (Path.IsPathRooted(filePullName)) {
                        proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(filePullName);
                    }
                    proc.Start();
                    proc.WaitForExit(10 * 1000);
                    exitCode = proc.ExitCode;
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        public static void RunClose(string filePullName, string args, ref int exitCode, out string output) {
            try {
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardInput = true;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = $"/C \"{filePullName}\" {args}";
                    if (Path.IsPathRooted(filePullName)) {
                        proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(filePullName);
                    }
                    proc.Start();

                    output = proc.StandardOutput.ReadToEnd();// 注意：读取输出可能被阻塞
                    proc.WaitForExit(10 * 1000);
                    exitCode = proc.ExitCode;
                }
            }
            catch (Exception e) {
                output = string.Empty;
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}

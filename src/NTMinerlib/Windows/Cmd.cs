using System;
using System.Diagnostics;
using System.IO;

namespace NTMiner.Windows {
    public static class Cmd {
        const string cmdMsg = "注意cmd参数中不能带参数，参数必须放在args中";
        /// <summary>
        /// 注意cmd参数中不能带参数，参数必须放在args中
        /// </summary>
        public static void RunClose(string cmd, string args, bool waitForExit = false) {
            if (string.IsNullOrEmpty(cmd)) {
                return;
            }
            try {
                if (Path.IsPathRooted(cmd)) {
                    cmd = $"\"{cmd}\"";
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine($"cmd={cmd} {cmdMsg}");
                Logger.ErrorDebugLine(e);
            }
            try {
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = $"/C {cmd} {args}";
                    proc.StartInfo.WorkingDirectory = AssemblyInfo.LocalDirFullName;
                    proc.Start();
                    if (waitForExit) {
                        proc.WaitForExit(10 * 1000);
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        /// <summary>
        /// 注意cmd参数中不能带参数，参数必须放在args中
        /// </summary>
        public static void RunClose(string cmd, string args, ref int exitCode) {
            if (string.IsNullOrEmpty(cmd)) {
                return;
            }
            try {
                if (Path.IsPathRooted(cmd)) {
                    cmd = $"\"{cmd}\"";
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine($"cmd={cmd} {cmdMsg}");
                Logger.ErrorDebugLine(e);
            }
            try {
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = $"/C {cmd} {args}";
                    proc.StartInfo.WorkingDirectory = AssemblyInfo.LocalDirFullName;
                    proc.Start();
                    proc.WaitForExit(10 * 1000);
                    exitCode = proc.ExitCode;
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        /// <summary>
        /// 注意cmd参数中不能带参数，参数必须放在args中
        /// </summary>
        public static void RunClose(string cmd, string args, ref int exitCode, out string output) {
            if (string.IsNullOrEmpty(cmd)) {
                output = string.Empty;
                return;
            }
            try {
                if (Path.IsPathRooted(cmd)) {
                    cmd = $"\"{cmd}\"";
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine($"cmd={cmd} {cmdMsg}");
                Logger.ErrorDebugLine(e);
            }
            try {
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardInput = true;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = $"/C {cmd} {args}";
                    proc.StartInfo.WorkingDirectory = AssemblyInfo.LocalDirFullName;
                    proc.Start();

                    output = proc.StandardOutput.ReadToEnd();// 注意：读取输出可能被阻塞
                    proc.WaitForExit(10 * 1000);
                    exitCode = proc.ExitCode;
                }
            }
            catch (Exception e) {
                output = string.Empty;
                Logger.ErrorDebugLine(e);
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;

namespace NTMiner.Windows {
    public static class Cmd {
        const string cmdMsg = "注意cmdName参数中不能带参数，参数必须放在args中";
        /// <summary>
        /// 注意cmdName参数中不能带参数，参数必须放在args中
        /// </summary>
        public static void RunClose(string cmdName, string args, bool waitForExit = false) {
            if (string.IsNullOrEmpty(cmdName)) {
                return;
            }
            try {
                if (Path.IsPathRooted(cmdName)) {
                    cmdName = $"\"{cmdName}\"";
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine($"cmdName={cmdName} {cmdMsg}");
                Logger.ErrorDebugLine(e);
            }
            try {
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = $"/C {cmdName} {args}";
                    proc.StartInfo.WorkingDirectory = MainAssemblyInfo.TempDirFullName;
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
        public static void RunClose(string cmdName, string args, ref int exitCode) {
            if (string.IsNullOrEmpty(cmdName)) {
                return;
            }
            try {
                if (Path.IsPathRooted(cmdName)) {
                    cmdName = $"\"{cmdName}\"";
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine($"cmdName={cmdName} {cmdMsg}");
                Logger.ErrorDebugLine(e);
            }
            try {
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = $"/C {cmdName} {args}";
                    proc.StartInfo.WorkingDirectory = MainAssemblyInfo.TempDirFullName;
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
        public static void RunClose(string cmdName, string args, ref int exitCode, out string output) {
            if (string.IsNullOrEmpty(cmdName)) {
                output = string.Empty;
                return;
            }
            try {
                if (Path.IsPathRooted(cmdName)) {
                    cmdName = $"\"{cmdName}\"";
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine($"cmdName={cmdName} {cmdMsg}");
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
                    proc.StartInfo.Arguments = $"/C {cmdName} {args}";
                    proc.StartInfo.WorkingDirectory = MainAssemblyInfo.TempDirFullName;
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

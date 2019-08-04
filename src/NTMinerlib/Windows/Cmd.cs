using System;
using System.Diagnostics;
using System.IO;

namespace NTMiner.Windows {
    public static class Cmd {
        public static void RunClose(string filePullName, string args, bool waitForExit = false) {
            if (string.IsNullOrEmpty(filePullName)) {
                return;
            }
            try {
                if (Path.IsPathRooted(filePullName)) {
                    filePullName = $"\"{filePullName}\"";
                }
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = $"/C {filePullName} {args}";
                    proc.StartInfo.WorkingDirectory = AssemblyInfo.LocalDirFullName;
                    proc.Start();
                    if (waitForExit) {
                        proc.WaitForExit(10 * 1000);
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine($"filePullName={filePullName}, args={args}");
                Logger.ErrorDebugLine(e);
            }
        }

        public static void RunClose(string filePullName, string args, ref int exitCode) {
            if (string.IsNullOrEmpty(filePullName)) {
                return;
            }
            try {
                if (Path.IsPathRooted(filePullName)) {
                    filePullName = $"\"{filePullName}\"";
                }
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = $"/C {filePullName} {args}";
                    proc.StartInfo.WorkingDirectory = AssemblyInfo.LocalDirFullName;
                    proc.Start();
                    proc.WaitForExit(10 * 1000);
                    exitCode = proc.ExitCode;
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine($"filePullName={filePullName}, args={args}");
                Logger.ErrorDebugLine(e);
            }
        }

        public static void RunClose(string filePullName, string args, ref int exitCode, out string output) {
            if (string.IsNullOrEmpty(filePullName)) {
                output = string.Empty;
                return;
            }
            try {
                if (Path.IsPathRooted(filePullName)) {
                    filePullName = $"\"{filePullName}\"";
                }
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardInput = true;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = $"/C {filePullName} {args}";
                    proc.StartInfo.WorkingDirectory = AssemblyInfo.LocalDirFullName;
                    proc.Start();

                    output = proc.StandardOutput.ReadToEnd();// 注意：读取输出可能被阻塞
                    proc.WaitForExit(10 * 1000);
                    exitCode = proc.ExitCode;
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine($"filePullName={filePullName}, args={args}");
                output = string.Empty;
                Logger.ErrorDebugLine(e);
            }
        }
    }
}

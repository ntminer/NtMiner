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
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = $"/C \"{filePullName}\" {args}";
                    proc.StartInfo.WorkingDirectory = VirtualRoot.GlobalDirFullName;
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

        public static void RunClose(string filePullName, string args, ref int exitCode) {
            if (string.IsNullOrEmpty(filePullName)) {
                return;
            }
            try {
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = $"/C \"{filePullName}\" {args}";
                    proc.StartInfo.WorkingDirectory = VirtualRoot.GlobalDirFullName;
                    proc.Start();
                    proc.WaitForExit(10 * 1000);
                    exitCode = proc.ExitCode;
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        public static void RunClose(string filePullName, string args, ref int exitCode, out string output) {
            if (string.IsNullOrEmpty(filePullName)) {
                output = string.Empty;
                return;
            }
            try {
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardInput = true;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = $"/C \"{filePullName}\" {args}";
                    proc.StartInfo.WorkingDirectory = VirtualRoot.GlobalDirFullName;
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

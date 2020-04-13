using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace NTMiner.Windows {
    public static class Cmd {
        #region cmd_here
        private static readonly string _cmdHere0 = "SOFTWARE\\Classes\\Directory\\shell\\cmd_here";
        private static readonly string _cmdHere1 = "SOFTWARE\\Classes\\Directory\\background\\shell\\cmd_here";
        private static readonly string _cmdPrompt = "SOFTWARE\\Classes\\Folder\\shell\\cmdPrompt";

        public static bool IsRegedCmdHere() {
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(_cmdHere0)) {
                if (registryKey != null) {
                    registryKey.Close();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 在Windows Directory和Folder右键上下文菜单中添加“命令行”菜单
        /// </summary>
        public static void RegCmdHere() {
            try {
                string cmdHereCommand0 = _cmdHere0 + "\\command";
                WinRegistry.SetValue(Registry.LocalMachine, _cmdHere0, "", "命令行");
                WinRegistry.SetValue(Registry.LocalMachine, _cmdHere0, "Icon", "cmd.exe");
                WinRegistry.SetValue(Registry.LocalMachine, cmdHereCommand0, "", "\"cmd.exe\"");

                string cmdHereCommand1 = _cmdHere1 + "\\command";
                WinRegistry.SetValue(Registry.LocalMachine, _cmdHere1, "", "命令行");
                WinRegistry.SetValue(Registry.LocalMachine, _cmdHere1, "Icon", "cmd.exe");
                WinRegistry.SetValue(Registry.LocalMachine, cmdHereCommand1, "", "\"cmd.exe\"");

                string cmdPromptCommand = _cmdPrompt + "\\command";
                WinRegistry.SetValue(Registry.LocalMachine, _cmdPrompt, "", "命令行");
                WinRegistry.SetValue(Registry.LocalMachine, cmdPromptCommand, "", "\"cmd.exe\" \"cd %1\"");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        public static void UnRegCmdHere() {
            RegistryKey root = Registry.LocalMachine;
            try {
                root.DeleteSubKeyTree(_cmdHere0);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }

            try {
                root.DeleteSubKeyTree(_cmdHere1);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }

            try {
                root.DeleteSubKeyTree(_cmdPrompt);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
        #endregion

        const string cmdMsg = "注意cmdName参数中不能带参数，参数必须放在args中";
        /// <summary>
        /// 注意cmdName参数中不能带参数，参数必须放在args中
        /// </summary>
        public static void RunClose(string cmdName, string args, bool waitForExit = false, bool createNoWindow = true) {
            if (string.IsNullOrEmpty(cmdName)) {
                return;
            }
            try {
                if (Path.IsPathRooted(cmdName)) {
                    cmdName = $"\"{cmdName}\"";
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine($"cmdName={cmdName} {cmdMsg}", e);
            }
            try {
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = createNoWindow;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.FileName = cmdName;
                    proc.StartInfo.Arguments = $"{(string.IsNullOrEmpty(args) ? string.Empty : " ")}{args}";
                    proc.StartInfo.WorkingDirectory = EntryAssemblyInfo.TempDirFullName;
                    proc.Start();
                    if (waitForExit) {
                        proc.WaitForExit(10 * 1000);
                    }
                    proc.Close();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        /// <summary>
        /// 注意cmdName参数中不能带参数，参数必须放在args中
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
                Logger.ErrorDebugLine($"cmdName={cmdName} {cmdMsg}", e);
            }
            try {
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.FileName = cmdName;
                    proc.StartInfo.Arguments = $"{(string.IsNullOrEmpty(args) ? string.Empty : " ")}{args}";
                    proc.StartInfo.WorkingDirectory = EntryAssemblyInfo.TempDirFullName;
                    proc.Start();
                    proc.WaitForExit(10 * 1000);
                    exitCode = proc.ExitCode;
                    proc.Close();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        /// <summary>
        /// 注意cmdName参数中不能带参数，参数必须放在args中
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
                Logger.ErrorDebugLine($"cmdName={cmdName} {cmdMsg}", e);
            }
            try {
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardInput = true;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.FileName = cmdName;
                    proc.StartInfo.Arguments = $"{(string.IsNullOrEmpty(args) ? string.Empty : " ")}{args}";
                    proc.StartInfo.WorkingDirectory = EntryAssemblyInfo.TempDirFullName;
                    proc.Start();

                    output = proc.StandardOutput.ReadToEnd();// 注意：读取输出可能被阻塞
                    proc.WaitForExit(10 * 1000);
                    exitCode = proc.ExitCode;
                    proc.Close();
                }
            }
            catch (Exception e) {
                output = string.Empty;
                Logger.ErrorDebugLine(e);
            }
        }
    }
}

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;

namespace NTMiner {
    public class NTMinerDaemonService : INTMinerDaemonService {
        public string GetDaemonVersion() {
            return Global.Sha1;
        }

        public void RestartWindows() {
            Windows.Power.Restart();
        }

        public void ShutdownWindows() {
            Windows.Power.Shutdown();
        }

        private string GetNTMinerLocation() {
            object locationValue = Windows.Registry.GetValue(Registry.Users, ClientId.NTMinerRegistrySubKey, "Location");
            if (locationValue != null) {
                return (string)locationValue;
            }
            return string.Empty;
        }

        private string GetNTMinerArguments(Guid workId) {
            object argumentsValue = Windows.Registry.GetValue(Registry.Users, ClientId.NTMinerRegistrySubKey, "Arguments");
            if (argumentsValue != null) {
                string arguments = (string)argumentsValue;
                string[] parts = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                int workIdIndex = -1;
                int controlCenterIndex = -1;
                for (int i = 0; i < parts.Length; i++) {
                    string item = parts[i];
                    if (item.StartsWith("--workid=")) {
                        workIdIndex = i;
                    }
                    else if (item.StartsWith("--controlcenter")) {
                        controlCenterIndex = i;
                    }
                }
                if (workId == Guid.Empty) {
                    if (workIdIndex != -1) {
                        parts[workIdIndex] = string.Empty;
                    }
                    if (controlCenterIndex != -1) {
                        parts[controlCenterIndex] = string.Empty;
                    }
                }
                else {
                    if (workIdIndex != -1) {
                        parts[workIdIndex] = "--workid=" + workId;
                    }
                    else {
                        Array.Resize(ref parts, parts.Length + 1);
                        parts[parts.Length - 1] = "--workid=" + workId;
                    }
                }
                return string.Join(" ", parts);
            }
            else if (workId != Guid.Empty) {
                return "--workid=" + workId;
            }
            return string.Empty;
        }

        public void OpenNTMiner(Guid workId) {
            try {
                string location = GetNTMinerLocation();
                if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                    string arguments = string.Empty;
                    if (workId != Guid.Empty) {
                        arguments = "--workid=" + workId.ToString();
                    }
                    Windows.Cmd.RunClose(location, arguments);
                }
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
            }
        }

        private static NamedPipeClientStream _pipeClient;
        private static void CloseNTMinerMainWindow() {
            try {
                Process[] ntMinerProcesses = Process.GetProcessesByName("NTMiner");
                if (ntMinerProcesses.Length == 0) {
                    return;
                }
                _pipeClient = new NamedPipeClientStream(".", "ntminerclient", PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);
                _pipeClient.Connect(200);
                StreamWriter sw = new StreamWriter(_pipeClient);
                sw.WriteLine("CloseMainWindow");
                Thread.Sleep(1000);
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex) {
                Global.Logger.Error(ex.Message, ex);
            }
        }

        public void RestartNTMiner(Guid workId) {
            try {
                CloseNTMinerMainWindow();
                string location = GetNTMinerLocation();
                string arguments = GetNTMinerArguments(workId);
                if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                    Windows.Cmd.RunClose(location, arguments);
                }
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
            }
        }

        public void CloseNTMiner() {
            try {
                string location = GetNTMinerLocation();
                if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                    string processName = Path.GetFileNameWithoutExtension(location);
                    Process[] processes = Process.GetProcessesByName(processName);
                    if (processes.Length != 0) {
                        Windows.TaskKill.Kill(processName);
                    }
                }
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
            }
        }

        public bool IsNTMinerDaemonOnline() {
            return true;
        }

        public bool IsNTMinerOnline() {
            try {
                string location = GetNTMinerLocation();
                if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                    Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(location));
                    return processes.Length != 0;
                }
                return false;
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
                return false;
            }
        }

        public void StartMine(
            Guid contextId, 
            string minerName,
            string coin, 
            string poolIp, 
            int poolPort, 
            string ourWallet, 
            string testWallet, 
            string kernelName) {
            NoDevFee.NoDevFeeUtil.Start(contextId, minerName, coin, poolIp, ourWallet, testWallet, kernelName);
        }

        public void StopMine() {
            NoDevFee.NoDevFeeUtil.Stop();
        }

        public void Dispose() {
            // nothing need todo
        }
    }
}

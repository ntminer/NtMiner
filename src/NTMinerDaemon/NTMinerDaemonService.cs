using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

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
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }

        public void RestartNTMiner(Guid workId) {
            try {
                CloseNTMiner();
                string location = GetNTMinerLocation();
                string arguments = GetNTMinerArguments(workId);
                if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                    Windows.Cmd.RunClose(location, arguments);
                }
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }

        public void CloseNTMiner() {
            try {
                string location = GetNTMinerLocation();
                if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                    string processName = Path.GetFileNameWithoutExtension(location);
                    Windows.TaskKill.Kill(processName);
                }
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
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
                Global.Logger.ErrorDebugLine(e.Message, e);
                return false;
            }
        }

        public void StartMine(
            int contextId, 
            string minerName,
            string coin, 
            string ourWallet, 
            string testWallet, 
            string kernelName) {
            NoDevFee.NoDevFeeUtil.StartAsync(contextId, minerName, coin, ourWallet, testWallet, kernelName);
        }

        public void StopMine() {
            NoDevFee.NoDevFeeUtil.Stop();
        }

        public void Dispose() {
            // nothing need todo
        }
    }
}

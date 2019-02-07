using System;
using System.IO;
using System.Web.Http;

namespace NTMiner {
    public class NTMinerDaemonController : ApiController {
        [HttpPost]
        public string GetDaemonVersion() {
            return Global.Sha1;
        }

        [HttpPost]
        public void RestartWindows() {
            Windows.Power.Restart();
        }

        [HttpPost]
        public void ShutdownWindows() {
            Windows.Power.Shutdown();
        }

        [HttpPost]
        public void OpenNTMiner(Guid workId) {
            try {
                string location = NTMinerRegistry.GetLocation();
                if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                    string arguments = string.Empty;
                    if (workId != Guid.Empty) {
                        arguments = "workid=" + workId.ToString();
                    }
                    Windows.Cmd.RunClose(location, arguments);
                }
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }

        [HttpPost]
        public void RestartNTMiner(Guid workId) {
            try {
                CloseNTMiner();
                string location = NTMinerRegistry.GetLocation();
                string arguments = NTMinerRegistry.GetArguments();
                if (!string.IsNullOrEmpty(arguments)) {
                    string[] parts = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    int workIdIndex = -1;
                    int controlCenterIndex = -1;
                    for (int i = 0; i < parts.Length; i++) {
                        string item = parts[i];
                        if (item.StartsWith("workid=")) {
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
                            parts[workIdIndex] = "workid=" + workId;
                        }
                        else {
                            Array.Resize(ref parts, parts.Length + 1);
                            parts[parts.Length - 1] = "workid=" + workId;
                        }
                    }
                    arguments = string.Join(" ", parts);
                }
                else if (workId != Guid.Empty) {
                    arguments = "workid=" + workId;
                }
                if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                    Windows.Cmd.RunClose(location, arguments);
                }
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }

        [HttpPost]
        public void CloseNTMiner() {
            try {
                string location = NTMinerRegistry.GetLocation();
                if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                    string processName = Path.GetFileNameWithoutExtension(location);
                    Windows.TaskKill.Kill(processName);
                }
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }

        [HttpPost]
        public void UpgradeNTMiner(string ntminerFileName) {
            try {
                if (string.IsNullOrEmpty(ntminerFileName)) {
                    return;
                }
                string location = NTMinerRegistry.GetLocation();
                if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                    string arguments = "upgrade=" + ntminerFileName;
                    Windows.Cmd.RunClose(location, arguments);
                }
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }

        [HttpPost]
        public void StartNoDevFee(
            int contextId,
            string minerName,
            string coin,
            string ourWallet,
            string testWallet,
            string kernelName) {
            NoDevFee.NoDevFeeUtil.StartAsync(contextId, minerName, coin, ourWallet, testWallet, kernelName);
        }

        [HttpPost]
        public void StopNoDevFee() {
            NoDevFee.NoDevFeeUtil.Stop();
        }
    }
}

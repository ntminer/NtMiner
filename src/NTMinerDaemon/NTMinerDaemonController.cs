using NTMiner.Daemon;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace NTMiner {
    public class NTMinerDaemonController : ApiController {
        private static string _sha1 = null;
        public static string Sha1 {
            get {
                if (_sha1 == null) {
                    _sha1 = HashUtil.Sha1(File.ReadAllBytes(Process.GetCurrentProcess().MainModule.FileName));
                }
                return _sha1;
            }
        }
        [HttpPost]
        public string GetDaemonVersion() {
            return Sha1;
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
        public void OpenNTMiner([FromBody]OpenNTMinerRequest request) {
            try {
                string location = NTMinerRegistry.GetLocation();
                if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                    string arguments = string.Empty;
                    if (request.WorkId != Guid.Empty) {
                        arguments = "workid=" + request.WorkId.ToString();
                    }
                    Windows.Cmd.RunClose(location, arguments);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        [HttpPost]
        public void RestartNTMiner([FromBody]RestartNTMinerRequest request) {
            Task.Factory.StartNew(() => {
                try {
                    CloseNTMiner();
                    System.Threading.Thread.Sleep(1000);
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
                        if (request.WorkId == Guid.Empty) {
                            if (workIdIndex != -1) {
                                parts[workIdIndex] = string.Empty;
                            }
                            if (controlCenterIndex != -1) {
                                parts[controlCenterIndex] = string.Empty;
                            }
                        }
                        else {
                            if (workIdIndex != -1) {
                                parts[workIdIndex] = "workid=" + request.WorkId;
                            }
                            else {
                                Array.Resize(ref parts, parts.Length + 1);
                                parts[parts.Length - 1] = "workid=" + request.WorkId;
                            }
                        }
                        arguments = string.Join(" ", parts);
                    }
                    else if (request.WorkId != Guid.Empty) {
                        arguments = "workid=" + request.WorkId;
                    }
                    if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                        Windows.Cmd.RunClose(location, arguments);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                }
            });
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
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        [HttpPost]
        public void UpgradeNTMiner([FromBody]UpgradeNTMinerRequest request) {
            try {
                if (string.IsNullOrEmpty(request.NTMinerFileName)) {
                    return;
                }
                string location = NTMinerRegistry.GetLocation();
                if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                    string arguments = "upgrade=" + request.NTMinerFileName;
                    Windows.Cmd.RunClose(location, arguments);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        [HttpPost]
        public void StartNoDevFee([FromBody]StartNoDevFeeRequest request) {
            NoDevFee.NoDevFeeUtil.StartAsync(request.ContextId, request.MinerName, request.Coin, request.OurWallet, request.TestWallet, request.KernelName);
        }

        [HttpPost]
        public void StopNoDevFee() {
            NoDevFee.NoDevFeeUtil.Stop();
        }
    }
}

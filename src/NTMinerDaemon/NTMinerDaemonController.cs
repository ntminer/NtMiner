using NTMiner.Daemon;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
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
        public ResponseBase RestartWindows([FromBody]RequestBase request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty);
            }
            TimeSpan.FromSeconds(2).Delay().ContinueWith(t => {
                Windows.Power.Restart();
            });
            return ResponseBase.Ok(request.MessageId);
        }

        [HttpPost]
        public ResponseBase ShutdownWindows([FromBody]RequestBase request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty);
            }
            TimeSpan.FromSeconds(2).Delay().ContinueWith(t => {
                Windows.Power.Shutdown();
            });
            return ResponseBase.Ok(request.MessageId);
        }

        [HttpPost]
        public ResponseBase OpenNTMiner([FromBody]OpenNTMinerRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty);
            }
            try {
                string location = NTMinerRegistry.GetLocation();
                if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                    string arguments = string.Empty;
                    if (request.WorkId != Guid.Empty) {
                        arguments = "workid=" + request.WorkId.ToString();
                    }
                    Windows.Cmd.RunClose(location, arguments);
                }
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        [HttpPost]
        public ResponseBase RestartNTMiner([FromBody]RestartNTMinerRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty);
            }
            Task.Factory.StartNew(() => {
                try {
                    DoCloseNTMiner();
                    System.Threading.Thread.Sleep(1000);
                    string arguments = NTMinerRegistry.GetArguments();
                    if (!string.IsNullOrEmpty(arguments)) {
                        string[] parts = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        int workIdIndex = -1;
                        for (int i = 0; i < parts.Length; i++) {
                            string item = parts[i];
                            if (item.StartsWith("workid=")) {
                                workIdIndex = i;
                            }
                        }
                        if (request.WorkId == Guid.Empty) {
                            if (workIdIndex != -1) {
                                parts[workIdIndex] = string.Empty;
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
                    string location = NTMinerRegistry.GetLocation();
                    if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                        Windows.Cmd.RunClose(location, arguments);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                }
            });
            return ResponseBase.Ok(request.MessageId);
        }

        [HttpPost]
        public ResponseBase CloseNTMiner([FromBody]RequestBase request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty);
            }
            Task.Factory.StartNew(() => {
                DoCloseNTMiner();
            });
            return ResponseBase.Ok(request.MessageId);
        }

        private void DoCloseNTMiner() {
            bool isClosed = false;
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:3336/api/MinerClient/CloseNTMiner", new RequestBase());
                    ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    isClosed = response.IsSuccess();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
            if (!isClosed) {
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
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{NTMinerRegistry.GetMinerServerHost()}:3339/api/Report/ReportState", new MinerServer.ReportStateRequest {
                        ClientId = NTMinerRegistry.GetClientId(),
                        IsMining = false
                    });
                    Write.DevLine("ReportStateAsync " + message.Result.ReasonPhrase);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        [HttpPost]
        public ResponseBase UpgradeNTMiner([FromBody]UpgradeNTMinerRequest request) {
            if (request == null || string.IsNullOrEmpty(request.NTMinerFileName)) {
                return ResponseBase.InvalidInput(Guid.Empty);
            }
            Task.Factory.StartNew(() => {
                try {
                    string location = NTMinerRegistry.GetLocation();
                    if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                        string arguments = "upgrade=" + request.NTMinerFileName;
                        Windows.Cmd.RunClose(location, arguments);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                }
            });
            return ResponseBase.Ok(request.MessageId);
        }

        [HttpPost]
        public ResponseBase StartNoDevFee([FromBody]StartNoDevFeeRequest request) {
            string message;
            NoDevFee.NoDevFeeUtil.StartAsync(request.ContextId, request.MinerName, request.Coin, request.OurWallet, request.TestWallet, request.KernelName, out message);
            return ResponseBase.Ok(request.MessageId, message);
        }

        [HttpPost]
        public ResponseBase StopNoDevFee([FromBody]RequestBase request) {
            NoDevFee.NoDevFeeUtil.Stop();
            return ResponseBase.Ok(request.MessageId);
        }
    }
}

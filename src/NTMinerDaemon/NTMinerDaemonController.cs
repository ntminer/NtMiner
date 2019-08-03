using NTMiner.Controllers;
using NTMiner.Daemon;
using NTMiner.RemoteDesktopEnabler;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace NTMiner {
    /// <summary>
    /// 端口号：<see cref="Consts.NTMinerDaemonPort"/>
    /// </summary>
    public class NTMinerDaemonController : ApiController, INTMinerDaemonController {
        [HttpPost]
        public ResponseBase EnableWindowsRemoteDesktop() {
            try {
                Rdp.SetRdpEnabled(true, true);
                Firewall.AddRemoteDesktopRule();
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public void CloseDaemon() {
            // 延迟100毫秒再退出从而避免当前的CloseDaemon请求尚未收到响应
            TimeSpan.FromMilliseconds(100).Delay().ContinueWith(t => {
                HostRoot.Exit();
            });
        }

        #region GetGpuProfilesJson
        [HttpPost]
        public string GetGpuProfilesJson() {
            try {
                return SpecialPath.ReadGpuProfilesJsonFile();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return string.Empty;
            }
        }
        #endregion

        #region SaveGpuProfilesJson
        [HttpPost]
        public void SaveGpuProfilesJson() {
            try {
                string json = Request.Content.ReadAsStringAsync().Result;
                SpecialPath.SaveGpuProfilesJsonFile(json);
                if (IsNTMinerOpened()) {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:{Consts.MinerClientPort}/api/MinerClient/OverClock", null);
                        Write.DevDebug($"{nameof(SaveGpuProfilesJson)} {message.Result.ReasonPhrase}");
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
        #endregion

        [HttpPost]
        public void SetAutoBootStart([FromUri]bool autoBoot, [FromUri]bool autoStart) {
            NTMinerRegistry.SetIsAutoBoot(autoBoot);
            NTMinerRegistry.SetIsAutoStart(autoStart);
            if (IsNTMinerOpened()) {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:{Consts.MinerClientPort}/api/MinerClient/RefreshAutoBootStart", null);
                    Write.DevDebug($"{nameof(SetAutoBootStart)} {message.Result.ReasonPhrase}");
                }
            }
        }

        [HttpPost]
        public ResponseBase RestartWindows([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                Windows.Power.Restart(10);
                CloseNTMiner();
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase ShutdownWindows([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                Windows.Power.Shutdown(10);
                CloseNTMiner();
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        private static bool IsNTMinerOpened() {
            string location = NTMinerRegistry.GetLocation();
            if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                string processName = Path.GetFileNameWithoutExtension(location);
                Process[] processes = Process.GetProcessesByName(processName);
                return processes.Length != 0;
            }
            return false;
        }

        [HttpPost]
        public ResponseBase StartMine([FromBody]WorkRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                ResponseBase response;
                if (request.WorkId != Guid.Empty) {
                    File.WriteAllText(SpecialPath.NTMinerLocalJsonFileFullName, request.LocalJson);
                    File.WriteAllText(SpecialPath.NTMinerServerJsonFileFullName, request.ServerJson);
                }
                string location = NTMinerRegistry.GetLocation();
                if (IsNTMinerOpened()) {
                    using (HttpClient client = new HttpClient()) {
                        WorkRequest innerRequest = new WorkRequest {
                            WorkId = request.WorkId
                        };
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:{Consts.MinerClientPort}/api/MinerClient/StartMine", innerRequest);
                        response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        return response;
                    }
                }
                else {
                    if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                        string arguments = "--AutoStart";
                        if (request.WorkId != Guid.Empty) {
                            arguments += " --work";
                        }
                        Windows.Cmd.RunClose(location, arguments);
                        return ResponseBase.Ok();
                    }
                    return ResponseBase.ServerError("挖矿端程序不存在");
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase StopMine([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                ResponseBase response;
                if (!IsNTMinerOpened()) {
                    return ResponseBase.Ok();
                }
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:{Consts.MinerClientPort}/api/MinerClient/StopMine", request);
                        response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        return response;
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase RestartNTMiner([FromBody]WorkRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            if (request.WorkId != Guid.Empty) {
                File.WriteAllText(SpecialPath.NTMinerLocalJsonFileFullName, request.LocalJson);
                File.WriteAllText(SpecialPath.NTMinerServerJsonFileFullName, request.ServerJson);
            }
            Task.Factory.StartNew(() => {
                try {
                    if (IsNTMinerOpened()) {
                        CloseNTMiner();
                        System.Threading.Thread.Sleep(1000);
                    }
                    string arguments = string.Empty;
                    if (request.WorkId != Guid.Empty) {
                        arguments = "--work";
                    }
                    string location = NTMinerRegistry.GetLocation();
                    if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                        Windows.Cmd.RunClose(location, arguments);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
            });
            return ResponseBase.Ok();
        }

        private void CloseNTMiner() {
            bool isClosed = false;
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:{Consts.MinerClientPort}/api/MinerClient/CloseNTMiner", new SignRequest { });
                    ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    isClosed = response.IsSuccess();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
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
                    Logger.ErrorDebugLine(e);
                }
            }
        }

        [HttpPost]
        public ResponseBase UpgradeNTMiner([FromBody]UpgradeNTMinerRequest request) {
            if (request == null || string.IsNullOrEmpty(request.NTMinerFileName)) {
                return ResponseBase.InvalidInput("参数错误");
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
                    Logger.ErrorDebugLine(e);
                }
            });
            return ResponseBase.Ok();
        }

        [HttpPost]
        public ResponseBase SetWallet([FromBody]SetWalletRequest request) {
            NoDevFee.NoDevFeeUtil.SetWallet(request.TestWallet);
            return ResponseBase.Ok();
        }
    }
}

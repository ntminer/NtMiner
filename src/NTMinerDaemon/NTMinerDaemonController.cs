using NTMiner.Controllers;
using NTMiner.Daemon;
using NTMiner.RemoteDesktop;
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
                Logger.InfoDebugLine("启用Windows远程桌面");
                Rdp.SetRdpEnabled(true);
                Firewall.AddRdpRule();
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public void CloseDaemon() {
            Logger.InfoDebugLine("退出守护进程");
            // 延迟100毫秒再退出从而避免当前的CloseDaemon请求尚未收到响应
            100.MillisecondsDelay().ContinueWith(t => {
                HostRoot.Exit();
            });
        }

        #region GetGpuProfilesJson
        [HttpPost]
        public string GetGpuProfilesJson() {
            try {
                Logger.InfoDebugLine("获取显卡参数");
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
                Logger.InfoDebugLine("保存显卡参数");
                string json = Request.Content.ReadAsStringAsync().Result;
                SpecialPath.SaveGpuProfilesJsonFile(json);
                if (IsNTMinerOpened()) {
                    using (HttpClient client = RpcRoot.Create()) {
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://localhost:{NTKeyword.MinerClientPort.ToString()}/api/MinerClient/OverClock", null);
                        Write.DevDebug($"{nameof(SaveGpuProfilesJson)} {getHttpResponse.Result.ReasonPhrase}");
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
            Logger.InfoDebugLine($"开机启动{(autoBoot ? "√" : "×")}，自动挖矿{(autoStart ? "√" : "×")}");
            MinerProfileUtil.SetAutoStart(autoBoot, autoStart);
            if (IsNTMinerOpened()) {
                using (HttpClient client = RpcRoot.Create()) {
                    Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://localhost:{NTKeyword.MinerClientPort.ToString()}/api/MinerClient/RefreshAutoBootStart", null);
                    Write.DevDebug($"{nameof(SetAutoBootStart)} {getHttpResponse.Result.ReasonPhrase}");
                }
            }
        }

        [HttpPost]
        public ResponseBase RestartWindows([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                Logger.InfoDebugLine("重启矿机");
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
                Logger.InfoDebugLine("关机");
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
                Logger.InfoDebugLine("开始挖矿");
                ResponseBase response;
                if (request.WorkId != Guid.Empty) {
                    File.WriteAllText(SpecialPath.NTMinerLocalJsonFileFullName, request.LocalJson);
                    File.WriteAllText(SpecialPath.NTMinerServerJsonFileFullName, request.ServerJson);
                }
                string location = NTMinerRegistry.GetLocation();
                if (IsNTMinerOpened()) {
                    using (HttpClient client = RpcRoot.Create()) {
                        WorkRequest innerRequest = new WorkRequest {
                            WorkId = request.WorkId
                        };
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://localhost:{NTKeyword.MinerClientPort.ToString()}/api/MinerClient/StartMine", innerRequest);
                        response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        return response;
                    }
                }
                else {
                    if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                        string arguments = NTKeyword.AutoStartCmdParameterName;
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
                Logger.InfoDebugLine("停止挖矿");
                ResponseBase response;
                if (!IsNTMinerOpened()) {
                    return ResponseBase.Ok();
                }
                try {
                    using (HttpClient client = RpcRoot.Create()) {
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://localhost:{NTKeyword.MinerClientPort.ToString()}/api/MinerClient/StopMine", request);
                        response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
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
            Logger.InfoDebugLine("重启挖矿端");
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
            Logger.InfoDebugLine("退出挖矿端");
            bool isClosed = false;
            try {
                using (HttpClient client = RpcRoot.Create()) {
                    Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://localhost:{NTKeyword.MinerClientPort.ToString()}/api/MinerClient/CloseNTMiner", new SignRequest { });
                    ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
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
            Logger.InfoDebugLine($"升级挖矿端至{request.NTMinerFileName}");
            Task.Factory.StartNew(() => {
                try {
                    string location = NTMinerRegistry.GetLocation();
                    if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                        string arguments = NTKeyword.UpgradeCmdParameterName + request.NTMinerFileName;
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
            NoDevFee.EthWalletSet.Instance.SetWallet(request.TestWallet);
            return ResponseBase.Ok();
        }
    }
}

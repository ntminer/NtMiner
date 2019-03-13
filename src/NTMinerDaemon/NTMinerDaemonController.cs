using Microsoft.Win32;
using NTMiner.Controllers;
using NTMiner.Daemon;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace NTMiner {
    public class NTMinerDaemonController : ApiController, INTMinerDaemonController {
        private static string s_sha1 = null;
        public static string Sha1 {
            get {
                if (s_sha1 == null) {
                    s_sha1 = HashUtil.Sha1(File.ReadAllBytes(Process.GetCurrentProcess().MainModule.FileName));
                }
                return s_sha1;
            }
        }

        [HttpPost]
        public string GetDaemonVersion() {
            return Sha1;
        }

        [HttpPost]
        public void CloseDaemon() {
            HostRoot.WaitHandle.Set();
        }

        [HttpPost]
        public void RefreshNotifyIcon() {
            HostRoot.NotifyIcon?.RefreshIcon();
        }

        [HttpPost]
        public ResponseBase SetMinerName([FromBody]MinerClient.SetMinerNameRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                SetMinerNameAsync(request, response1 => {
                    if (!response1.IsSuccess()) {
                        ResponseBase response;
                        if (request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                            if (!string.IsNullOrEmpty(request.MinerName)) {
                                request.MinerName = new string(request.MinerName.ToCharArray().Where(a => !MinerNameConst.InvalidChars.Contains(a)).ToArray());
                            }
                            Windows.Registry.SetValue(Registry.Users, NTMinerRegistry.NTMinerRegistrySubKey, "MinerName", request.MinerName ?? string.Empty);
                        }
                    }
                });
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        private static void SetMinerNameAsync(MinerClient.SetMinerNameRequest request, Action<ResponseBase> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:{WebApiConst.MinerClientAppPort}/api/MinerClient/SetMinerName", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(null);
                }
            });
        }

        [HttpPost]
        public void RefreshUserSet() {
            HostRoot.Current.UserSet.Refresh();
        }

        [HttpPost]
        public ResponseBase RestartWindows([FromBody]SignatureRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                TimeSpan.FromSeconds(2).Delay().ContinueWith(t => {
                    Windows.Power.Restart();
                });
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        [HttpPost]
        public ResponseBase ShutdownWindows([FromBody]SignatureRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                TimeSpan.FromSeconds(2).Delay().ContinueWith(t => {
                    Windows.Power.Shutdown();
                });
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
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
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }
                IUser user = HostRoot.Current.UserSet.GetUser(request.LoginName);
                if (user == null) {
                    return ResponseBase.Forbidden(request.MessageId);
                }

                if (request.WorkId != Guid.Empty) {
                    File.WriteAllText(SpecialPath.NTMinerLocalJsonFileFullName, request.LocalJson);
                    File.WriteAllText(SpecialPath.NTMinerServerJsonFileFullName, request.ServerJson);
                }
                string location = NTMinerRegistry.GetLocation();
                if (IsNTMinerOpened()) {
                    using (HttpClient client = new HttpClient()) {
                        WorkRequest innerRequest = new WorkRequest {
                            LoginName = request.LoginName,
                            MessageId = request.MessageId,
                            WorkId = request.WorkId
                        };
                        innerRequest.SignIt(user.Password);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:{WebApiConst.MinerClientAppPort}/api/MinerClient/StartMine", innerRequest);
                        response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        return response;
                    }
                }
                else {
                    if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                        string arguments = "--AutoStart";
                        if (request.WorkId != Guid.Empty) {
                            arguments += " workid=" + request.WorkId.ToString();
                        }
                        Windows.Cmd.RunClose(location, arguments);
                        return ResponseBase.Ok(request.MessageId);
                    }
                    return ResponseBase.ServerError(request.MessageId, "挖矿端程序不存在");
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        [HttpPost]
        public ResponseBase StopMine([FromBody]SignatureRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                    return response;
                }

                if (!IsNTMinerOpened()) {
                    return ResponseBase.Ok(request.MessageId);
                }
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:{WebApiConst.MinerClientAppPort}/api/MinerClient/StopMine", request);
                        response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        return response;
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                }
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        [HttpPost]
        public ResponseBase RestartNTMiner([FromBody]WorkRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            ResponseBase response;
            if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                return response;
            }
            IUser user = HostRoot.Current.UserSet.GetUser(request.LoginName);
            if (user == null) {
                return ResponseBase.Forbidden(request.MessageId);
            }

            if (request.WorkId != Guid.Empty) {
                File.WriteAllText(SpecialPath.NTMinerLocalJsonFileFullName, request.LocalJson);
                File.WriteAllText(SpecialPath.NTMinerServerJsonFileFullName, request.ServerJson);
            }
            Task.Factory.StartNew(() => {
                try {
                    if (IsNTMinerOpened()) {
                        SignatureRequest innerRequest = new SignatureRequest {
                            LoginName = request.LoginName
                        };
                        innerRequest.SignIt(user.Password);
                        DoCloseNTMiner(innerRequest);
                        System.Threading.Thread.Sleep(1000);
                    }
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

        private void DoCloseNTMiner(SignatureRequest request) {
            bool isClosed = false;
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:{WebApiConst.MinerClientAppPort}/api/MinerClient/CloseNTMiner", request);
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
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{NTMinerRegistry.GetMinerServerHost()}:{WebApiConst.MinerServerPort}/api/Report/ReportState", new MinerServer.ReportState {
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
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            ResponseBase response;
            if (!request.IsValid(HostRoot.Current.UserSet.GetUser, out response)) {
                return response;
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

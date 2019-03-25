using NTMiner.Controllers;
using NTMiner.Daemon;
using NTMiner.JsonDb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Client {
        public class NTMinerDaemonServiceFace {
            public static readonly NTMinerDaemonServiceFace Instance = new NTMinerDaemonServiceFace();
            private static readonly string s_controllerName = ControllerUtil.GetControllerName<INTMinerDaemonController>();

            private NTMinerDaemonServiceFace() { }

            public void GetDaemonVersionAsync(Action<string, Exception> callback) {
                Process[] processes = Process.GetProcessesByName("NTMinerDaemon");
                if (processes.Length == 0) {
                    callback?.Invoke(string.Empty, null);
                }
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:{WebApiConst.NTMinerDaemonPort}/api/{s_controllerName}/{nameof(INTMinerDaemonController.GetDaemonVersion)}", null);
                            string response = message.Result.Content.ReadAsAsync<string>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        e = e.GetInnerException();
                        callback?.Invoke(string.Empty, e);
                    }
                });
            }

            public void CloseDaemon() {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:{WebApiConst.NTMinerDaemonPort}/api/{s_controllerName}/{nameof(INTMinerDaemonController.CloseDaemon)}", null);
                        Write.DevLine("CloseDaemon " + message.Result.ReasonPhrase);
                    }
                }
                catch (Exception e) {
                    e = e.GetInnerException();
                    Logger.ErrorDebugLine(e.Message, e);
                }
            }

            public void GetGpuProfilesJsonAsync(string clientHost, Action<GpuProfilesJsonDb, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            client.Timeout = TimeSpan.FromMilliseconds(3000);
                            Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:{WebApiConst.NTMinerDaemonPort}/api/{s_controllerName}/{nameof(INTMinerDaemonController.GetGpuProfilesJson)}", null);
                            string json = message.Result.Content.ReadAsAsync<string>().Result;
                            GpuProfilesJsonDb data = VirtualRoot.JsonSerializer.Deserialize<GpuProfilesJsonDb>(json);
                            callback?.Invoke(data, null);
                            return data;
                        }
                    }
                    catch (Exception e) {
                        e = e.GetInnerException();
                        callback?.Invoke(null, e);
                        return null;
                    }
                });
            }

            public void SaveGpuProfilesJsonAsync(string clientHost, string json) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            client.Timeout = TimeSpan.FromMilliseconds(3000);
                            HttpContent content = new StringContent(json);
                            Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:{WebApiConst.NTMinerDaemonPort}/api/{s_controllerName}/{nameof(INTMinerDaemonController.SaveGpuProfilesJson)}", content);
                            Write.DevLine(message.Result.ReasonPhrase);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                });
            }

            public void SetAutoBootStartAsync(string clientHost, bool autoBoot, bool autoStart) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            client.Timeout = TimeSpan.FromMilliseconds(3000);
                            Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:{WebApiConst.NTMinerDaemonPort}/api/{s_controllerName}/{nameof(INTMinerDaemonController.SetAutoBootStart)}?autoBoot={autoBoot}&autoStart={autoStart}", null);
                            Write.DevLine(message.Result.ReasonPhrase);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                });
            }

            public ResponseBase RestartWindows(string clientIp, SignatureRequest request) {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientIp}:{WebApiConst.NTMinerDaemonPort}/api/{s_controllerName}/{nameof(INTMinerDaemonController.RestartWindows)}", request);
                    ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public ResponseBase ShutdownWindows(string clientIp, SignatureRequest request) {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientIp}:{WebApiConst.NTMinerDaemonPort}/api/{s_controllerName}/{nameof(INTMinerDaemonController.ShutdownWindows)}", request);
                    ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public ResponseBase RestartNTMiner(string clientIp, WorkRequest request) {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientIp}:{WebApiConst.NTMinerDaemonPort}/api/{s_controllerName}/{nameof(INTMinerDaemonController.RestartNTMiner)}", request);
                    ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public ResponseBase UpgradeNTMiner(string clientIp, UpgradeNTMinerRequest request) {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientIp}:{WebApiConst.NTMinerDaemonPort}/api/{s_controllerName}/{nameof(INTMinerDaemonController.UpgradeNTMiner)}", request);
                    ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public ResponseBase StartMine(string clientIp, WorkRequest request) {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientIp}:{WebApiConst.NTMinerDaemonPort}/api/{s_controllerName}/{nameof(INTMinerDaemonController.StartMine)}", request);
                    ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public ResponseBase StopMine(string clientIp, SignatureRequest request) {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientIp}:{WebApiConst.NTMinerDaemonPort}/api/{s_controllerName}/{nameof(INTMinerDaemonController.StopMine)}", request);
                    ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public void StartNoDevFeeAsync(StartNoDevFeeRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:{WebApiConst.NTMinerDaemonPort}/api/{s_controllerName}/{nameof(INTMinerDaemonController.StartNoDevFee)}", request);
                            ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        e = e.GetInnerException();
                        callback?.Invoke(null, e);
                    }
                });
            }

            public void StopNoDevFeeAsync(Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            RequestBase request = new RequestBase();
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:{WebApiConst.NTMinerDaemonPort}/api/{s_controllerName}/{nameof(INTMinerDaemonController.StopNoDevFee)}", request);
                            ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        e = e.GetInnerException();
                        callback?.Invoke(null, e);
                    }
                });
            }
        }
    }
}

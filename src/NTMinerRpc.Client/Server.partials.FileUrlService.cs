using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public partial class FileUrlServiceFace {
            public static readonly FileUrlServiceFace Instance = new FileUrlServiceFace();
            private readonly string baseUrl = $"http://{MinerServerHost}:{MinerServerPort}/api/FileUrl";

            private FileUrlServiceFace() { }

            #region GetNTMinerUrlAsync
            public void GetNTMinerUrlAsync(string fileName, Action<string> callback) {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/NTMinerUrl", new { fileName });
                        string response = message.Result.Content.ReadAsStringAsync().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(string.Empty);
                }
            }
            #endregion

            #region GetNTMinerFilesAsync
            public void GetNTMinerFilesAsync(Action<List<NTMinerFileData>> callback) {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"{baseUrl}/NTMinerFiles", null);
                        List<NTMinerFileData> response = message.Result.Content.ReadAsAsync<List<NTMinerFileData>>().Result;
                        callback?.Invoke(response ?? new List<NTMinerFileData>());
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(new List<NTMinerFileData>());
                }
            }
            #endregion

            #region AddOrUpdateNTMinerFileAsync
            public void AddOrUpdateNTMinerFileAsync(NTMinerFileData entity, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        AddOrUpdateNTMinerFileRequest request = new AddOrUpdateNTMinerFileRequest() {
                            MessageId = messageId,
                            Data = entity,
                            LoginName = LoginName,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/AddOrUpdateNTMinerFile", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<ResponseBase>(messageId, e.Message));
                }
            }
            #endregion

            #region RemoveNTMinerFileAsync
            public void RemoveNTMinerFileAsync(Guid id, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        RemoveNTMinerFileRequest request = new RemoveNTMinerFileRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            NTMinerId = id,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/RemoveNTMinerFile", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError<ResponseBase>(messageId, e.Message));
                }
            }
            #endregion

            #region GetLiteDBExplorerUrlAsync
            public void GetLiteDBExplorerUrlAsync(Action<string> callback) {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"{baseUrl}/LiteDBExplorerUrl", null);
                        string response = message.Result.Content.ReadAsStringAsync().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(string.Empty);
                }
            }
            #endregion

            #region GetNTMinerUpdaterUrlAsync
            public void GetNTMinerUpdaterUrlAsync(Action<string> callback) {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"{baseUrl}/NTMinerUpdaterUrl", null);
                        string response = message.Result.Content.ReadAsStringAsync().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(string.Empty);
                }
            }
            #endregion

            #region GetPackageUrlAsync
            public void GetPackageUrlAsync(string package, Action<string> callback) {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"{baseUrl}/PackageUrl", null);
                        string response = message.Result.Content.ReadAsStringAsync().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(string.Empty);
                }
            }
            #endregion
        }
    }
}
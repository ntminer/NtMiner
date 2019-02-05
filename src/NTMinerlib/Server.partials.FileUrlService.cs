using NTMiner.ServiceContracts;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public partial class FileUrlServiceFace {
            public static readonly FileUrlServiceFace Instance = new FileUrlServiceFace();

            private FileUrlServiceFace() { }

            private IFileUrlService CreateService() {
                return ChannelFactory.CreateChannel<IFileUrlService>(MinerServerHost, MinerServerPort);
            }

            #region GetNTMinerUrlAsync
            public void GetNTMinerUrlAsync(string fileName, Action<string> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var client = CreateService()) {
                            string url = client.GetNTMinerUrl(fileName);
                            callback?.Invoke(url);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.WriteDevLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(string.Empty);
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(string.Empty);
                    }
                });
            }
            #endregion

            #region GetNTMinerFilesAsync
            public void GetNTMinerFilesAsync(Action<List<NTMinerFileData>> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var client = CreateService()) {
                            List<NTMinerFileData> list = client.GetNTMinerFiles() ?? new List<NTMinerFileData>();
                            callback?.Invoke(list);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.WriteDevLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(new List<NTMinerFileData>());
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(new List<NTMinerFileData>());
                    }
                });
            }
            #endregion

            #region AddOrUpdateNTMinerFileAsync
            public void AddOrUpdateNTMinerFileAsync(NTMinerFileData entity, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
                        AddOrUpdateNTMinerFileRequest request = new AddOrUpdateNTMinerFileRequest() {
                            MessageId = messageId,
                            Data = entity,
                            LoginName = LoginName,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        using (var service = CreateService()) {
                            ResponseBase response = service.AddOrUpdateNTMinerFile(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.WriteDevLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }
            #endregion

            #region RemoveNTMinerFileAsync
            public void RemoveNTMinerFileAsync(Guid id, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
                        RemoveNTMinerFileRequest request = new RemoveNTMinerFileRequest {
                            MessageId = messageId,
                            LoginName = LoginName,
                            NTMinerId = id,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        using (var service = CreateService()) {
                            ResponseBase response = service.RemoveNTMinerFile(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.WriteDevLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }
            #endregion

            #region GetLiteDBExplorerUrlAsync
            public void GetLiteDBExplorerUrlAsync(Action<string> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var service = CreateService()) {
                            callback?.Invoke(service.GetLiteDBExplorerUrl());
                        }
                    }
                    catch (CommunicationException e) {
                        Global.WriteDevLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(string.Empty);
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(string.Empty);
                    }
                });
            }
            #endregion

            #region GetNTMinerUpdaterUrlAsync
            public void GetNTMinerUpdaterUrlAsync(Action<string> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var service = CreateService()) {
                            string downloadUrl = service.GetNTMinerUpdaterUrl();
                            callback?.Invoke(downloadUrl);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.WriteDevLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(string.Empty);
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(string.Empty);
                    }
                });
            }
            #endregion

            #region GetPackageUrlAsync
            public void GetPackageUrlAsync(string package, Action<string> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var service = CreateService()) {
                            callback?.Invoke(service.GetPackageUrl(package));
                        }
                    }
                    catch (CommunicationException e) {
                        Global.WriteDevLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(string.Empty);
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(string.Empty);
                    }
                });
            }
            #endregion
        }
    }
}
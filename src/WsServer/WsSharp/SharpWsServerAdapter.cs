using NTMiner.Core;
using NTMiner.User;
using System;
using System.IO;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace NTMiner.WsSharp {
    public class SharpWsServerAdapter : IWsServerAdapter {
        private readonly WebSocketServer _wsServer = null;

        public IWsSessionsAdapter MinerClientWsSessions { get; private set; }
        public IWsSessionsAdapter MinerStudioWsSessions { get; private set; }

        public SharpWsServerAdapter(IHostConfig config) {
            _wsServer = new WebSocketServer($"ws://0.0.0.0:{config.GetServerPort().ToString()}") {
                KeepClean = true,
                AuthenticationSchemes = AuthenticationSchemes.Basic, // 用基本验证字段承载签名信息，因为传输的并非原始密码，所以虽然是基本验证但不存在安全问题
                UserCredentialsFinder = id => {
                    string base64String = id.Name;
                    if (!AppRoot.TryGetUser(base64String, out WsUserName wsUserName, out UserData userData, out string _)) {
                        return null;
                    }
                    var password = HashUtil.Sha1(base64String + AppRoot.GetUserPassword(wsUserName, userData));
                    // 经验证username对于基本验证来说没有用
                    return new NetworkCredential(userData.LoginName, password, domain: null, roles: wsUserName.ClientType.GetName());
                }
            };
            //_wsServer.Log.Level = WebSocketSharp.LogLevel.Debug;
            _wsServer.Log.File = TempPath.WebSocketSharpMinerStudioLogFileFullName;
            _wsServer.Log.Output = (data, path) => {
                Console.WriteLine(data.Message);
                if (path != null && path.Length > 0) {
                    using (var writer = new StreamWriter(path, true))
                    using (var syncWriter = TextWriter.Synchronized(writer)) {
                        syncWriter.WriteLine(data.ToString());
                    }
                }
            };
            _wsServer.AddWebSocketService<MinerClientSharpWsSessionAdapter>(AppRoot.MinerClientPath);
            _wsServer.AddWebSocketService<MinerStudioSharpWsSessionAdapter>(AppRoot.MinerStudioPath);
            MinerClientWsSessions = new SharpWsSessionsAdapter(_wsServer.WebSocketServices[AppRoot.MinerClientPath].Sessions);
            MinerStudioWsSessions = new SharpWsSessionsAdapter(_wsServer.WebSocketServices[AppRoot.MinerStudioPath].Sessions);
        }

        public bool Start() {
            try {
                _wsServer?.Start();
                return true;
            }
            catch (Exception e) {
                NTMinerConsole.UserError($"{e.Message} {e.StackTrace}");
                return false;
            }
        }

        public void Stop() {
            _wsServer?.Stop();
        }
    }
}

using NTMiner.Core;
using NTMiner.User;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace NTMiner.WsSharp {
    public class SharpWsServer : IWsServer {
        private readonly WebSocketServer _wsServer = null;

        public IWsSessionsAdapter MinerClientWsSessionsAdapter { get; private set; } = EmptyWsSessionsAdapter.Instance;
        public IWsSessionsAdapter MinerStudioWsSessionsAdapter { get; private set; } = EmptyWsSessionsAdapter.Instance;

        public SharpWsServer(IHostConfig config) {
            _wsServer = new WebSocketServer($"ws://0.0.0.0:{config.GetServerPort().ToString()}") {
                KeepClean = true,
                AuthenticationSchemes = AuthenticationSchemes.Basic, // 用基本验证字段承载签名信息，因为传输的并非原始密码，所以虽然是基本验证但不存在安全问题
                UserCredentialsFinder = id => {
                    string base64String = id.Name;
                    if (!WsRoot.TryGetUser(base64String, out WsUserName wsUserName, out UserData userData, out string _)) {
                        return null;
                    }
                    string password = WsCommonService.GetUserPassword(wsUserName, userData);
                    password = HashUtil.Sha1(base64String + password);
                    // 经验证username对于基本验证来说没有用
                    return new NetworkCredential(userData.LoginName, password, domain: null, roles: wsUserName.ClientType.GetName());
                }
            };
            _wsServer.AddWebSocketService<MinerClientSharpWsSessionAdapter>(WsRoot.MinerClientPath);
            _wsServer.AddWebSocketService<MinerStudioSharpWsSessionAdapter>(WsRoot.MinerStudioPath);
            MinerClientWsSessionsAdapter = new SharpWsSessionsAdapter(_wsServer.WebSocketServices[WsRoot.MinerClientPath].Sessions);
            MinerStudioWsSessionsAdapter = new SharpWsSessionsAdapter(_wsServer.WebSocketServices[WsRoot.MinerStudioPath].Sessions);
        }

        public bool Start() {
            _wsServer?.Start();
            return true;
        }

        public void Stop() {
            _wsServer?.Stop();
        }
    }
}

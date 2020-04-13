using NTMiner.Core;
using NTMiner.Core.Impl;
using NTMiner.Core.Mq.MqMessagePaths;
using NTMiner.Core.Mq.Senders;
using NTMiner.Core.Mq.Senders.Impl;
using NTMiner.Core.Redis.Impl;
using NTMiner.User;
using System;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace NTMiner {
    public static class WsRoot {
        public static IMinerClientSessionSet MinerClientSessionSet { get; private set; }
        public static IMinerStudioSessionSet MinerStudioSessionSet { get; private set; }
        public static IReadOnlyUserSet ReadOnlyUserSet { get; private set; }
        public static IMinerSignSet MinerSignSet { get; private set; }
        public static IWsServerNodeAddressSet WsServerNodeAddressSet { get; private set; }
        public static IMinerClientMqSender MinerClientMqSender { get; private set; }
        public static IOperationMqSender OperationMqSender { get; private set; }
        public static IUserMqSender UserMqSender { get; private set; }

        public static WebSocketServer WebSocketServer { get; private set; } = 
            new WebSocketServer($"ws://0.0.0.0:{ServerRoot.HostConfig.GetServerPort().ToString()}") {
                KeepClean = false, // 由客户端周期ping，服务端禁用ping
                AuthenticationSchemes = AuthenticationSchemes.Basic, // 用基本验证字段承载签名信息，因为传输的并非原始密码，所以虽然是基本验证但不存在安全问题
                UserCredentialsFinder = id => {
                    var base64String = id.Name;
                    if (string.IsNullOrEmpty(base64String)) {
                        return null;
                    }
                    string json = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));
                    WsUserName wsUserName = VirtualRoot.JsonSerializer.Deserialize<WsUserName>(json);
                    if (wsUserName == null) {
                        return null;
                    }
                    UserData userData = ReadOnlyUserSet.GetUser(UserId.Create(wsUserName.UserId));
                    if (userData == null || !userData.IsEnabled) {
                        return null;
                    }
                    string password;
                    switch (wsUserName.ClientType) {
                        case NTMinerAppType.MinerClient:
                            password = string.Empty;
                            break;
                        case NTMinerAppType.MinerStudio:
                            password = userData.Password;
                            break;
                        default:
                            password = string.Empty;
                            break;
                    }
                    password = HashUtil.Sha1(base64String + password);
                    // 经验证username对于基本验证来说没有用
                    return new NetworkCredential(userData.LoginName, password, domain: null, roles: wsUserName.ClientType.GetName());
                }
            };

        private static IServerContext _serverContext;
        static void Main() {
            HomePath.SetHomeDirFullName(AppDomain.CurrentDomain.BaseDirectory);
            NTMinerConsole.DisbleQuickEditMode();
            DevMode.SetDevMode();

            Windows.ConsoleHandler.Register(Exit);

            MinerClientSessionSet = new MinerClientSessionSet();
            MinerStudioSessionSet = new MinerStudioSessionSet();
            WsServerNodeAddressSet = new WsServerNodeAddressSet();

            Console.Title = $"{ServerAppType.WsServer.GetName()}_{ServerRoot.HostConfig.ThisServerAddress}";

            // 用本节点的地址作为队列名，消费消息时根据路由键区分消息类型
            string queue = $"{ServerAppType.WsServer.GetName()}.{ServerRoot.HostConfig.ThisServerAddress}";
            string durableQueue = queue + MqKeyword.DurableQueueEndsWith;
            AbstractMqMessagePath[] mqMessagePaths = new AbstractMqMessagePath[] {
                new ReadOnlyUserMqMessagePath(durableQueue),
                new MinerSignMqMessagePath(durableQueue), 
                new WsServerNodeMqMessagePath(queue), 
                new OperationMqMessagePath(queue)
            };
            _serverContext = ServerContext.Create(mqClientTypeName: ServerAppType.WsServer.GetName(), mqMessagePaths);
            if (_serverContext == null) {
                Write.UserError("启动失败，无法继续，因为服务器上下文创建失败");
                return;
            }
            MinerClientMqSender = new MinerClientMqSender(_serverContext.Channel);
            OperationMqSender = new OperationMqSender(_serverContext.Channel);
            UserMqSender = new UserMqSender(_serverContext.Channel);
            var minerRedis = new ReadOnlyMinerRedis(_serverContext.RedisConn);
            var userRedis = new ReadOnlyUserRedis(_serverContext.RedisConn);
            VirtualRoot.StartTimer();
            RpcRoot.SetRpcUser(new RpcUser(ServerRoot.HostConfig.RpcLoginName, HashUtil.Sha1(ServerRoot.HostConfig.RpcPassword)), isOuterNet: false);
            // 构造函数中异步访问redis初始化用户列表，因为是异步的所以提前构造
            ReadOnlyUserSet = new ReadOnlyUserSet(userRedis);
            MinerSignSet = new MinerSignSet(minerRedis);
            WebSocketServer.Log.Level = LogLevel.Trace;
            WebSocketServer.AddWebSocketService<MinerStudioBehavior>(MinerStudioBehavior.WsServiceHostPath);
            WebSocketServer.AddWebSocketService<MinerClientBehavior>(MinerClientBehavior.WsServiceHostPath);
            WebSocketServer.Start();
            VirtualRoot.RaiseEvent(new WebSocketServerStatedEvent());

            Console.ReadKey(true);
            Exit();
        }

        private static void Exit() {
            if (_serverContext != null) {
                RpcRoot.OfficialServer.WsServerNodeService.RemoveNodeAsync(ServerRoot.HostConfig.ThisServerAddress, callback: null);
                // 等待1秒以使ReportNodeAsync过程完成
                System.Threading.Thread.Sleep(1000);
            }
            WebSocketServer?.Stop();
        }
    }
}

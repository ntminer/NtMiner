using NTMiner.Core;
using NTMiner.Core.Impl;
using NTMiner.Core.Mq.MqMessagePaths;
using NTMiner.Core.Mq.Senders;
using NTMiner.Core.Mq.Senders.Impl;
using NTMiner.Core.Redis;
using NTMiner.Core.Redis.Impl;
using NTMiner.User;
using NTMiner.WsSharp;
using System;
using System.Text;

namespace NTMiner {
    public static class WsRoot {
        public const string MinerClientPath = "/" + nameof(NTMinerAppType.MinerClient);
        public const string MinerStudioPath = "/" + nameof(NTMinerAppType.MinerStudio);

        private static IWsServer _wsServer = null;
        public static IWsServer WsServer {
            get {
                return _wsServer;
            }
        }

        public static IMinerClientSessionSet MinerClientSessionSet { get; private set; }
        public static IMinerStudioSessionSet MinerStudioSessionSet { get; private set; }
        public static IReadOnlyUserSet ReadOnlyUserSet { get; private set; }
        public static IMinerSignSet MinerSignSet { get; private set; }
        public static IWsServerNodeAddressSet WsServerNodeAddressSet { get; private set; }
        public static ISpeedDataRedis SpeedDataRedis { get; private set; }
        public static IMinerClientMqSender MinerClientMqSender { get; private set; }
        public static IOperationMqSender OperationMqSender { get; private set; }
        public static IUserMqSender UserMqSender { get; private set; }
        public static IWsServerNodeRedis WsServerNodeRedis { get; private set; }

        private static IWsServerNodeMqSender _wsServerNodeMqSender;
        private static bool _started = false;
        static void Main() {
            NTMinerConsole.SetIsMainUiOk(true);
            NTMinerConsole.DisbleQuickEditMode();
            DevMode.SetDevMode();

            Windows.ConsoleHandler.Register(Exit);

            Console.Title = $"{ServerAppType.WsServer.GetName()}_{ServerRoot.HostConfig.ThisServerAddress}";
            // 通过WsServer的网络缓解对WebApiServer的外网流量的压力。WsServer调用WebApiServer的时候走内网调用节省外网带宽
            RpcRoot.SetOfficialServerAddress(ServerRoot.HostConfig.RpcServerLocalAddress);
            // 用本节点的地址作为队列名，消费消息时根据路由键区分消息类型
            string queue = $"{ServerAppType.WsServer.GetName()}.{ServerRoot.HostConfig.ThisServerAddress}";
            string durableQueue = queue + MqKeyword.DurableQueueEndsWith;
            AbstractMqMessagePath[] mqMessagePaths = new AbstractMqMessagePath[] {
                new ReadOnlyUserMqMessagePath(durableQueue),
                new MinerSignMqMessagePath(durableQueue),
                new WsServerNodeMqMessagePath(queue),
                new OperationMqMessagePath(queue)
            };
            if (!ServerConnection.Create(ServerAppType.WsServer, mqMessagePaths, out IServerConnection serverConfig)) {
                NTMinerConsole.UserError("启动失败，无法继续，因为服务器上下文创建失败");
                return;
            }
            MinerClientMqSender = new MinerClientMqSender(serverConfig);
            SpeedDataRedis = new SpeedDataRedis(serverConfig);
            WsServerNodeRedis = new WsServerNodeRedis(serverConfig);
            OperationMqSender = new OperationMqSender(serverConfig);
            UserMqSender = new UserMqSender(serverConfig);
            _wsServerNodeMqSender = new WsServerNodeMqSender(serverConfig);
            WsServerNodeAddressSet = new WsServerNodeAddressSet(WsServerNodeRedis, _wsServerNodeMqSender);
            var minerRedis = new ReadOnlyMinerRedis(serverConfig);
            var userRedis = new ReadOnlyUserRedis(serverConfig);
            VirtualRoot.StartTimer();
            RpcRoot.SetRpcUser(new RpcUser(ServerRoot.HostConfig.RpcLoginName, HashUtil.Sha1(ServerRoot.HostConfig.RpcPassword)));
            RpcRoot.SetIsOuterNet(false);
            // 构造函数中异步访问redis初始化用户列表，因为是异步的所以提前构造
            ReadOnlyUserSet = new ReadOnlyUserSet(userRedis);
            MinerSignSet = new MinerSignSet(minerRedis);
            _wsServer = new SharpWsServer(ServerRoot.HostConfig);
            MinerClientSessionSet = new MinerClientSessionSet(_wsServer.MinerClientWsSessionsAdapter);
            MinerStudioSessionSet = new MinerStudioSessionSet(_wsServer.MinerStudioWsSessionsAdapter);
            _wsServer.Start();
            VirtualRoot.RaiseEvent(new WebSocketServerStatedEvent());
            _started = true;

            Console.ReadKey(true);
            Exit();
        }

        public static bool TryGetUser(string base64String, out WsUserName wsUserName, out UserData userData, out string errMsg) {
            wsUserName = null;
            userData = null;
            errMsg = string.Empty;
            if (string.IsNullOrEmpty(base64String)) {
                errMsg = $"{nameof(base64String)}为空";
                return false;
            }
            string json = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));
            wsUserName = VirtualRoot.JsonSerializer.Deserialize<WsUserName>(json);
            if (wsUserName == null) {
                errMsg = $"{nameof(base64String)}编码的json字符串格式错误";
                return false;
            }
            if (!wsUserName.IsValid()) {
                errMsg = "wsUserName提交的数据无效";
                return false;
            }
            userData = ReadOnlyUserSet.GetUser(UserId.Create(wsUserName.UserId));
            if (userData == null) {
                errMsg = $"用户{wsUserName.UserId}不存在";
                return false;
            }
            if (!userData.IsEnabled) {
                errMsg = $"用户{wsUserName.UserId}已被禁用";
                return false;
            }
            return true;
        }

        private static void Exit() {
            if (_started) {
                try {
                    WsServerNodeRedis.DeleteByAddressAsync(ServerRoot.HostConfig.ThisServerAddress).ContinueWith(_ => {
                        _wsServerNodeMqSender.SendWsServerNodeRemoved(ServerRoot.HostConfig.ThisServerAddress);
                    }).Wait();
                }
                catch {
                }
            }
            _wsServer?.Stop();
        }
    }
}

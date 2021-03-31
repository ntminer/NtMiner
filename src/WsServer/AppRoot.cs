using NTMiner.Core;
using NTMiner.Core.Impl;
using NTMiner.Core.MinerServer;
using NTMiner.Core.Mq.MqMessagePaths;
using NTMiner.Core.Mq.Senders;
using NTMiner.Core.Mq.Senders.Impl;
using NTMiner.Core.Redis;
using NTMiner.Core.Redis.Impl;
using NTMiner.User;
using NTMiner.Ws;
using NTMiner.WsSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NTMiner {
    public static class AppRoot {
        public const string MinerClientPath = "/" + nameof(NTMinerAppType.MinerClient);
        public const string MinerStudioPath = "/" + nameof(NTMinerAppType.MinerStudio);

        private static IWsServerAdapter _wsServer = null;
        public static IWsServerAdapter WsServer {
            get {
                return _wsServer;
            }
        }

        public static IMinerClientSessionSet MinerClientSessionSet { get; private set; }
        public static IMinerStudioSessionSet MinerStudioSessionSet { get; private set; }
        public static IReadOnlyUserSet UserSet { get; private set; }
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
            VirtualRoot.SetOut(new ConsoleOut());
            NTMinerConsole.MainUiOk();
            NTMinerConsole.DisbleQuickEditMode();
            DevMode.SetDevMode();

            Windows.ConsoleHandler.Register(Exit);

            string thisServerAddress = ServerRoot.HostConfig.ThisServerAddress;
            Console.Title = $"{ServerAppType.WsServer.GetName()}_{thisServerAddress}";
            // 用本节点的地址作为队列名，消费消息时根据路由键区分消息类型
            string queue = $"{ServerAppType.WsServer.GetName()}.{thisServerAddress}";
            string durableQueue = queue + MqKeyword.DurableQueueEndsWith;
            AbstractMqMessagePath[] mqMessagePaths = new AbstractMqMessagePath[] {
                new ReadOnlyUserMqMessagePath(durableQueue),
                new MinerSignMqMessagePath(durableQueue),
                new WsServerNodeMqMessagePath(queue),
                new OperationMqMessagePath(queue),
                new MinerClientMqMessagePath(queue, thisServerAddress)
            };
            if (!MqRedis.Create(ServerAppType.WsServer, mqMessagePaths, out IMqRedis mqRedis)) {
                NTMinerConsole.UserError("启动失败，无法继续，因为服务器上下文创建失败");
                return;
            }
            MinerClientMqSender = new MinerClientMqSender(mqRedis);
            SpeedDataRedis = new SpeedDataRedis(mqRedis);
            WsServerNodeRedis = new WsServerNodeRedis(mqRedis);
            OperationMqSender = new OperationMqSender(mqRedis);
            UserMqSender = new UserMqSender(mqRedis);
            _wsServerNodeMqSender = new WsServerNodeMqSender(mqRedis);
            WsServerNodeAddressSet = new WsServerNodeAddressSet(WsServerNodeRedis, _wsServerNodeMqSender);
            var minerRedis = new ReadOnlyMinerDataRedis(mqRedis);
            var userRedis = new ReadOnlyUserDataRedis(mqRedis);
            VirtualRoot.StartTimer();
            // 构造函数中异步访问redis初始化用户列表，因为是异步的所以提前构造
            UserSet = new ReadOnlyUserSet(userRedis);
            MinerSignSet = new MinerSignSet(minerRedis);
            _wsServer = new SharpWsServerAdapter(ServerRoot.HostConfig);
            MinerClientSessionSet = new MinerClientSessionSet(_wsServer.MinerClientWsSessions);
            MinerStudioSessionSet = new MinerStudioSessionSet(_wsServer.MinerStudioWsSessions);
            _started = _wsServer.Start();
            if (!_started) {
                NTMinerConsole.UserError("启动失败，无法继续，因为_wsServer启动失败");
                return;
            }
            VirtualRoot.RaiseEvent(new WebSocketServerStatedEvent());
            MinerClientsBreatherInit();

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
            userData = UserSet.GetUser(UserId.Create(wsUserName.UserId));
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
                        _wsServerNodeMqSender.SendWsServerNodeRemoved();
                    }).Wait();
                }
                catch {
                }
            }
            _wsServer?.Stop();
        }

        public static string GetUserPassword(WsUserName wsUserName, UserData userData) {
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
            return password;
        }

        public static string GetSignPassword(this ISession session) {
            string password = null;
            switch (session.WsUserName.ClientType) {
                case NTMinerAppType.MinerClient:
                    password = ((IMinerClientSession)session).GetSignPassword();
                    break;
                case NTMinerAppType.MinerStudio:
                    var userData = UserSet.GetUser(UserId.CreateLoginNameUserId(session.LoginName));
                    if (userData != null) {
                        password = userData.Password;
                    }
                    break;
                default:
                    break;
            }
            return password;
        }

        public static void ActiveMinerStudioSession(string sessionId) {
            MinerStudioSessionSet.ActiveByWsSessionId(sessionId, out _);
        }

        private static readonly List<Guid> _toBreathClientIds = new List<Guid>();
        private static readonly object _lockerForToBreathClientIds = new object();
        private static void MinerClientsBreatherInit() {
            // 这样做以消减WebApiServer收到的Mq消息的数量，能消减90%以上，降低CPU使用率
            VirtualRoot.BuildEventPath<Per1SecondEvent>("每1秒钟将WsServer暂存的来自挖矿端的呼吸通过Mq发送给WebApiServer", LogEnum.None, message => {
                Guid[] clientIds;
                lock (_lockerForToBreathClientIds) {
                    clientIds = _toBreathClientIds.ToArray();
                    _toBreathClientIds.Clear();
                }
                MinerClientMqSender.SendMinerClientsWsBreathed(clientIds);
            }, typeof(AppRoot));
        }

        public static void ActiveMinerClientSession(string sessionId) {
            if (MinerClientSessionSet.ActiveByWsSessionId(sessionId, out IMinerClientSession minerSession)) {
                lock (_lockerForToBreathClientIds) {
                    _toBreathClientIds.Add(minerSession.ClientId);
                }
            }
        }

        public static void AddMinerStudioSession(WsUserName wsUserName, UserData userData, IPEndPoint remoteEndPoint, IWsSessionAdapter session) {
            IMinerStudioSession minerSession = MinerStudioSession.Create(userData, wsUserName, remoteEndPoint, session.SessionId, WsServer.MinerStudioWsSessions);
            MinerStudioSessionSet.Add(minerSession);
            session.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.ServerTime) {
                Data = Timestamp.GetTimestamp()
            }.SignToBytes(userData.Password));
        }

        public static void AddMinerClientSession(WsUserName wsUserName, UserData userData, IPEndPoint remoteEndPoint, IWsSessionAdapter session) {
            IMinerClientSession minerSession = MinerClientSession.Create(userData, wsUserName, remoteEndPoint, session.SessionId, WsServer.MinerClientWsSessions);
            MinerClientSessionSet.Add(minerSession);
            // 通知WebApiServer节点该矿机Ws连线了
            MinerClientMqSender.SendMinerClientWsOpened(minerSession.ClientId);
            bool isMinerSignChanged;
            if (!MinerSignSet.TryGetByClientId(wsUserName.ClientId, out MinerSign minerSign)) {
                // 此时该矿机是第一次在服务端出现
                minerSign = new MinerSign {
                    Id = LiteDB.ObjectId.NewObjectId().ToString(),
                    ClientId = wsUserName.ClientId,
                    LoginName = userData.LoginName,
                    OuterUserId = wsUserName.UserId,
                    AESPassword = string.Empty,
                    AESPasswordOn = Timestamp.UnixBaseTime
                };
                isMinerSignChanged = true;
            }
            else {
                isMinerSignChanged = minerSign.OuterUserId != wsUserName.UserId || minerSign.LoginName != userData.LoginName;
                if (isMinerSignChanged) {
                    minerSign.OuterUserId = wsUserName.UserId;
                    minerSign.LoginName = userData.LoginName;
                }
            }
            // 通常执行不到，因为用户注册的时候已经生成了RSA公私钥对了
            if (string.IsNullOrEmpty(userData.PublicKey) || string.IsNullOrEmpty(userData.PrivateKey)) {
                var key = Cryptography.RSAUtil.GetRASKey();
                userData.PublicKey = key.PublicKey;
                userData.PrivateKey = key.PrivateKey;
                UserMqSender.SendUpdateUserRSAKey(userData.LoginName, key);
            }
            DateTime now = DateTime.Now;
            if (string.IsNullOrEmpty(minerSign.AESPassword) || minerSign.AESPasswordOn.AddDays(1) < now) {
                isMinerSignChanged = true;
                minerSign.AESPassword = Cryptography.AESUtil.GetRandomPassword();
                minerSign.AESPasswordOn = now;
            }
            session.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.UpdateAESPassword) {
                Data = new AESPassword {
                    PublicKey = userData.PublicKey,
                    Password = Cryptography.RSAUtil.EncryptString(minerSign.AESPassword, userData.PrivateKey)
                }
            }.SignToJson(minerSign.AESPassword));
            if (isMinerSignChanged) {
                // TODO:这个可以直接写redis然后发MinerSignChangedMqMessage，没必要发给WebApiServer
                // 改的时候记得WebApiServer那订阅MinerSignChangedMqMessage
                MinerClientMqSender.SendChangeMinerSign(minerSign);
            }
        }

        public static void RemoveMinerStudioSession(string sessionId) {
            MinerStudioSessionSet.RemoveByWsSessionId(sessionId);
        }

        public static void RemoveMinerClientSession(string sessionId) {
            IMinerClientSession minerSession = MinerClientSessionSet.RemoveByWsSessionId(sessionId);
            if (minerSession != null) {
                MinerClientMqSender.SendMinerClientWsClosed(minerSession.ClientId);
            }
        }

        public static void HandleMinerStudioMessage(IWsSessionAdapter session, WsMessage message) {
            if (message == null) {
                return;
            }
            if (!MinerStudioSessionSet.TryGetByWsSessionId(session.SessionId, out IMinerStudioSession minerSession)) {
                session.CloseAsync(WsCloseCode.Normal, "意外，会话不存在，请重新连接");
                return;
            }
            if (!minerSession.IsValid(message)) {
                session.CloseAsync(WsCloseCode.Normal, "意外，签名验证失败，请重新连接");
                return;
            }
            if (WsMessageFromMinerStudioHandler.TryGetHandler(message.Type, out Action<IMinerStudioSession, WsMessage> handler)) {
                try {
                    handler.Invoke(minerSession, message);
                }
                catch (Exception ex) {
                    Logger.ErrorDebugLine(ex);
                }
            }
            else {
                NTMinerConsole.UserWarn($"{nameof(HandleMinerStudioMessage)} Received InvalidType {message.Type}");
            }
        }

        public static void HandleMinerClientMessage(IWsSessionAdapter session, WsMessage message) {
            if (message == null) {
                return;
            }
            if (!MinerClientSessionSet.TryGetByWsSessionId(session.SessionId, out IMinerClientSession minerSession)) {
                session.CloseAsync(WsCloseCode.Normal, "意外，会话不存在，请重新连接");
                return;
            }
            else if (WsMessageFromMinerClientHandler.TryGetHandler(message.Type, out Action<IMinerClientSession, Guid, WsMessage> handler)) {
                try {
                    handler.Invoke(minerSession, minerSession.ClientId, message);
                }
                catch (Exception ex) {
                    Logger.ErrorDebugLine(ex);
                }
            }
            else {
                NTMinerConsole.UserWarn($"{nameof(HandleMinerClientMessage)} Received InvalidType {message.Type}");
            }
        }

        public static T ParseWsMessage<T>(string json) {
            if (string.IsNullOrEmpty(json) || json[0] != '{' || json[json.Length - 1] != '}') {
                return default;
            }
            return VirtualRoot.JsonSerializer.Deserialize<T>(json);
        }
    }
}

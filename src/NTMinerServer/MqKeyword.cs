using NTMiner.Ws;

namespace NTMiner {
    public static class MqKeyword {
        public const string SafeIgnoreMessage = "该消息发生的时间早于本节点启动时间1分钟，安全忽略";
        public const string DurableQueueEndsWith = ".durable";
        public const string WsBreathQueueEndsWith = ".breath";
        public const string Expiration36sec = "36000000"; // 36秒，单位是微秒（1微秒是10的负6次方秒）
        public const string Expiration60sec = "60000000";

        public const string NTMinerExchange = "ntminer";

        public const string ClientIdHeaderName = "clientId";
        public const string LoginNameHeaderName = "loginName";
        public const string StudioIdHeaderName = "studioId";
        public const string SessionIdHeaderName = "sessionId";
        public const string MinerIpHeaderName = "minerIp";

        public const string MqCountRoutingKey = "MqCount";
        public const string UserAddedRoutingKey = "UserAdded";
        public const string UserUpdatedRoutingKey = "UserUpdated";
        public const string UserRemovedRoutingKey = "UserRemoved";
        public const string UserEnabledRoutingKey = "UserEnabled";
        public const string UserDisabledRoutingKey = "UserDisabled";
        public const string UserPasswordChangedRoutingKey = "UserPasswordChanged";
        public const string UpdateUserRSAKeyRoutingKey = "UpdateUserRSAKey";
        public const string UserRSAKeyUpdatedRoutingKey = "UserRSAKeyUpdated";

        public const string WsServerNodeAddedRoutingKey = "WsServerNodeAdded";
        public const string WsServerNodeRemovedRoutingKey = "WsServerNodeRemoved";

        public const string MinerClientWsClosedRoutingKey = "MinerClientWsClosed";
        public const string MinerClientsWsBreathedRoutingKey = "MinerClientsWsBreathed";

        public const string MinerDataRemovedRoutingKey = "MinerDataRemoved";
        public const string MinerDatasRemovedRoutingKey = "MinerDatasRemoved";
        public const string MinerSignsSetedRoutingKey = "MinerSignsSeted";

        public const string QueryClientsForWsRoutingKey = "QueryClientsForWs";
        public const string AutoQueryClientsForWsRoutingKey = "AutoQueryClientsForWs";
        public const string QueryClientsForWsResponseRoutingKey = "QueryClientsForWsResponse{0}";
        public const string AutoQueryClientsForWsResponseRoutingKey = "AutoQueryClientsForWsResponse{0}";
        public const string SpeedsRoutingKey = WsMessage.Speed + "s";

        public const string StartMineRoutingKey = WsMessage.StartMine;
        public const string StartWorkMineRoutingKey = WsMessage.StartWorkMine;

        public const string RefreshMinerTestIdRoutingKey = "RefreshMinerTestId";
        public const string CalcConfigsUpdatedRoutingKey = "CalcConfigsUpdated";
    }
}

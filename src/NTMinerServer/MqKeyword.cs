using NTMiner.Ws;

namespace NTMiner {
    public static class MqKeyword {
        public const string SafeIgnoreMessage = "该消息发生的时间早于本节点启动时间1分钟，安全忽略";
        public const string DurableQueueEndsWith = ".durable";
        public const string Expiration36sec = "36000000"; // 36秒，单位是微秒（1微秒是10的负6次方秒）
        public const string Expiration60sec = "60000000";

        public const string NTMinerExchange = "ntminer";

        public const string ClientIdHeaderName = "clientId";
        public const string LoginNameHeaderName = "loginName";
        public const string SessionIdHeaderName = "sessionId";
        public const string MinerIpHeaderName = "minerIp";

        // TODO:因为一个节点只对应两个队列，一个不持久队列和一个持久队列，消息通过路由键分类。
        // 通过Mq的管理后台是看不出每个路由键的消息速率的，所以考虑统计一下每个路由键消息的速率。
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

        public const string MinerClientWsOpenedRoutingKey = "MinerClientWsOpened";
        public const string MinerClientWsClosedRoutingKey = "MinerClientWsClosed";
        public const string MinerClientWsBreathedRoutingKey = "MinerClientWsBreathed";

        public const string MinerDataAddedRoutingKey = "MinerDataAdded";
        public const string MinerDataRemovedRoutingKey = "MinerDataRemoved";
        public const string MinerSignChangedRoutingKey = "MinerSignChanged";
        public const string ChangeMinerSignRoutingKey = "ChangeMinerSign";

        public const string QueryClientsForWsRoutingKey = "QueryClientsForWs";
        public const string QueryClientsForWsResponseRoutingKey = "QueryClientsForWsResponse{0}";
        public const string SpeedRoutingKey = WsMessage.Speed;
    }
}

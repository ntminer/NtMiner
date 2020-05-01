using NTMiner.Ws;

namespace NTMiner {
    public static class MqKeyword {
        public const string DurableQueueEndsWith = ".durable";

        public const string NTMinerExchange = "ntminer";

        public const string LoginNameHeaderName = "loginName";
        public const string MinerIpHeaderName = "minerIp";

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

        public const string SpeedRoutingKey = WsMessage.Speed;
        public const string GZippedSpeedRoutingKey = WsMessage.GZippedSpeed;
    }
}

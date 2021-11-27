namespace NTMiner {
    // 这类代码的意义只是为了将同类信息集中于一处便于程序员管理
    public static class RedisKeyword {
        /// <summary>
        /// 根据Id索引Miner对象的json
        /// </summary>
        public const string MinersMinerById = "miners.MinerById";
        /// <summary>
        /// 根据LoginName索引User对象的json
        /// </summary>
        public const string UsersUserByLoginName = "users.UserByLoginName";
        /// <summary>
        /// 根据Address索引WsServerNodeState对象的json
        /// </summary>
        public const string WsServerNodesWsServerNodeByAddress = "wsServerNodes.WsServerNodeByAddress";
        /// <summary>
        /// 是个键值对，键是Address值是时间戳
        /// </summary>
        public const string WsServerNodesAddress = "wsServerNodes.Address";
        /// <summary>
        /// 根据ClientId索引SpeedData对象的json
        /// </summary>
        public const string SpeedDatasSpeedDataByClientId = "speedDatas.SpeedDataByClientId";
        /// <summary>
        /// 根据Id索引Captcha对象的json
        /// </summary>
        public const string CaptchasCaptchaById = "captchas.CaptchaById";
        /// <summary>
        /// 是个键值对，键是CoinCode
        /// </summary>
        public const string CalcConfigs = "calcConfigs";
        /// <summary>
        /// 根据Id索引ActiveOn时间戳字符串，这个Id跟ClientData的Id是同一个Id。这里的时间戳字符串
        /// 不会经常更新但肯定会更新，因为这个时间戳的目的是为了周期发现用不活跃的记录从而删除掉。
        /// </summary>
        public const string ClientActiveOnById = "clientActiveOnById";
        public const string ClientTestId = "clientTestId";
    }
}

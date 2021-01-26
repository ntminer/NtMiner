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
    }
}

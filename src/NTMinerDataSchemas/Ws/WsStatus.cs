namespace NTMiner.Ws {
    /// <summary>
    /// 服务器Ws服务的状态，该状态在ServerState中夹带着返回给客户端从而提供给客户端一个提早发现服务端Ws服务可用状态的机会。
    /// 为什么说提早呢？
    /// 因为挖矿端每重试连接Ws服务失败一次会延长重试周期，从而多次重试不成功后周期会大于2分钟，而每两分钟会有一次算力上报，
    /// 算力上报成功时的返回值中带有服务端Ws服务的状态，从而客户端总是有机会在两分钟左右发现服务端的Ws服务的可用状态。
    /// </summary>
    public enum WsStatus {
        Undefined,
        Online,
        Offline
    }
}

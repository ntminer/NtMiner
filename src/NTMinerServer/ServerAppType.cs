namespace NTMiner {
    // 注意：不要重命名枚举成员，因为该枚举的成员名称已经拼接进rabbitmq队列名称是队列名称的段。
    public enum ServerAppType {
        WsServer,
        WebApiServer
    }
}

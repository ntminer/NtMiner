namespace NTMiner {
    public interface IMinerIp {
        /// <summary>
        /// 可能是空、1个或多个Ip，多个Ip以英文“,”号分隔，Ip末尾可能带有英文"()"小括号括住的"(动态)"或"(🔒)"
        /// 例如：192.168.1.110(🔒)
        /// 例如：192.168.1.110(🔒),10.1.1.119(动态)
        /// </summary>
        string LocalIp { get; }
        /// <summary>
        /// 服务端通过请求上下文得到的客户端Ip
        /// </summary>
        string MinerIp { get; }
    }
}

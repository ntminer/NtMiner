using System;

namespace NTMiner.Ws {
    // 考虑设计个自定义数据协议，用二进制传输，除了Data之外的字段放在Head部分且加入一个HeadLength字段，Head部分的第一个int字节是HeadLength，用RLP编码头部。
    // 但是二进制对将来的web群控客户端不友好，javascript不擅长处理二进制；
    // 这么滴吧，经测试序列化和反序列化挺快的，不会造成瓶颈，那就先传字符串序列化和反序列化去吧，将来如有必要再开辟新路径制定二进制协议传二进制；
    public class WsMessage : IData {
        #region message type 注意这些常量不区分大小写，注意唯一性
        // ping
        /// <summary>
        /// 服务端是个集群，两种情况下会向挖矿端发送ReGetServerAddress指令：
        /// 1. 当集群中上线了新节点后，可以计算出哪些客户端会分配到新节点去，从而通知这些节点重新获取服务器地址连向新节点；
        /// 2. 每个服务器节点周期（比如1分钟）检查自己持有的每个客户端连接是否还是分片到自己的客户端，如果不是则发送该消息要求客户端重新获取服务器地址。
        /// 以上两种情况中其中第二种是防护措施通常不会行走到。
        /// </summary>
        public const string ReGetServerAddress = "ReGetServerAddress";
        public const string UpdateAESPassword = "UpdateAESPassword";
        /// <summary>
        /// 指的是用户的密码变更后群控客户端需要重新登录而不是指挖矿端，挖矿端有连接概念并没有登录概念。
        /// </summary>
        public const string ReLogin = "ReLogin";
        public const string GetConsoleOutLines = "GetConsoleOutLines";
        public const string GetLocalMessages = "GetLocalMessages";
        public const string GetOperationResults = "GetOperationResults";
        public const string GetDrives = "GetDrives";
        public const string GetLocalIps = "GetLocalIps";
        public const string GetSpeed = "GetSpeed";
        public const string GetLocalJson = "GetLocalJson";
        public const string GetGpuProfilesJson = "GetGpuProfilesJson";

        public const string EnableRemoteDesktop = "EnableRemoteDesktop";
        public const string BlockWAU = "BlockWAU";
        public const string AtikmdagPatcher = "AtikmdagPatcher";
        public const string SwitchRadeonGpu = "SwitchRadeonGpu";
        public const string SetVirtualMemory = "SetVirtualMemory";
        public const string SetLocalIps = "SetLocalIps";
        public const string SaveGpuProfilesJson = "SaveGpuProfilesJson";
        public const string SetAutoBootStart = "SetAutoBootStart";
        public const string RestartWindows = "RestartWindows";
        public const string ShutdownWindows = "ShutdownWindows";
        public const string UpgradeNTMiner = "UpgradeNTMiner";
        public const string StartMine = "StartMine";
        public const string StopMine = "StopMine";

        // pong
        public const string ConsoleOutLines = "ConsoleOutLines";
        public const string OperationResults = "OperationResults";
        public const string Drives = "Drives";
        public const string LocalIps = "LocalIps";
        public const string LocalMessages = "LocalMessages";
        public const string Speed = "Speed";
        public const string OperationReceived = "OperationReceived";
        public const string LocalJson = "LocalJson";
        public const string GpuProfilesJson = "GpuProfilesJson";
        #endregion

        public WsMessage() {
        }

        public WsMessage(Guid id, string type) {
            this.Id = id;
            this.Type = type;
            this.Timestamp = NTMiner.Timestamp.GetTimestamp();
        }

        public Guid Id { get; set; }
        public string Type { get; set; }
        public long Timestamp { get; set; }
        public string Sign { get; set; }

        public object Data { get; set; }
    }
}

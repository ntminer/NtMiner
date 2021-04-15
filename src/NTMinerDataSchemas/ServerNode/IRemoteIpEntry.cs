using System;

namespace NTMiner.ServerNode {
    public interface IRemoteIpEntry {
        // IPAddress类型不支持序列化
        string RemoteIp { get; }
        int ActionTimes { get; }
        DateTime LastActionOn { get; }
        long ActionSpeed { get; }
    }
}

using System;

namespace NTMiner.MinerServer {
    public interface IMineWork : IEntity<Guid> {
        Guid Id { get; }
        string Name { get; }
        string Description { get; }
        // 保持作业时作业中引用的服务器数据库记录的指纹
        string ServerJsonSha1 { get; }
    }
}

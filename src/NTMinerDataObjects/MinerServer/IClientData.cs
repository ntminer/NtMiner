using System;

namespace NTMiner.MinerServer {
    public interface IClientData : IEntity<Guid> {
        Guid WorkId { get; }

        string Version { get; }

        bool IsMining { get; }

        string MinerName { get; }

        Guid GroupId { get; }

        string MinerIp { get; }

        string RemoteUserName { get; }

        string RemotePassword { get; }

        string MainCoinCode { get; }

        double MainCoinSpeed { get; }

        string MainCoinPool { get; }

        string MainCoinWallet { get; }

        string Kernel { get; }

        bool IsDualCoinEnabled { get; }

        string DualCoinCode { get; }

        double DualCoinSpeed { get; }

        string DualCoinPool { get; }

        string DualCoinWallet { get; }

        string GpuInfo { get; }
    }
}

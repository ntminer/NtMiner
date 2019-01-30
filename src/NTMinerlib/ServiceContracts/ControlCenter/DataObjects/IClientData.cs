using System;

namespace NTMiner.ServiceContracts.ControlCenter.DataObjects {
    public interface IClientData : IEntity<Guid> {
        Guid WorkId { get; }

        string Version { get; }

        bool IsMining { get; }

        string MinerName { get; }

        Guid GroupId { get; }

        string MinerIp { get; }

        string MainCoinCode { get; }

        string MainCoinPool { get; }

        string MainCoinWallet { get; }

        string Kernel { get; }

        bool IsDualCoinEnabled { get; }

        string DualCoinCode { get; }

        string DualCoinPool { get; }

        string DualCoinWallet { get; }

        string GpuInfo { get; }
    }
}

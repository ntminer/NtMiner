﻿using NTMiner.Hashrate;
using System;

namespace NTMiner.MinerServer {
    public interface IClientData : IEntity<Guid> {
        Guid WorkId { get; }

        string Version { get; }

        bool IsMining { get; }

        DateTime BootOn { get; }

        DateTime? MineStartedOn { get; }

        string MinerName { get; }

        Guid GroupId { get; }

        string MinerIp { get; }

        string RemoteUserName { get; }

        string RemotePassword { get; }

        string MainCoinCode { get; }

        int MainCoinTotalShare { get; }

        int MainCoinRejectShare { get; }

        double MainCoinSpeed { get; }

        string MainCoinPool { get; }

        string MainCoinWallet { get; }

        string Kernel { get; }

        bool IsDualCoinEnabled { get; }

        string DualCoinCode { get; }

        int DualCoinTotalShare { get; }

        int DualCoinRejectShare { get; }

        double DualCoinSpeed { get; }

        string DualCoinPool { get; }

        string DualCoinWallet { get; }

        string GpuInfo { get; }

        string OSName { get; }

        double OSVirtualMemory { get; }

        GpuType GpuType { get; }

        string GpuDriver { get; }

        GpuSpeedData[] GpuTable { get; }
    }
}

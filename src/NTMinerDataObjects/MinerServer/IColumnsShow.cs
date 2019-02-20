using System;

namespace NTMiner.MinerServer {
    public interface IColumnsShow : IEntity<Guid> {
        bool Work { get; }
        bool MinerName { get; }
        bool MinerIp  { get; }
        bool MinerGroup  { get; }
        bool MainCoinCode  { get; }
        bool MainCoinSpeedText  { get; }
        bool MainCoinWallet  { get; }
        bool MainCoinPool  { get; }
        bool Kernel  { get; }
        bool DualCoinCode  { get; }
        bool DualCoinSpeedText  { get; }
        bool DualCoinWallet  { get; }
        bool DualCoinPool  { get; }
        bool LastActivedOnText  { get; }
        bool Version  { get; }
        bool RemoteUserNameAndPassword  { get; }
        bool GpuInfo  { get; }
    }
}

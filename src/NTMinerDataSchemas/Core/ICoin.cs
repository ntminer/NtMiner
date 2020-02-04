using System;

namespace NTMiner.Core {
    public interface ICoin : IEntity<Guid> {
        Guid Id { get; }
        string Code { get; }
        string EnName { get; }
        string CnName { get; }
        string Icon { get; }
        Guid AlgoId { get; }
        // json向后兼容
        string Algo { get; }
        string TestWallet { get; }
        string WalletRegexPattern { get; }
        bool JustAsDualCoin { get; }
        string Notice { get; }
        string TutorialUrl { get; }
        bool IsHot { get; }
        /// <summary>
        /// GpuType:KernelBrandId;GpuType:KernelBrandId
        /// </summary>
        string KernelBrand { get; }
        double MinGpuMemoryGb { get; }
    }
}

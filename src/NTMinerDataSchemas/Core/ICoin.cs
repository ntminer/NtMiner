using System;

namespace NTMiner.Core {
    public interface ICoin : IEntity<Guid> {
        Guid Id { get; }
        string Code { get; }
        string EnName { get; }
        string CnName { get; }
        string Icon { get; }
        Guid AlgoId { get; }
        // json向后兼容，是个展示问题，不影响业务，后续可以删除这个字段。
        string Algo { get; }
        string TestWallet { get; }
        string WalletRegexPattern { get; }
        bool JustAsDualCoin { get; }
        string Notice { get; }
        string TutorialUrl { get; }
        bool IsHot { get; }
        /// <summary>
        /// 指示该币种默认选中的内核品牌，如果不指定则界面上默认选中第一个内核。
        /// 具体到具体显卡类型，格式形如：GpuType:KernelBrandId;GpuType:KernelBrandId
        /// </summary>
        string KernelBrand { get; }
        double MinGpuMemoryGb { get; }
    }
}

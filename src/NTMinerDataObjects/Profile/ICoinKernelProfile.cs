using System;

namespace NTMiner.Profile {
    public interface ICoinKernelProfile : ICanUpdateByReflection {
        Guid CoinKernelId { get; }
        bool IsDualCoinEnabled { get; }
        Guid DualCoinId { get; }
        double DualCoinWeight { get; }
        bool IsAutoDualWeight { get; }
        string CustomArgs { get; }
        /// <summary>
        /// 用户触碰过的自定义参数
        /// </summary>
        string TouchedArgs { get; }
    }
}

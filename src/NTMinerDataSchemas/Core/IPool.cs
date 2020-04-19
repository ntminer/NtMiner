using System;

namespace NTMiner.Core {
    public interface IPool : ILevelEntity<Guid> {
        Guid Id { get; }
        Guid CoinId { get; }
        Guid BrandId { get; }
        string Name { get; }
        string Server { get; }
        /// <summary>
        /// 收益查看地址
        /// </summary>
        string Url { get; }
        string Website { get; }
        int SortNumber { get; }
        bool IsUserMode { get; }
        string UserName { get; }
        string Password { get; }
        string Notice { get; }
        string TutorialUrl { get; }
        bool NoPool1 { get; }
        bool NotPool1 { get; }
        string MinerNamePrefix { get; }
        string MinerNamePostfix { get; }
    }
}

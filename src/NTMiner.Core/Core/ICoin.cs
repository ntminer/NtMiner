using System;

namespace NTMiner.Core {
    public interface ICoin : IEntity<Guid> {
        string Code { get; }
        string EnName { get; }
        string CnName { get; }
        string Algo { get; }
        int SortNumber { get; }
        string TestWallet { get; }
        string WalletRegexPattern { get; }
        bool JustAsDualCoin { get; }
    }
}

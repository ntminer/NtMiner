using System;

namespace NTMiner.Core {
    public interface IMineContext {
        Guid Id { get; }
        string MinerName { get; }
        ICoin MainCoin { get; }
        IPool MainCoinPool { get; }
        IKernel Kernel { get; }
        ICoinKernel CoinKernel { get; }
        string MainCoinWallet { get; }
        int ProcessDisappearedCound { get; set; }
        string PipeFileName { get; }

        string CommandLine { get; }

        DateTime CreatedOn { get; }
    }
}

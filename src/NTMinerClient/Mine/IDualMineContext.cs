using NTMiner.Core;

namespace NTMiner.Mine {
    public interface IDualMineContext : IMineContext {
        ICoin DualCoin { get; }
        IPool DualCoinPool { get; }
        string DualCoinWallet { get; }
        double DualCoinWeight { get; }
    }
}

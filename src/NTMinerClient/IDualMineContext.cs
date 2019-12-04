using NTMiner.Core;

namespace NTMiner {
    public interface IDualMineContext : IMineContext {
        ICoin DualCoin { get; }
        IPool DualCoinPool { get; }
        string DualCoinWallet { get; }
        double DualCoinWeight { get; }
    }
}

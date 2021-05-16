using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Mine {
    public class DualMineContext : MineContext, IDualMineContext {
        public DualMineContext(
            IMineContext mineContext,
            ICoin dualCoin,
            IPool dualCoinPool,
            string dualCoinWallet,
            double dualCoinWeight,
            Dictionary<string, string> parameters,
            Dictionary<Guid, string> fragments,
            Dictionary<Guid, string> fileWriters,
            int[] useDevices) : base(
                mineContext.MinerName,
                mineContext.MainCoin,
                mineContext.MainCoinPool,
                mineContext.Kernel,
                mineContext.KernelInput,
                mineContext.KernelOutput,
                mineContext.CoinKernel,
                mineContext.MainCoinWallet,
                mineContext.CommandLine,
                mineContext.IsTestWallet,
                mineContext.IsTestUserName,
                parameters,
                fragments,
                fileWriters,
                useDevices) {
            this.DualCoin = dualCoin;
            this.DualCoinPool = dualCoinPool;
            this.DualCoinWallet = dualCoinWallet;
            this.DualCoinWeight = dualCoinWeight;
        }


        public ICoin DualCoin { get; private set; }

        public IPool DualCoinPool { get; private set; }

        public string DualCoinWallet { get; private set; }

        public double DualCoinWeight { get; private set; }
    }
}

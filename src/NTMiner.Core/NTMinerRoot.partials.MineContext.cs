using NTMiner.Core;
using System;

namespace NTMiner {
    public partial class NTMinerRoot : INTMinerRoot {
        private class MineContext : IMineContext {
            public MineContext(
                string minerName,
                ICoin mainCoin,
                IPool mainCoinPool,
                IKernel kernel,
                ICoinKernel coinKernel,
                string mainCoinWallet,
                string commandLine) {

                this.Id = Guid.NewGuid();
                this.MinerName = minerName;
                this.MainCoin = mainCoin;
                this.MainCoinPool = mainCoinPool;
                this.Kernel = kernel;
                this.CoinKernel = coinKernel;
                this.MainCoinWallet = mainCoinWallet;
                this.ProcessDisappearedCound = 0;
                this.CommandLine = commandLine;
                this.CreatedOn = DateTime.Now;
                this.PipeFileName = "pip_" + DateTime.Now.Ticks.ToString() + ".log";
            }

            public Guid Id { get; private set; }

            public string MinerName { get; private set; }

            public ICoin MainCoin { get; private set; }

            public IPool MainCoinPool { get; private set; }

            public IKernel Kernel { get; private set; }

            public ICoinKernel CoinKernel { get; private set; }

            public string MainCoinWallet { get; private set; }

            public int ProcessDisappearedCound { get; set; }

            public string PipeFileName { get; private set; }

            public string CommandLine { get; private set; }

            public DateTime CreatedOn { get; private set; }
        }

        private class DualMineContext : MineContext, IDualMineContext {
            public DualMineContext(
                IMineContext mineContext,
                ICoin dualCoin,
                IPool dualCoinPool,
                string dualCoinWallet,
                double dualCoinWeight) : base(
                    mineContext.MinerName,
                    mineContext.MainCoin,
                    mineContext.MainCoinPool,
                    mineContext.Kernel,
                    mineContext.CoinKernel,
                    mineContext.MainCoinWallet,
                    mineContext.CommandLine) {
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
}

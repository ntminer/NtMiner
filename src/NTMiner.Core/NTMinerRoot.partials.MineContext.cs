using NTMiner.Core;
using System;
using System.Collections.Generic;

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
                string commandLine,
                Dictionary<string, string> parameters,
                Dictionary<Guid, string> fragments,
                Dictionary<Guid, string> fileWriters) {
                this.Fragments = fragments;
                this.FileWriters = fileWriters;
                this.Id = Guid.NewGuid();
                this.MinerName = minerName;
                this.MainCoin = mainCoin;
                this.MainCoinPool = mainCoinPool;
                this.Kernel = kernel;
                this.CoinKernel = coinKernel;
                this.MainCoinWallet = mainCoinWallet;
                this.AutoRestartKernelCount = 0;
                this.KernelSelfRestartCount = 0;
                this.CommandLine = commandLine;
                this.CreatedOn = DateTime.Now;
                this.PipeFileName = $"{kernel.Code}_pip_{DateTime.Now.Ticks.ToString()}.log";
                this.Parameters = parameters;
            }

            public Guid Id { get; private set; }

            public string MinerName { get; private set; }

            public ICoin MainCoin { get; private set; }

            public IPool MainCoinPool { get; private set; }

            public IKernel Kernel { get; private set; }

            public ICoinKernel CoinKernel { get; private set; }

            public string MainCoinWallet { get; private set; }

            public int AutoRestartKernelCount { get; set; }

            public int KernelSelfRestartCount { get; set; }

            public string PipeFileName { get; private set; }

            public string CommandLine { get; private set; }

            public DateTime CreatedOn { get; private set; }

            public Dictionary<string, string> Parameters { get; private set; }

            public Dictionary<Guid, string> Fragments { get; private set; }

            public Dictionary<Guid, string> FileWriters { get; private set; }
        }

        private class DualMineContext : MineContext, IDualMineContext {
            public DualMineContext(
                IMineContext mineContext,
                ICoin dualCoin,
                IPool dualCoinPool,
                string dualCoinWallet,
                double dualCoinWeight,
                Dictionary<string, string> parameters,
                Dictionary<Guid, string> fragments,
                Dictionary<Guid, string> fileWriters) : base(
                    mineContext.MinerName,
                    mineContext.MainCoin,
                    mineContext.MainCoinPool,
                    mineContext.Kernel,
                    mineContext.CoinKernel,
                    mineContext.MainCoinWallet,
                    mineContext.CommandLine,
                    parameters,
                    fragments,
                    fileWriters) {
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

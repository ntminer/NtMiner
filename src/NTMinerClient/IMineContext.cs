using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NTMiner {
    public interface IMineContext {
        void Start(bool isRestart);
        void Close();
        bool IsClosed { get; }
        Guid Id { get; }
        bool IsRestart { get; }
        string MinerName { get; }
        ICoin MainCoin { get; }
        IPool MainCoinPool { get; }
        IKernel Kernel { get; }
        ICoinKernel CoinKernel { get; }
        string MainCoinWallet { get; }
        int AutoRestartKernelCount { get; set; }
        int KernelSelfRestartCount { get; set; }
        string LogFileFullName { get; }
        KernelProcessType KernelProcessType { get; }

        DateTime MineStartedOn { get; }
        Dictionary<string, string> Parameters { get; }
        Dictionary<Guid, string> Fragments { get; }
        Dictionary<Guid, string> FileWriters { get; }
        int[] UseDevices { get; }
        IKernelInput KernelInput { get; }
        IKernelOutput KernelOutput { get; }
        string CommandLine { get; }
        Process KernelProcess { get; set; }
    }
}

using NTMiner.Core;
using NTMiner.Hub;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NTMiner {
    public interface IMineContext {
        event Action OnKill;
        void AddEventPath<TEvent>(string description, LogEnum logType, Action<TEvent> action, Type location) where TEvent : IEvent;
        void AddOnecePath<TMessage>(string description, LogEnum logType, Action<TMessage> action, Guid pathId, Type location);
        void NewLogFileName();
        void Kill();
        void Close();
        bool IsClosed { get; }
        Guid Id { get; }
        bool IsRestart { get; set; }
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

        DateTime CreatedOn { get; }
        Dictionary<string, string> Parameters { get; }
        Dictionary<Guid, string> Fragments { get; }
        Dictionary<Guid, string> FileWriters { get; }
        int[] UseDevices { get; }
        IKernelInput KernelInput { get; }
        IKernelOutput KernelOutput { get; }
        string CommandLine { get; }
        Process KernelProcess { get; set; }
        Dictionary<string, object> Data { get; }
    }
}

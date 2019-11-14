using NTMiner.Core;
using System.Collections.Generic;

namespace NTMiner.JsonDb {
    public interface IServerJsonDb : IJsonDb {
        CoinGroupData[] CoinGroups { get; }
        List<CoinKernelData> CoinKernels { get; }
        CoinData[] Coins { get; }
        GroupData[] Groups { get; }
        KernelInputData[] KernelInputs { get; }
        KernelOutputData[] KernelOutputs { get; }
        KernelOutputTranslaterData[] KernelOutputTranslaters { get; }
        List<KernelData> Kernels { get; }
        List<PoolKernelData> PoolKernels { get; }
        List<PoolData> Pools { get; }
        SysDicItemData[] SysDicItems { get; }
        SysDicData[] SysDics { get; }
        List<FileWriterData> FileWriters { get; }
        List<FragmentWriterData> FragmentWriters { get; }
        List<PackageData> Packages { get; }
    }
}
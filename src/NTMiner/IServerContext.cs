using NTMiner.Core;
using NTMiner.Core.Kernels;

namespace NTMiner {
    public interface IServerContext {
        ICoinGroupSet CoinGroupSet { get; }
        ICoinSet CoinSet { get; }
        IFileWriterSet FileWriterSet { get; }
        IFragmentWriterSet FragmentWriterSet { get; }
        IGroupSet GroupSet { get; }
        IPoolSet PoolSet { get; }
        ISysDicItemSet SysDicItemSet { get; }
        ISysDicSet SysDicSet { get; }
        ICoinKernelSet CoinKernelSet { get; }
        IKernelInputSet KernelInputSet { get; }
        IKernelOutputSet KernelOutputSet { get; }
        IKernelOutputTranslaterSet KernelOutputTranslaterSet { get; }
        IKernelSet KernelSet { get; }
        IPackageSet PackageSet { get; }
        IPoolKernelSet PoolKernelSet { get; }
    }
}

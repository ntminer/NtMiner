using System;

namespace NTMiner.Core {
    public interface IKernel : IEntity<Guid> {
        string Code { get; }
        Guid BrandId { get; }
        string Version { get; }
        ulong PublishOn { get; }
        string Package { get; }
        long Size { get; }
        PublishStatus PublishState { get; }

        string Notice { get; }
        Guid KernelInputId { get; }
        Guid KernelOutputId { get; }
    }
}

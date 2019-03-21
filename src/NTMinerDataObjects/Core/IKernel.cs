using System;

namespace NTMiner.Core {
    public interface IKernel : IEntity<Guid> {
        string Code { get; }
        Guid BrandId { get; }
        string Version { get; }
        ulong PublishOn { get; }
        string Package { get; }
        string Sha1 { get; }
        long Size { get; }
        PublishStatus PublishState { get; }

        string HelpArg { get; set; }
        string Notice { get; }
        Guid KernelInputId { get; }
        Guid KernelOutputId { get; }
    }
}

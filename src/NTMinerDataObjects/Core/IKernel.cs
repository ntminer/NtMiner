using System;

namespace NTMiner.Core.Kernels {
    public interface IKernel : IEntity<Guid> {
        string Code { get; }
        string Version { get; }
        string FullName { get; }
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

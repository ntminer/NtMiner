using System;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels {
    public interface IPackageSet : IEnumerable<IPackage> {
        bool Contains(Guid packageId);
        bool TryGetPackage(Guid packageId, out IPackage package);
    }
}

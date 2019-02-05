using System;

namespace NTMiner.Core.Profiles {
    public interface ICoinKernelProfileSet {
        ICoinKernelProfile GetCoinKernelProfile(Guid coinKernelId);
        void SetCoinKernelProfileProperty(Guid coinKernelId, string propertyName, object value);
    }
}

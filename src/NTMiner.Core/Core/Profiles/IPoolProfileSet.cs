using NTMiner.Profile;
using System;

namespace NTMiner.Core.Profiles {
    public interface IPoolProfileSet {
        IPoolProfile GetPoolProfile(Guid poolId);
        void SetPoolProfileProperty(Guid poolId, string propertyName, object value);
    }
}

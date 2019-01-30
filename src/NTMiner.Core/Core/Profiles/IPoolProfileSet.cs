using NTMiner.ServiceContracts.ControlCenter.DataObjects;
using System;

namespace NTMiner.Core.Profiles {
    public interface IPoolProfileSet {
        IPoolProfile GetPoolProfile(Guid poolId);
        void SetPoolProfileProperty(Guid poolId, string propertyName, object value);
    }
}

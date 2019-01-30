using System;

namespace NTMiner.ServiceContracts.ControlCenter.DataObjects {
    public interface IPoolProfile {
        Guid PoolId { get; }
        string UserName { get; }
        string Password { get; }
    }
}

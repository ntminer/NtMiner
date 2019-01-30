using System;

namespace NTMiner.ServiceContracts.ControlCenter.DataObjects {
    public interface IMineWork : IEntity<Guid> {
        string Name { get; }
        string Description { get; }
    }
}

using System;

namespace NTMiner.ServiceContracts.DataObjects {
    public interface IMineWork : IEntity<Guid> {
        string Name { get; }
        string Description { get; }
    }
}

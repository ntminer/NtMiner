using System;

namespace NTMiner.ServiceContracts.DataObjects {
    public interface IMinerGroup : IEntity<Guid> {
        string Name { get; }
        string Description { get; }
    }
}

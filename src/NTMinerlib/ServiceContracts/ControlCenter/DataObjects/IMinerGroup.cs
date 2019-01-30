using System;

namespace NTMiner.ServiceContracts.ControlCenter.DataObjects {
    public interface IMinerGroup : IEntity<Guid> {
        string Name { get; }
        string Description { get; }
    }
}

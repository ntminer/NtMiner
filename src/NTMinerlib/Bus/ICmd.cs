namespace NTMiner.Bus {
    using NTMiner;
    using System;

    public interface ICmd : IMessage, IEntity<Guid> {
    }
}

namespace NTMiner.Bus {
    using System;

    public interface ICmd : IMessage, IEntity<Guid> {
    }
}

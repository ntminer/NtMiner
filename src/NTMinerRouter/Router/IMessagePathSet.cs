using System;
using System.Collections.Generic;

namespace NTMiner.Router {
    public interface IMessagePathSet {
        IEnumerable<IMessagePathId> GetAllPaths();
        void Route<TMessage>(TMessage message) where TMessage : IMessage;

        void Add<TMessage>(MessagePath<TMessage> handler);

        void Remove(IMessagePathId handlerId);

        event Action<IMessagePathId> Added;
        event Action<IMessagePathId> Removed;
    }
}

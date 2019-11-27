using System;
using System.Collections.Generic;

namespace NTMiner.Bus {
    public interface IMessageDispatcher {
        IEnumerable<IMessagePathId> GetAllPaths();
        void Dispatch<TMessage>(TMessage message) where TMessage : IMessage;

        void Connect<TMessage>(MessagePath<TMessage> handler);

        void Disconnect(IMessagePathId handlerId);

        event Action<IMessagePathId> Connected;
        event Action<IMessagePathId> Disconnected;
    }
}

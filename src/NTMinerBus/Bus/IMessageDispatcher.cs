using System;

namespace NTMiner.Bus {
    public interface IMessageDispatcher {
        void Dispatch<TMessage>(TMessage message);

        void Connect<TMessage>(MessagePath<TMessage> handler);

        void Disconnect(IMessagePathId handlerId);

        event Action<IMessagePathId> Connected;
        event Action<IMessagePathId> Disconnected;
    }
}

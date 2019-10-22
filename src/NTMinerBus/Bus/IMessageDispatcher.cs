using System;

namespace NTMiner.Bus {
    public interface IMessageDispatcher {
        void Dispatch<TMessage>(TMessage message);

        void Connect<TMessage>(DelegatePath<TMessage> handler);

        void Disconnect(IPathId handlerId);

        event Action<IPathId> Connected;
        event Action<IPathId> Disconnected;
    }
}

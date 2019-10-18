using System;

namespace NTMiner.Bus {
    public interface IMessageDispatcher {
        void Dispatch<TMessage>(TMessage message);

        void Connect<TMessage>(DelegateHandler<TMessage> handler);

        void Disconnect(IHandlerId handlerId);

        event Action<IHandlerId> Connected;
        event Action<IHandlerId> Disconnected;
    }
}

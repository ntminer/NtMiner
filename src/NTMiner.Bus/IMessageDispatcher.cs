
using System;
using System.Collections.Generic;

namespace NTMiner.Bus {
    public interface IMessageDispatcher {
        void DispatchMessage<TMessage>(TMessage message);

        void Register<TMessage>(DelegateHandler<TMessage> handler);

        void UnRegister(IDelegateHandler handler);

        IEnumerable<IHandlerId> HandlerIds { get; }
        event Action<IHandlerId> HandlerIdAdded;
        event Action<IHandlerId> HandlerIdRemoved;
    }
}

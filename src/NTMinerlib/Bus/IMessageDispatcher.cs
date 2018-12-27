
namespace NTMiner.Bus {
    using System;

    public interface IMessageDispatcher {
        void DispatchMessage<TMessage>(TMessage message);

        void Register<TMessage>(DelegateHandler<TMessage> handler);

        void UnRegister<TMessage>(DelegateHandler<TMessage> handler);

        event EventHandler<MessageDispatchEventArgs> Dispatching;

        event EventHandler<MessageDispatchEventArgs> DispatchFailed;

        event EventHandler<MessageDispatchEventArgs> Dispatched;
    }
}

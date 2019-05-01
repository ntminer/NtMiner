
namespace NTMiner.Bus {
    public interface IMessageDispatcher {
        bool HasSubscriber<TMessage>();
        void DispatchMessage<TMessage>(TMessage message);

        void Register<TMessage>(DelegateHandler<TMessage> handler);

        void UnRegister(IDelegateHandler handler);
    }
}

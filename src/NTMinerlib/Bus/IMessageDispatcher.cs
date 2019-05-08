
namespace NTMiner.Bus {
    public interface IMessageDispatcher {
        void DispatchMessage<TMessage>(TMessage message);

        void Register<TMessage>(DelegateHandler<TMessage> handler);

        void UnRegister(IDelegateHandler handler);
    }
}

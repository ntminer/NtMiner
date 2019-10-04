using System;

namespace NTMiner.Bus {
    public interface IMessageDispatcher {
        void DispatchMessage<TMessage>(TMessage message);

        /// <summary>
        /// 接通
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="handler"></param>
        void Connect<TMessage>(DelegateHandler<TMessage> handler);

        /// <summary>
        /// 断开
        /// </summary>
        /// <param name="handlerId"></param>
        void Disconnect(IHandlerId handlerId);

        /// <summary>
        /// 接通时
        /// </summary>
        event Action<IHandlerId> Connected;
        /// <summary>
        /// 断开时
        /// </summary>
        event Action<IHandlerId> Disconnected;
    }
}

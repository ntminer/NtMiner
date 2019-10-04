using System;

namespace NTMiner.Bus {
    /// <summary>
    /// 委托处理器，该处理器将处理逻辑委托给构造时传入的委托。
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class DelegateHandler<TMessage> : IMessageHandler {
        private readonly Action<TMessage> _action;

        public IHandlerId HandlerId { get; private set; }

        public DelegateHandler(IHandlerId handlerId, Action<TMessage> action) {
            this.HandlerId = handlerId;
            _action = action;
        }

        public void Handle(TMessage message) {
            try {
                _action?.Invoke(message);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(HandlerId.HandlerPath + ":" + e.Message, e);
                throw;
            }
        }
    }
}

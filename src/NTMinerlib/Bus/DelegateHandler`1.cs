using System;

namespace NTMiner.Bus {
    public class DelegateHandler<TMessage> {
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

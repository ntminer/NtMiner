using System;
using System.Collections.Generic;

namespace NTMiner.Bus {
    public class DelegateHandler<TMessage> : IDelegateHandler {
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

        public DelegateHandler<TMessage> AddToCollection(List<IDelegateHandler> handlers) {
            if (!handlers.Contains(this)) {
                handlers.Add(this);
            }
            return this;
        }
    }
}

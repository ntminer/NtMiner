
namespace NTMiner.Bus {
    using System;

    public class MessageDispatchEventArgs : EventArgs {
        #region Public Properties
        public dynamic Message { get; private set; }

        public Type HandlerType { get; private set; }

        public object Handler { get; private set; }
        #endregion

        #region Ctor
        public MessageDispatchEventArgs() { }

        public MessageDispatchEventArgs(dynamic message, Type handlerType, object handler) {
            this.Message = message;
            this.HandlerType = handlerType;
            this.Handler = handler;
        }
        #endregion
    }
}

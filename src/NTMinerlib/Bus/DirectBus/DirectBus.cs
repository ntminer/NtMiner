
namespace NTMiner.Bus.DirectBus {
    using System;
    using System.Collections.Generic;

    public abstract class DirectBus : IBus {
        #region Private Fields
        private readonly IMessageDispatcher _dispatcher;
        private readonly Queue<dynamic> _messageQueue = new Queue<dynamic>();
        private readonly object _queueLock = new object();
        private dynamic[] _backupMessageArray;
        #endregion

        #region Ctor
        protected DirectBus(IMessageDispatcher dispatcher) {
            if (dispatcher == null) {
                throw new ArgumentNullException("dispatcher");
            }
            this._dispatcher = dispatcher;
        }
        #endregion

        #region IBus Members
        public void Publish<TMessage>(TMessage message) {
            lock (_queueLock) {
                _messageQueue.Enqueue(message);
            }
        }

        public void Publish<TMessage>(IEnumerable<TMessage> messages) {
            lock (_queueLock) {
                foreach (var message in messages) {
                    _messageQueue.Enqueue(message);
                }
            }
        }

        public void Clear() {
            lock (_queueLock) {
                this._messageQueue.Clear();
            }
        }
        #endregion

        public void Commit() {
            lock (_queueLock) {
                _backupMessageArray = new dynamic[_messageQueue.Count];
                _messageQueue.CopyTo(_backupMessageArray, 0);
                while (_messageQueue.Count > 0) {
                    _dispatcher.DispatchMessage(_messageQueue.Dequeue());
                }
            }
        }
    }
}

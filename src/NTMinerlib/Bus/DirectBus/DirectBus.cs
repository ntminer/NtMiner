
namespace NTMiner.Bus.DirectBus {
    using System;
    using System.Collections.Generic;

    public abstract class DirectBus : IBus {
        #region Private Fields
        private volatile bool _committed = true;
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
                _committed = false;
            }
        }

        public void Publish<TMessage>(IEnumerable<TMessage> messages) {
            lock (_queueLock) {
                foreach (var message in messages) {
                    _messageQueue.Enqueue(message);
                }
                _committed = false;
            }
        }

        public void Clear() {
            lock (_queueLock) {
                this._messageQueue.Clear();
            }
        }
        #endregion

        #region IUnitOfWork Members
        public bool DistributedTransactionSupported {
            get { return false; }
        }

        public bool Committed {
            get { return this._committed; }
        }

        public void Commit() {
            lock (_queueLock) {
                _backupMessageArray = new dynamic[_messageQueue.Count];
                _messageQueue.CopyTo(_backupMessageArray, 0);
                while (_messageQueue.Count > 0) {
                    _dispatcher.DispatchMessage(_messageQueue.Dequeue());
                }
                _committed = true;
            }
        }

        public void Rollback() {
            lock (_queueLock) {
                if (_backupMessageArray != null && _backupMessageArray.Length > 0) {
                    _messageQueue.Clear();
                    foreach (var msg in _backupMessageArray) {
                        _messageQueue.Enqueue(msg);
                    }
                }
                _committed = false;
            }
        }
        #endregion
    }
}

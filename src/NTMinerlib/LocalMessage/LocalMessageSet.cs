using LiteDB;
using NTMiner.Core.MinerClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.LocalMessage {
    public class LocalMessageSet : ILocalMessageSet {
        private readonly string _connectionString;
        private readonly LinkedList<ILocalMessage> _records = new LinkedList<ILocalMessage>();

        public LocalMessageSet(string dbFileFullName) {
            if (!string.IsNullOrEmpty(dbFileFullName)) {
                _connectionString = $"filename={dbFileFullName}";
            }
            VirtualRoot.AddCmdPath<AddLocalMessageCommand>(action: message => {
                if (string.IsNullOrEmpty(_connectionString)) {
                    return;
                }
                InitOnece();
                var data = LocalMessageData.Create(message.Input);
                // TODO:批量持久化，异步持久化
                List<ILocalMessage> removes = new List<ILocalMessage>();
                lock (_locker) {
                    _records.AddFirst(data);
                    try {
                        while (_records.Count > NTKeyword.LocalMessageSetCapacity) {
                            var toRemove = _records.Last;
                            removes.Add(toRemove.Value);
                            _records.RemoveLast();
                            using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                                var col = db.GetCollection<LocalMessageData>();
                                col.Delete(toRemove.Value.Id);
                            }
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                }
                try {
                    using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                        var col = db.GetCollection<LocalMessageData>();
                        col.Insert(data);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
                VirtualRoot.RaiseEvent(new LocalMessageAddedEvent(message.Id, data, removes));
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<ClearLocalMessageSetCommand>(action: message => {
                if (string.IsNullOrEmpty(_connectionString)) {
                    return;
                }
                lock (_locker) {
                    _records.Clear();
                }
                try {
                    using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                        db.DropCollection(nameof(LocalMessageData));
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
                VirtualRoot.RaiseEvent(new LocalMessageSetClearedEvent());
            }, location: this.GetType());
        }

        private bool _isInited = false;
        private readonly object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    if (string.IsNullOrEmpty(_connectionString)) {
                        return;
                    }
                    try {
                        using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                            var col = db.GetCollection<LocalMessageData>();
                            foreach (var item in col.FindAll().OrderBy(a => a.Timestamp)) {
                                if (_records.Count < NTKeyword.LocalMessageSetCapacity) {
                                    _records.AddFirst(item);
                                }
                                else {
                                    col.Delete(item.Id);
                                }
                            }
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                        try {
                            using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                                db.DropCollection(nameof(LocalMessageData));
                            }
                        }
                        catch {
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public IEnumerable<ILocalMessage> AsEnumerable() {
            InitOnece();
            return _records;
        }
    }
}

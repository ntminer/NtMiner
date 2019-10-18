using LiteDB;
using NTMiner.MinerClient;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class WorkerEventSet : IWorkerEventSet {
        private int _lastWorkerEventId;
        private readonly string _connectionString;

        public WorkerEventSet() {
            _connectionString = $"filename={SpecialPath.WorkerEventDbFileFullName};journal=false";
            VirtualRoot.EventPath<WorkerEvent>("将矿机事件记录到磁盘", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                        var col = db.GetCollection<WorkerEventData>();
                        _lastWorkerEventId = col.Insert(new WorkerEventData {
                            Id = 0,
                            Channel = message.Channel,
                            Content = message.Content,
                            EventOn = DateTime.Now
                        }).AsInt32;
                    }
                });
        }

        public int LastWorkerEventId {
            get { return _lastWorkerEventId; }
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
                    using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                        var col = db.GetCollection<WorkerEventData>();
                        _lastWorkerEventId = col.Max(a => a.Id).AsInt32;
                    }
                    _isInited = true;
                }
            }
        }

        public IEnumerable<IWorkerEvent> GetEvents(WorkerEventChannel channel, string keyword) {
            InitOnece();
            using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                var col = db.GetCollection<WorkerEventData>();
                if (channel != WorkerEventChannel.Undefined) {
                    if (!string.IsNullOrEmpty(keyword)) {
                        return col.Find(
                            Query.And(
                                Query.GT("_id", _lastWorkerEventId - VirtualRoot.WorkerEventSetSliding),
                                Query.EQ(nameof(WorkerEventData.Channel), channel.GetName()),
                                Query.Contains(nameof(WorkerEventData.Content), keyword)));
                    }
                    else {
                        return col.Find(
                            Query.And(
                                Query.GT("_id", _lastWorkerEventId - VirtualRoot.WorkerEventSetSliding),
                                Query.EQ(nameof(WorkerEventData.Channel), channel.GetName())));
                    }
                }
                else {
                    if (!string.IsNullOrEmpty(keyword)) {
                        return col.Find(
                            Query.And(
                                Query.GT("_id", _lastWorkerEventId - VirtualRoot.WorkerEventSetSliding),
                                Query.Contains(nameof(WorkerEventData.Content), keyword)));
                    }
                    else {
                        return col.Find(Query.GT("_id", _lastWorkerEventId - VirtualRoot.WorkerEventSetSliding));
                    }
                }
            }
        }
    }
}

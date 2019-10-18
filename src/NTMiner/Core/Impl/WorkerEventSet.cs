using LiteDB;
using NTMiner.MinerClient;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class WorkerEventSet : IWorkerEventSet {
        private int _lastWorkerEventId;

        public WorkerEventSet() {
            VirtualRoot.EventPath<WorkerEventHappenedEvent>("将矿机事件记录到磁盘", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    using (LiteDatabase db = new LiteDatabase($"filename={SpecialPath.WorkerEventDbFileFullName};journal=false")) {
                        var col = db.GetCollection<WorkerEventData>();
                        _lastWorkerEventId = col.Insert(WorkerEventData.Create(message.Source)).AsInt32;
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
                    using (LiteDatabase db = new LiteDatabase($"filename={SpecialPath.WorkerEventDbFileFullName};journal=false")) {
                        var col = db.GetCollection<WorkerEventData>();
                        _lastWorkerEventId = col.Max(a => a.Id).AsInt32;
                    }
                    _isInited = true;
                }
            }
        }

        public IEnumerable<IWorkerEvent> GetEvents(Guid typeId, string keyword) {
            InitOnece();
            using (LiteDatabase db = new LiteDatabase($"filename={SpecialPath.WorkerEventDbFileFullName};journal=false")) {
                var col = db.GetCollection<WorkerEventData>();
                if (typeId != Guid.Empty) {
                    if (!string.IsNullOrEmpty(keyword)) {
                        return col.Find(
                            Query.And(
                                Query.GT("_id", _lastWorkerEventId - VirtualRoot.WorkerEventSetSliding),
                                Query.EQ(nameof(WorkerEventData.EventTypeId), typeId),
                                Query.Contains(nameof(WorkerEventData.Content), keyword))); ;
                    }
                    else {
                        return col.Find(
                            Query.And(
                                Query.GT("_id", _lastWorkerEventId - VirtualRoot.WorkerEventSetSliding),
                                Query.EQ(nameof(WorkerEventData.EventTypeId), typeId)));
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

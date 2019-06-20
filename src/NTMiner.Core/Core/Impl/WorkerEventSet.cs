using LiteDB;
using NTMiner.MinerClient;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class WorkerEventSet : IWorkerEventSet {
        private readonly INTMinerRoot _root;

        public WorkerEventSet(INTMinerRoot root) {
            _root = root;
            VirtualRoot.On<WorkerEventOccurredEvent>("将矿机事件记录到磁盘", LogEnum.DevConsole,
                action: message => {
                    using (LiteDatabase db = new LiteDatabase($"filename={SpecialPath.WorkerEventDbFileFullName};journal=false")) {
                        var col = db.GetCollection<WorkerEventData>();
                        col.Insert(WorkerEventData.Create(message.Source));
                    }
                });
        }

        public IEnumerable<IWorkerEvent> GetEvents(Guid typeId, string keyword) {
            using (LiteDatabase db = new LiteDatabase($"filename={SpecialPath.WorkerEventDbFileFullName};journal=false")) {
                var col = db.GetCollection<WorkerEventData>();
                col.EnsureIndex(nameof(WorkerEventData.EventOn), unique: false);
                if (typeId != Guid.Empty) {
                    if (!string.IsNullOrEmpty(keyword)) {
                        return col.Find(
                            Query.And(
                                Query.EQ(nameof(WorkerEventData.EventTypeId), typeId),
                                Query.Contains(nameof(WorkerEventData.Description), keyword)));
                    }
                    else {
                        return col.Find(Query.EQ(nameof(WorkerEventData.EventTypeId), typeId));
                    }
                }
                else {
                    if (!string.IsNullOrEmpty(keyword)) {
                        return col.Find(Query.Contains(nameof(WorkerEventData.Description), keyword));
                    }
                    else {
                        return col.FindAll();
                    }
                }
            }
        }
    }
}

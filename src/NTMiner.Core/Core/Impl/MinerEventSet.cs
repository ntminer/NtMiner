using LiteDB;
using NTMiner.MinerClient;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class MinerEventSet : IMinerEventSet {
        private readonly INTMinerRoot _root;

        public MinerEventSet(INTMinerRoot root) {
            _root = root;
        }

        public IEnumerable<IMinerEvent> GetEvents(Guid? typeId, string keyword) {
            using (LiteDatabase db = new LiteDatabase($"filename={SpecialPath.LocalDbFileFullName};journal=false")) {
                var col = db.GetCollection<MinerEventData>();
                col.EnsureIndex(nameof(MinerEventData.EventOn), unique: false);
                if (typeId.HasValue) {
                    if (!string.IsNullOrEmpty(keyword)) {
                        return col.Find(
                            Query.And(
                                Query.EQ(nameof(MinerEventData.TypeId), typeId.Value),
                                Query.Contains(nameof(MinerEventData.Description), keyword)));
                    }
                    else {
                        return col.Find(Query.EQ(nameof(MinerEventData.TypeId), typeId.Value));
                    }
                }
                else {
                    if (!string.IsNullOrEmpty(keyword)) {
                        return col.Find(Query.Contains(nameof(MinerEventData.Description), keyword));
                    }
                    else {
                        return col.FindAll();
                    }
                }
            }
        }
    }
}

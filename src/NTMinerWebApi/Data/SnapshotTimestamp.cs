using LiteDB;
using System;

namespace NTMiner.Data {
    public class SnapshotTimestamp {
        [BsonId]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }

        public static DateTime GetSnapshotTimestamp() {
            using (LiteDatabase db = HostRoot.CreateReportDb()) {
                var col = db.GetCollection<SnapshotTimestamp>();
                SnapshotTimestamp entity = col.FindById(1);
                if (entity == null) {
                    return HostRoot.Current.ClientCoinSnapshotSet.GetMinTimestamp();
                }
                return entity.Timestamp;
            }
        }

        public static void SetSnapshotTimestamp(DateTime timestamp) {
            using (LiteDatabase db = HostRoot.CreateReportDb()) {
                var col = db.GetCollection<SnapshotTimestamp>();
                SnapshotTimestamp entity = col.FindById(1);
                if (entity == null) {
                    col.Insert(new SnapshotTimestamp {
                        Id = 1,
                        Timestamp = timestamp
                    });
                }
                else {
                    entity.Timestamp = timestamp;
                    col.Update(entity);
                }
            }
        }
    }
}

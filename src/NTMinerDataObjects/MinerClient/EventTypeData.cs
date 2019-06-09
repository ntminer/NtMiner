using System;

namespace NTMiner.MinerClient {
    public class EventTypeData : IEventType, IDbEntity<Guid> {
        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}

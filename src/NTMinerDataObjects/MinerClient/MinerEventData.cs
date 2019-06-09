using System;

namespace NTMiner.MinerClient {
    public class MinerEventData : IMinerEvent, IDbEntity<Guid> {
        public MinerEventData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public MinerEventSource Source { get; set; }

        public Guid TypeId { get; set; }

        public string Description { get; set; }

        public DateTime EventOn { get; set; }
    }
}

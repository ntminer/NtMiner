using System;

namespace NTMiner.Bus {
    public abstract class EventBase : IEvent {
        protected EventBase() {
            this.Id = Guid.NewGuid();
            this.Timestamp = DateTime.Now;
        }

        protected EventBase(Guid bornPathId) : this() {
            this.BornPathId = bornPathId;
        }

        public Guid Id { get; private set; }

        public Guid BornPathId { get; private set; }

        public DateTime Timestamp { get; private set; }
    }
}

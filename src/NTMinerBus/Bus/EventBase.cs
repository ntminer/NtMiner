using System;

namespace NTMiner.Bus {
    public abstract class EventBase : IEvent {
        protected EventBase() {
            this.Id = Guid.NewGuid();
            this.Timestamp = DateTime.Now;
        }

        protected EventBase(Guid pathId) : this() {
            this.PathId = pathId;
        }

        public Guid Id { get; private set; }

        public Guid PathId { get; private set; }

        public DateTime Timestamp { get; private set; }
    }
}

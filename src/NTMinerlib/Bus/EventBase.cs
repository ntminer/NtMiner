using System;

namespace NTMiner.Bus {
    public abstract class EventBase : IEvent {
        public EventBase() {
            this.Id = Guid.NewGuid();
            this.Timestamp = DateTime.Now;
        }
        public Guid GetId() {
            return this.Id;
        }


        public Guid Id { get; private set; }

        public DateTime Timestamp { get; private set; }
    }
}

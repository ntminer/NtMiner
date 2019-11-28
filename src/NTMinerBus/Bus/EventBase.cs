using System;

namespace NTMiner.Bus {
    public abstract class EventBase : IEvent {
        protected EventBase() {
            this.Id = Guid.NewGuid();
            this.BornOn = DateTime.Now;
            this.BornPathId = Guid.Empty;
        }

        protected EventBase(Guid bornPathId) : this() {
            this.BornPathId = bornPathId;
        }

        public Guid Id { get; private set; }

        public Guid BornPathId { get; private set; }

        public DateTime BornOn { get; private set; }
    }
}

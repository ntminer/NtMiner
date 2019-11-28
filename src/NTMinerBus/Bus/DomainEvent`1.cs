using System;

namespace NTMiner.Bus {
    public abstract class DomainEvent<TEntity> : IEvent {
        protected DomainEvent(Guid bornPathId, TEntity source) {
            this.Id = Guid.NewGuid();
            this.BornPathId = bornPathId;
            this.Target = source;
            this.Timestamp = DateTime.Now;
        }

        public Guid Id { get; private set; }
        public Guid BornPathId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TEntity Target { get; private set; }
    }
}

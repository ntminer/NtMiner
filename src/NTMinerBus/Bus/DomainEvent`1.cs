using System;

namespace NTMiner.Bus {
    public abstract class DomainEvent<TEntity> : IEvent {
        protected DomainEvent(TEntity source) {
            this.Id = Guid.NewGuid();
            this.Source = source;
            this.Timestamp = DateTime.Now;
        }
        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TEntity Source { get; private set; }
    }
}

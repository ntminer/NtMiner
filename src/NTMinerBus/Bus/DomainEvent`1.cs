using System;

namespace NTMiner.Bus {
    public abstract class DomainEvent<TEntity> : IEvent {
        protected DomainEvent(Guid pathId, TEntity source) {
            this.Id = Guid.NewGuid();
            this.PathId = pathId;
            this.Source = source;
            this.Timestamp = DateTime.Now;
        }

        public Guid Id { get; private set; }
        public Guid PathId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TEntity Source { get; private set; }
    }
}

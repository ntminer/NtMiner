using System;

namespace NTMiner.Router {
    public abstract class DomainEvent<TEntity> : IEvent {
        protected DomainEvent(Guid bornPathId, TEntity source) {
            this.Id = Guid.NewGuid();
            this.BornPathId = bornPathId;
            this.Target = source;
            this.BornOn = DateTime.Now;
        }

        public Guid Id { get; private set; }
        public Guid BornPathId { get; private set; }
        public DateTime BornOn { get; private set; }
        public TEntity Target { get; private set; }
    }
}

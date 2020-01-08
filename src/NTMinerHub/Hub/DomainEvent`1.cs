using System;

namespace NTMiner.Hub {
    public abstract class DomainEvent<TEntity> : IEvent {
        protected DomainEvent(PathId targetPathId, TEntity source) {
            this.Id = Guid.NewGuid();
            this.TargetPathId = targetPathId;
            this.Target = source;
            this.BornOn = DateTime.Now;
        }

        public Guid Id { get; private set; }
        public PathId TargetPathId { get; private set; }
        public DateTime BornOn { get; private set; }
        public TEntity Target { get; private set; }
    }
}

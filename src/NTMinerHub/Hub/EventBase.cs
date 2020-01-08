using System;

namespace NTMiner.Hub {
    public abstract class EventBase : IEvent {
        protected EventBase() {
            this.Id = Guid.NewGuid();
            this.BornOn = DateTime.Now;
            this.TargetPathId = PathId.Empty;
        }

        protected EventBase(PathId targetPathId) : this() {
            this.TargetPathId = targetPathId;
        }

        public Guid Id { get; private set; }

        public PathId TargetPathId { get; private set; }

        public DateTime BornOn { get; private set; }
    }
}

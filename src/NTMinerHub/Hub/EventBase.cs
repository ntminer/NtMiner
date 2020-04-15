using System;

namespace NTMiner.Hub {
    public abstract class EventBase : IEvent {
        protected EventBase() {
            this.MessageId = Guid.NewGuid();
            this.BornOn = DateTime.Now;
            this.TargetPathId = PathId.Empty;
        }

        protected EventBase(PathId targetPathId) : this() {
            this.TargetPathId = targetPathId;
        }

        public Guid MessageId { get; private set; }

        /// <summary>
        /// <see cref="IEvent.TargetPathId"/>
        /// </summary>
        public PathId TargetPathId { get; private set; }

        /// <summary>
        /// <see cref="IEvent.BornOn"/>
        /// </summary>
        public DateTime BornOn { get; private set; }
    }
}

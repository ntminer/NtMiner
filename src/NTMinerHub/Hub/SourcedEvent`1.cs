using System;

namespace NTMiner.Hub {
    public abstract class SourcedEvent<TEntity> : IEvent {
        protected SourcedEvent(PathId targetPathId, TEntity source) {
            this.MessageId = Guid.NewGuid();
            this.TargetPathId = targetPathId;
            this.Source = source;
            this.BornOn = DateTime.Now;
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

        /// <summary>
        /// 事件诞生地，事件的源头。
        /// </summary>
        public TEntity Source { get; private set; }
    }
}

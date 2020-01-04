using System;

namespace NTMiner.Hub {
    public abstract class EventBase : IEvent {
        protected EventBase() {
            this.Id = Guid.NewGuid();
            this.BornOn = DateTime.Now;
            this.RouteToPathId = Guid.Empty;
        }

        protected EventBase(Guid routeToPathId) : this() {
            this.RouteToPathId = routeToPathId;
        }

        public Guid Id { get; private set; }

        public Guid RouteToPathId { get; private set; }

        public DateTime BornOn { get; private set; }
    }
}

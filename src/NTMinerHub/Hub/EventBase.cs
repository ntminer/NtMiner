using System;

namespace NTMiner.Hub {
    public abstract class EventBase : IEvent {
        protected EventBase() {
            this.Id = Guid.NewGuid();
            this.BornOn = DateTime.Now;
            this.RouteToPathId = RouteToPathId.All;
        }

        protected EventBase(RouteToPathId routeToPathId) : this() {
            this.RouteToPathId = routeToPathId;
        }

        public Guid Id { get; private set; }

        public RouteToPathId RouteToPathId { get; private set; }

        public DateTime BornOn { get; private set; }
    }
}

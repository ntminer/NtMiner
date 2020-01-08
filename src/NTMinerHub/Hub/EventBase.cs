using System;

namespace NTMiner.Hub {
    public abstract class EventBase : IEvent {
        protected EventBase() {
            this.Id = Guid.NewGuid();
            this.BornOn = DateTime.Now;
            this.RouteToPathId = PathId.Empty;
        }

        protected EventBase(PathId routeToPathId) : this() {
            this.RouteToPathId = routeToPathId;
        }

        public Guid Id { get; private set; }

        public PathId RouteToPathId { get; private set; }

        public DateTime BornOn { get; private set; }
    }
}

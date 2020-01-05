using System;

namespace NTMiner.Hub {
    public struct RouteToPathId {
        public static RouteToPathId All = new RouteToPathId(Guid.Empty);

        public readonly Guid PathId;

        public RouteToPathId(Guid pathId) {
            this.PathId = pathId;
        }

        public bool IsAll {
            get {
                return PathId == Guid.Empty;
            }
        }

        public static implicit operator RouteToPathId(Guid guid) {
            return new RouteToPathId(guid);
        }

        public static bool operator==(RouteToPathId pathId, Guid guid) {
            return pathId.PathId == guid;
        }

        public static bool operator!=(RouteToPathId pathId, Guid guid) {
            return !(pathId == guid);
        }

        public static bool operator ==(RouteToPathId left, RouteToPathId right) {
            return left.PathId == right.PathId;
        }

        public static bool operator !=(RouteToPathId left, RouteToPathId right) {
            return !(left == right);
        }

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }

            if (!(obj is RouteToPathId data)) {
                return false;
            }

            return this.PathId == data.PathId;
        }

        public bool Equals(RouteToPathId obj) {
            return this.PathId == obj.PathId;
        }

        public override int GetHashCode() {
            return this.PathId.GetHashCode();
        }
    }
}

using System;

namespace NTMiner.Hub {
    /// <summary>
    /// 消息路径的标识或消息的目标路径的标识。对Guid的包装，只为词汇，只为可读。
    /// </summary>
    public struct PathId {
        public static PathId Empty = new PathId(Guid.Empty);

        private readonly Guid Id;

        public PathId(Guid id) {
            this.Id = id;
        }

        public static implicit operator PathId(Guid id) {
            return new PathId(id);
        }

        public static bool operator==(PathId pathId, Guid id) {
            return pathId.Id == id;
        }

        public static bool operator!=(PathId pathId, Guid id) {
            return !(pathId == id);
        }

        public static bool operator ==(PathId left, PathId right) {
            return left.Id == right.Id;
        }

        public static bool operator !=(PathId left, PathId right) {
            return !(left == right);
        }

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }

            if (!(obj is PathId data)) {
                return false;
            }

            return this.Id == data.Id;
        }

        public bool Equals(PathId obj) {
            return this.Id == obj.Id;
        }

        public override int GetHashCode() {
            return this.Id.GetHashCode();
        }
    }
}

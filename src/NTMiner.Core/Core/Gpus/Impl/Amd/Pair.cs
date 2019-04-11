namespace NTMiner.Core.Gpus.Impl.Amd {
    public struct Pair<F, S> {
        private F first;
        private S second;

        public Pair(F first, S second) {
            this.first = first;
            this.second = second;
        }

        public F First {
            get { return first; }
            set { first = value; }
        }

        public S Second {
            get { return second; }
            set { second = value; }
        }

        public override int GetHashCode() {
            return (first != null ? first.GetHashCode() : 0) ^
              (second != null ? second.GetHashCode() : 0);
        }
    }
}

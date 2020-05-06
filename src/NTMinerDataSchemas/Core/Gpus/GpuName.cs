namespace NTMiner.Core.Gpus {
    public class GpuName : IGpuName {
        public GpuName() { }

        public string Name { get; set; }

        public ulong TotalMemory { get; set; }

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }
            return this.ToString() == obj.ToString(); ;
        }

        public override int GetHashCode() {
            return this.ToString().GetHashCode();
        }

        public override string ToString() {
            ulong totalMemoryGb = (this.TotalMemory + NTKeyword.ULongG - 1) / NTKeyword.ULongG;
            return $"{this.Name}///{totalMemoryGb.ToString()}";
        }
    }
}

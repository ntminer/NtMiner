namespace NTMiner.Core.Gpus {
    public class GpuName : IGpuName {
        public GpuName() { }

        public string Name { get; set; }

        public ulong TotalMemory { get; set; }

        public override int GetHashCode() {
            return this.ToString().GetHashCode();
        }

        public override string ToString() {
            const ulong gb = 1024 * 1024 * 1024;
            ulong totalMemoryGb = (this.TotalMemory + gb - 1) / gb;
            return $"{this.Name}///{totalMemoryGb.ToString()}";
        }
    }
}

namespace NTMiner.Core.Gpus.Impl {
    internal class Gpu : IGpu {
        public static readonly IGpu Total = new Gpu {
            Index = NTMinerRoot.Current.GpuAllId,
            Name = "全部显卡",
            Temperature = 0,
            FanSpeed = 0,
            PowerUsage = 0
        };

        public Gpu() {
        }

        public int Index { get; set; }

        public string Name { get; set; }

        public uint Temperature { get; set; }

        public uint FanSpeed { get; set; }

        public uint PowerUsage { get; set; }
    }
}

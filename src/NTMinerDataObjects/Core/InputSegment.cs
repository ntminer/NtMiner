namespace NTMiner.Core {
    /// <summary>
    /// Segment是分割的片段，里面没有变量
    /// </summary>
    public class InputSegment : IInputSegment {
        public InputSegment() {
            this.TargetGpu = SupportedGpu.Both;
            this.IsDefault = false;
        }

        public InputSegment(IInputSegment data) {
            this.TargetGpu = data.TargetGpu;
            this.Name = data.Name;
            this.Segment = data.Name;
            this.Description = data.Description;
            this.IsDefault = data.IsDefault;
        }

        public void Update(IInputSegment data) {
            this.TargetGpu = data.TargetGpu;
            this.Name = data.Name;
            this.Segment = data.Name;
            this.Description = data.Description;
            this.IsDefault = data.IsDefault;
        }

        public SupportedGpu TargetGpu { get; set; }
        public string Name { get; set; }
        public string Segment { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
    }
}

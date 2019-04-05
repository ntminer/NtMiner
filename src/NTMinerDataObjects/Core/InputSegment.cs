namespace NTMiner.Core {
    public class InputSegment : IInputSegment {
        public InputSegment() { }

        public InputSegment(IInputSegment data) {
            this.Name = data.Name;
            this.Segment = data.Name;
            this.Description = data.Description;
        }

        public string Name { get; set; }
        public string Segment { get; set; }
        public string Description { get; set; }
    }
}

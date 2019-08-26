namespace NTMiner.Core {
    public interface IInputSegment {
        SupportedGpu TargetGpu { get; }
        string Name { get; }
        string Segment { get; }
        string Description { get; }
        bool IsDefault { get; }
    }
}

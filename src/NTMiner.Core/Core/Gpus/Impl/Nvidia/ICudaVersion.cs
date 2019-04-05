namespace NTMiner.Core.Gpus.Impl.Nvidia {
    public interface ICudaVersion : IDbEntity<string> {
        string Description { get; }
        double MinDriverVersion { get; }
        string Version { get; }
    }
}
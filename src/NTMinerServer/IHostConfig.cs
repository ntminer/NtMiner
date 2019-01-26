namespace NTMiner {
    public interface IHostConfig {
        string OssAccessKeyId { get; }
        string OssAccessKeySecret { get; }
        string OssEndpoint { get; }
        ulong ServerJsonVersion { get; set; }
    }
}

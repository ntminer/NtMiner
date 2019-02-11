namespace NTMiner.MinerServer {
    public interface IAppSetting {
        string Key { get; }
        object Value { get; }
    }
}

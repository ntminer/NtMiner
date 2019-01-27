namespace NTMiner.ServiceContracts.DataObjects {
    public interface IAppSetting {
        string Key { get; }
        object Value { get; }
    }
}

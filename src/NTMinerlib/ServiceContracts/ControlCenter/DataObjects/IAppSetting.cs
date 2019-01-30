namespace NTMiner.ServiceContracts.ControlCenter.DataObjects {
    public interface IAppSetting {
        string Key { get; }
        object Value { get; }
    }
}

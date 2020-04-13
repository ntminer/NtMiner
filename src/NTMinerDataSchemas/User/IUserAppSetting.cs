namespace NTMiner.User {
    public interface IUserAppSetting : IAppSetting {
        string Id { get; }
        string LoginName { get; }
    }
}

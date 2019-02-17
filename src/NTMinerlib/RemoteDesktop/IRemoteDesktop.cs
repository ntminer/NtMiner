namespace NTMiner.RemoteDesktop {
    public interface IRemoteDesktop {
        void OpenRemoteDesktop(string serverIp, string userName, string password, string description);
    }
}

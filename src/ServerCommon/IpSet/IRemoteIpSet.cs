namespace NTMiner.IpSet {
    public interface IRemoteIpSet : ICountSet {
        RemoteIpEntry[] GetTopNRemoteIpEntries(int n);
    }
}

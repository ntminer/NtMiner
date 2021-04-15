namespace NTMiner.IpSet {
    public interface IRemoteIpSet {
        int Count { get; }
        RemoteIpEntry[] GetTopNRemoteIpEntries(int n);
    }
}

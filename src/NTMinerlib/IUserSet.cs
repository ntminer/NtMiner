using NTMiner.ServiceContracts.DataObjects;

namespace NTMiner {
    public interface IUserSet {
        bool Contains(string pubKey);
        bool TryGetKey(string pubKey, out IUser key);
    }
}

using NTMiner.ServiceContracts.ControlCenter.DataObjects;

namespace NTMiner {
    public interface IUserSet {
        bool Contains(string pubKey);
        bool TryGetKey(string pubKey, out IUser key);
    }
}

using System;
using System.Threading.Tasks;

namespace NTMiner.Core {
    public interface IMinerIdSet {
        bool TryGetMinerId(Guid clientId, out string minerId);
        void Set(Guid clientId, string minerId);
        void Remove(Guid clientId);
        Task WaitReadiedAsync();
    }
}

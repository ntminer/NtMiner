using System;

namespace NTMiner.Core {
    public interface IPool : ILevelEntity<Guid> {
        Guid CoinId { get; }
        string Name { get; }
        string Server { get; }
        string Url { get; }
        int SortNumber { get; }
        bool IsUserMode { get; }
        string UserName { get; }
        string Password { get; }
        string Notice { get; }
    }
}

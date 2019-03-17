using NTMiner.User;
using System;

namespace NTMiner {
    public interface IHostRoot {
        DateTime StartedOn { get; }
        IUserSet UserSet { get; }
        void RefreshUserSet();
    }
}

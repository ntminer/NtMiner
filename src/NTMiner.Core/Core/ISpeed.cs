using System;

namespace NTMiner.Core {
    public interface ISpeed {
        double Value { get; }
        DateTime SpeedOn { get; }
        int FoundShare { get; }
        int AcceptShare { get; }
        int RejectShare { get; }
        int IncorrectShare { get; }
    }
}

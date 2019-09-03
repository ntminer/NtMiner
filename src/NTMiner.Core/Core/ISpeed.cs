using System;

namespace NTMiner.Core {
    public interface ISpeed {
        double Value { get; }
        DateTime SpeedOn { get; }
        int FoundShare { get; }
        int RejectShare { get; }
    }
}

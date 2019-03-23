using System;

namespace NTMiner.Core {
    public interface ISpeed {
        double Value { get; }
        DateTime SpeedOn { get; }
    }
}

using System;

namespace NTMiner.Core {
    public interface ISpeed {
        double Value { get; set; }
        DateTime SpeedOn { get; set; }
    }
}

using System;

namespace NTMiner.Core {
    public interface ISpeed {
        long Value { get; set; }
        DateTime SpeedOn { get; set; }
    }
}

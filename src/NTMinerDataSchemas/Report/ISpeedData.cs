using System;

namespace NTMiner.Report {
    public interface ISpeedData : ISpeedDto {
        DateTime SpeedOn { get; }
    }
}

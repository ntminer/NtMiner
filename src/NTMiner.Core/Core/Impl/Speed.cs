using System;

namespace NTMiner.Core.Impl {
    public class Speed : ISpeed {
        public static ISpeed Empty = new Speed {
            Value = 0
        };

        public Speed() {
            Value = 0;
            SpeedOn = DateTime.Now;
        }

        public DateTime SpeedOn { get; set; }

        public double Value { get; set; }
    }
}

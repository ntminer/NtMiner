using System;

namespace NTMiner.Core.Impl {
    internal class Speed : ISpeed {
        public static ISpeed Empty = new Speed {
            Value = 0
        };

        public Speed() {
            Value = 0;
            SpeedOn = DateTime.Now;
        }

        public DateTime SpeedOn { get; set; }

        public long Value { get; set; }
    }
}

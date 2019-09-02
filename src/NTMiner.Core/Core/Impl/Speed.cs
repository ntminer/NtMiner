using System;

namespace NTMiner.Core.Impl {
    public class Speed : ISpeed {
        public static ISpeed Empty = new Speed {
        };

        public Speed() {
            SpeedOn = DateTime.Now;
        }

        public DateTime SpeedOn { get; set; }

        public double Value { get; set; }

        public int AcceptShare { get; set; }

        public int RejectShare { get; set; }
    }
}

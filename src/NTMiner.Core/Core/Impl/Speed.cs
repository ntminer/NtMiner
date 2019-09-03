using System;

namespace NTMiner.Core.Impl {
    public class Speed : ISpeed {
        public static ISpeed Empty = new Speed {
        };

        public Speed() {
            SpeedOn = DateTime.Now;
        }

        public Speed(ISpeed data) {
            this.SpeedOn = data.SpeedOn;
            this.Value = data.Value;
            this.AcceptShare = data.AcceptShare;
            this.RejectShare = data.RejectShare;
        }

        public DateTime SpeedOn { get; set; }

        public double Value { get; set; }

        public int AcceptShare { get; set; }

        public int RejectShare { get; set; }
    }
}

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
            this.FoundShare = data.FoundShare;
            this.AcceptShare = data.AcceptShare;
            this.RejectShare = data.RejectShare;
            this.IncorrectShare = data.IncorrectShare;
        }

        public void Reset() {
            Value = 0;
            SpeedOn = DateTime.Now;
            FoundShare = 0;
            AcceptShare = 0;
            RejectShare = 0;
            IncorrectShare = 0;
        }

        public void ResetShare() {
            FoundShare = 0;
            AcceptShare = 0;
            RejectShare = 0;
            IncorrectShare = 0;
        }

        public DateTime SpeedOn { get; set; }

        public double Value { get; set; }

        public int FoundShare { get; set; }

        public int AcceptShare { get; set; }

        public int RejectShare { get; set; }

        public int IncorrectShare { get; set; }
    }
}

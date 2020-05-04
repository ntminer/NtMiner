using System;

namespace NTMiner.User {
    public class TryLoginTimes : ITryLoginTimes {
        public TryLoginTimes() { }

        public string LoginName { get; set; }
        public int Times { get; set; }
        public DateTime LastTryOn { get; set; }
    }
}

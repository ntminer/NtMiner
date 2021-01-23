using System;

namespace NTMiner.Ws {
    public class WsClientState {
        public WsClientState() {
            NextTrySecondsDelay = -1;
            LastTryOn = DateTime.MinValue;
        }

        public WsClientStatus Status { get; set; }
        public string Description { get; set; }
        public string WsServerIp { get; set; }
        public int NextTrySecondsDelay { get; set; }
        public DateTime LastTryOn { get; set; }
        public bool ToOut { get; set; }
    }
}

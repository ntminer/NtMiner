using System;

namespace NTMiner.Ws {
    public class EmptyWsClient : IWsClient {
        public static readonly IWsClient Empty = new EmptyWsClient {
            IsOpen = false
        };

        private EmptyWsClient() { }

        public bool IsOpen { get; private set; }

        public WsClientState GetState() {
            return new WsClientState {
                Status = WsClientStatus.Closed,
                Description = nameof(EmptyWsClient),
                LastTryOn = DateTime.MinValue,
                NextTrySecondsDelay = -1
            };
        }

        public void SendAsync(WsMessage message) {
            // 什么也不做
        }

        public void OpenOrCloseWs(bool isResetFailCount = false) {
            // 什么也不做
        }
    }
}

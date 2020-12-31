using NTMiner.Controllers;
using System;

namespace NTMiner.Ws {
    public class DaemonWsClient : AbstractWsClient {
        public DaemonWsClient() : base(NTMinerAppType.MinerClient) {
        }

        protected override bool TryGetHandler(string messageType, out Action<Action<WsMessage>, WsMessage> handler) {
            return DaemonWsMessageHandler.TryGetHandler(messageType, out handler);
        }

        protected override void UpdateWsStateAsync(string description, bool toOut) {
            if (VirtualRoot.DaemonOperation.IsNTMinerOpened()) {
                var state = base.GetState();
                if (!string.IsNullOrEmpty(description)) {
                    state.Description = description;
                }
                state.ToOut = toOut;
                RpcRoot.JsonRpc.FirePostAsync(NTKeyword.Localhost, NTKeyword.MinerClientPort, RpcRoot.GetControllerName<IMinerClientController>(), nameof(IMinerClientController.ReportWsDaemonState), null, state, timeountMilliseconds: 3000);
            }
        }
    }
}

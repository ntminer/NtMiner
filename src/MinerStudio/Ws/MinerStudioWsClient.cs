using System;

namespace NTMiner.Ws {
    public class MinerStudioWsClient : AbstractWsClient {
        public MinerStudioWsClient() : base(NTMinerAppType.MinerStudio) {
        }

        protected override bool TryGetHandler(string messageType, out Action<Action<WsMessage>, WsMessage> handler) {
            return MinerStudioWsMessageHandler.TryGetHandler(messageType, out handler);
        }

        protected override void UpdateWsStateAsync(string description, bool toOut) {
            var state = base.GetState();
            if (!string.IsNullOrEmpty(description)) {
                state.Description = description;
            }
            state.ToOut = toOut;
            VirtualRoot.Execute(new RefreshWsStateCommand(state));
        }
    }
}

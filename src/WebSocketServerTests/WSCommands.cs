using NTMiner.Hub;
using System;

namespace NTMiner {
    public abstract class WsCommandBase : ICmd {
        public WsCommandBase(string action, Guid id, WebSocketBehaviorBase ws) {
            this.Action = action;
            this.Id = id;
            this.Ws = ws;
        }

        public string Action { get; private set; }
        public Guid Id { get; private set; }
        public WebSocketBehaviorBase Ws { get; private set; }
    }

    public class GetSpeedWsCommand : WsCommandBase {
        public GetSpeedWsCommand(string action, Guid id, WebSocketBehaviorBase ws) : base(action, id, ws) {
        }
    }
}

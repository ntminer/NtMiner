using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace NTMiner {
    public class AllInOne : WebSocketBehaviorBase {
        protected override void OnOpen() {
            base.OnOpen();
            Write.DevWarn("ConnCount " + Sessions.Count);
        }

        protected override void OnClose(CloseEventArgs e) {
            base.OnClose(e);
            Write.DevWarn("ConnCount " + Sessions.Count);
        }

        protected override void OnError(ErrorEventArgs e) {
            base.OnError(e);
            Write.DevException(e.Exception);
        }

        protected override void OnMessage(MessageEventArgs e) {
            if (string.IsNullOrEmpty(e.Data) || e.Data[0] != '{' || e.Data[e.Data.Length - 1] != '}') {
                return;
            }
            JsonRequest request = VirtualRoot.JsonSerializer.Deserialize<JsonRequest>(e.Data);
            if (request == null) {
                return;
            }
            switch (request.action) {
                case "getSpeed":
                    request.Parse(out Guid messageId);
                    VirtualRoot.Execute(new GetSpeedWsCommand(request.action, messageId, this));
                    break;
                default:
                    base.Send($"invalid action: {request.action}");
                    break;
            }
            Write.DevWarn("ConnCount " + Sessions.Count);
        }
    }
}

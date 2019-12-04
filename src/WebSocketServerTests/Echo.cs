using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace NTMiner {
    public class Echo : WebSocketBehavior {
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
            if (string.IsNullOrEmpty(e.Data)) {
                return;
            }
            if (e.Data[0] != '{' || e.Data[e.Data.Length - 1] != '}') {
                base.Send(e.Data);
            }
            else {
                JsonRequest request = VirtualRoot.JsonSerializer.Deserialize<JsonRequest>(e.Data);
                if (request == null) {
                    return;
                }
                switch (request.action) {
                    case "getSpeed":
                        Dictionary<string, object> data = VirtualRoot.JsonSerializer.Deserialize<Dictionary<string, object>>(request.json);
                        string messageId = string.Empty;
                        if (data != null) {
                            messageId = data["messageId"]?.ToString();
                        }
                        base.Send(new JsonResponse {
                            messageId = messageId,
                            code = 200,
                            phrase = "Ok",
                            des = "成功",
                            res = "getSpeed",
                            data = new Dictionary<string, object> {
                                        {"str", "hello" },
                                        {"num", 111 },
                                        {"date", DateTime.Now }
                                    }
                        }.ToJson());
                        break;
                    default:
                        base.Send(e.Data);
                        break;
                }
            }
            Write.DevWarn("ConnCount " + Sessions.Count);
        }
    }
}

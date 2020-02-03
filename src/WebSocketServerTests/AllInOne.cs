using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace NTMiner {
    public class AllInOne : WebSocketBehavior {
        private static readonly HashSet<string> _holdSessionIds = new HashSet<string>();
        private static bool _isFirst = true;
        private static object _locker = new object();

        protected override void OnOpen() {
            base.OnOpen();
            if (_isFirst) {
                lock (_locker) {
                    if (_isFirst) {
                        VirtualRoot.AddEventPath<Per10SecondEvent>("测试，周期getSpeed", LogEnum.None, action: message => {
                            foreach (var sessionId in _holdSessionIds) {
                                VirtualRoot.Execute(new GetSpeedWsCommand(Guid.Empty, sessionId, base.Sessions));
                            }
                        }, location: this.GetType());
                        _isFirst = false;
                    }
                }
            }
            Write.DevWarn("ConnCount " + base.Sessions.Count);
            _holdSessionIds.Add(base.ID);
        }

        protected override void OnClose(CloseEventArgs e) {
            _holdSessionIds.Remove(base.ID);
            base.OnClose(e);
            Write.DevWarn("ConnCount " + base.Sessions.Count);
        }

        protected override void OnError(ErrorEventArgs e) {
            _holdSessionIds.Remove(base.ID);
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
                case GetSpeedWsCommand.Action:
                    request.Parse(out Guid messageId);
                    VirtualRoot.Execute(new GetSpeedWsCommand(messageId, base.ID, base.Sessions));
                    break;
                default:
                    base.Send($"invalid action: {request.action}");
                    break;
            }
            Write.DevWarn("ConnCount " + base.Sessions.Count);
        }
    }
}

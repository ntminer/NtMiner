using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;
using WsCommands;

namespace NTMiner {
    public class AllInOne : WebSocketBehavior {
        private static readonly HashSet<string> _holdSessionIds = new HashSet<string>();
        private static bool _isFirst = true;
        private static readonly object _locker = new object();

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
            WsMessage message = VirtualRoot.JsonSerializer.Deserialize<WsMessage>(e.Data);
            if (message == null) {
                return;
            }
            switch (message.GetAction()) {
                case GetSpeedWsCommand.ResponseAction:
                    Write.DevDebug(e.Data);
                    break;
                default:
                    base.Send(new WsMessage().SetAction(message.GetAction()).SetCode(400).SetDes("invalid action").SetPhrase("invalid action").ToJson());
                    break;
            }
            Write.DevWarn("ConnCount " + base.Sessions.Count);
        }
    }
}

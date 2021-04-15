using NTMiner.Controllers;
using System;

namespace NTMiner.Ws {
    public class DaemonWsClient : AbstractWsClient {
        public DaemonWsClient() : base(NTMinerAppType.MinerClient) {
            VirtualRoot.BuildEventPath<Per2MinuteEvent>("每2分钟通过Ws通道上报一次算力", LogEnum.DevConsole, typeof(VirtualRoot), PathPriority.Normal, path: message => {
                if (!IsOpen) {
                    return;
                }
                if (!VirtualRoot.DaemonOperation.IsNTMinerOpened()) {
                    return;
                }
                RpcRoot.Client.MinerClientService.WsGetSpeedAsync((data, ex) => {
                    // Ws通道未打开，或者未从本机MinerClient获取到算力（往往因为挖矿端没运行，所以服务不存在，当然获取不到算力）
                    if (!IsOpen || ex != null) {
                        return;
                    }
                    SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.Speed) {
                        Data = data
                    });
                });
            });
        }

        protected override bool TryGetHandler(string messageType, out Action<Action<WsMessage>, WsMessage> handler) {
            return DaemonWsMessageFromWsServerHandler.TryGetHandler(messageType, out handler);
        }

        protected override void UpdateWsStateAsync(string description, bool toOut) {
            if (VirtualRoot.DaemonOperation.IsNTMinerOpened()) {
                var state = base.GetState();
                if (!string.IsNullOrEmpty(description)) {
                    state.Description = description;
                }
                state.ToOut = toOut;
                RpcRoot.JsonRpc.FirePostAsync(NTKeyword.Localhost, NTKeyword.MinerClientPort, ControllerUtil.GetControllerName<IMinerClientController>(), nameof(IMinerClientController.ReportWsDaemonState), null, state, timeountMilliseconds: 3000);
            }
        }
    }
}

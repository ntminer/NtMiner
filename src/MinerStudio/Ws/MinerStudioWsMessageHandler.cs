using NTMiner.Core;
using NTMiner.Core.MinerClient;
using NTMiner.JsonDb;
using NTMiner.MinerStudio;
using NTMiner.VirtualMemory;
using System;
using System.Collections.Generic;

namespace NTMiner.Ws {
    public static class MinerStudioWsMessageHandler {
        private static readonly Dictionary<string, Action<Action<WsMessage>, WsMessage>> 
            _handlers = new Dictionary<string, Action<Action<WsMessage>, WsMessage>>(StringComparer.OrdinalIgnoreCase);
        public static bool TryGetHandler(string messageType, out Action<Action<WsMessage>, WsMessage> handler) {
            return _handlers.TryGetValue(messageType, out handler);
        }

        static MinerStudioWsMessageHandler() {
            _handlers.Add(WsMessage.ConsoleOutLines, (sendAsync, message) => {
                if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out List<ConsoleOutLine> data)) {
                    VirtualRoot.RaiseEvent(new ClientConsoleOutLinesEvent(wrapperClientIdData.ClientId, data));
                }
            });
            _handlers.Add(WsMessage.LocalMessages, (sendAsync, message) => {
                if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out List<LocalMessageDto> data)) {
                    VirtualRoot.RaiseEvent(new ClientLocalMessagesEvent(wrapperClientIdData.ClientId, data));
                }
            });
            _handlers.Add(WsMessage.Drives, (sendAsync, message) => {
                if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out List<DriveDto> data)) {
                    VirtualRoot.RaiseEvent(new GetDrivesResponsedEvent(wrapperClientIdData.ClientId, data));
                }
            });
            _handlers.Add(WsMessage.LocalIps, (sendAsync, message) => {
                if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out List<LocalIpDto> data)) {
                    VirtualRoot.RaiseEvent(new GetLocalIpsResponsedEvent(wrapperClientIdData.ClientId, data));
                }
            });
            _handlers.Add(WsMessage.OperationResults, (sendAsync, message) => {
                if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out List<OperationResultData> data)) {
                    VirtualRoot.RaiseEvent(new ClientOperationResultsEvent(wrapperClientIdData.ClientId, data));
                }
            });
            _handlers.Add(WsMessage.OperationReceived, (sendAsync, message) => {
                if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                    VirtualRoot.RaiseEvent(new ClientOperationReceivedEvent(wrapperClientId.ClientId));
                }
            });
            _handlers.Add(WsMessage.LocalJson, (sendAsync, message) => {
                if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out string json)) {
                    LocalJsonDb data = VirtualRoot.JsonSerializer.Deserialize<LocalJsonDb>(json) ?? new LocalJsonDb();
                    VirtualRoot.RaiseEvent(new GetLocalJsonResponsedEvent(wrapperClientIdData.ClientId, data));
                }
            });
            _handlers.Add(WsMessage.GpuProfilesJson, (sendAsync, message) => {
                if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out string json)) {
                    GpuProfilesJsonDb data = VirtualRoot.JsonSerializer.Deserialize<GpuProfilesJsonDb>(json) ?? new GpuProfilesJsonDb();
                    VirtualRoot.RaiseEvent(new GetGpuProfilesResponsedEvent(wrapperClientIdData.ClientId, data));
                }
            });
        }
    }
}

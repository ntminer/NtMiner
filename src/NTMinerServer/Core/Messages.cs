using NTMiner.Core.Daemon;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using NTMiner.Core.Mq;
using NTMiner.Cryptography;
using NTMiner.Hub;
using NTMiner.VirtualMemory;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    #region abstract classes
    public abstract class WsServerNodeMqEvent : EventBase {
        public WsServerNodeMqEvent(string appId, string wsServerAddress) {
            this.AppId = appId;
            this.WsServerAddress = wsServerAddress;
        }

        public string AppId { get; private set; }
        public string WsServerAddress { get; private set; }
    }

    public abstract class UserMqEvent : EventBase {
        public UserMqEvent(string appId, string loginName, DateTime timestamp) {
            this.AppId = appId;
            this.LoginName = loginName;
            this.Timestamp = timestamp;
        }

        public string AppId { get; private set; }
        public string LoginName { get; private set; }
        public DateTime Timestamp { get; private set; }
    }

    public abstract class MinerClientMqEvent : EventBase {
        public MinerClientMqEvent(string appId, Guid clientId, DateTime timestamp) {
            this.AppId = appId;
            this.Timestamp = timestamp;
            this.ClientId = clientId;
        }

        public string AppId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Guid ClientId { get; private set; }
    }

    public abstract class MinerDataMqEvent : EventBase {
        public MinerDataMqEvent(string appId, string minerId, Guid clientId, DateTime timestamp) {
            this.AppId = appId;
            this.MinerId = minerId;
            this.ClientId = clientId;
            this.Timestamp = timestamp;
        }

        public string AppId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string MinerId { get; private set; }
        public Guid ClientId { get; private set; }
    }

    public abstract class OperationMqEvent : EventBase {
        public OperationMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId) {
            this.AppId = appId;
            this.LoginName = loginName;
            this.Timestamp = timestamp;
            this.ClientId = clientId;
        }

        public string AppId { get; private set; }
        public string LoginName { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Guid ClientId { get; private set; }
    }

    public abstract class OperationMqEvent<T> : OperationMqEvent {
        public OperationMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, T data)
            : base(appId, loginName, timestamp, clientId) {
            this.Data = data;
        }

        public T Data { get; private set; }
    }
    #endregion

    [MessageType(description: "收到了WsServerNodeAdded Mq消息后")]
    public class WsServerNodeAddedMqEvent : WsServerNodeMqEvent {
        public WsServerNodeAddedMqEvent(string appId, string wsServerAddress) : base(appId, wsServerAddress) {
        }
    }

    [MessageType(description: "收到了WsServerNodeRemoved Mq消息后")]
    public class WsServerNodeRemovedMqEvent : WsServerNodeMqEvent {
        public WsServerNodeRemovedMqEvent(string appId, string wsServerAddress) : base(appId, wsServerAddress) {
        }
    }

    [MessageType(description: "收到了UpdateUserRSAKey Mq消息后，注意该消息是命令")]
    public class UpdateUserRSAKeyMqCommand : Cmd {
        public UpdateUserRSAKeyMqCommand(string appId, string loginName, DateTime timestamp, RSAKey key) {
            this.AppId = appId;
            this.LoginName = loginName;
            this.Timestamp = timestamp;
            this.Key = key;
        }

        public string AppId { get; private set; }
        public string LoginName { get; private set; }
        public DateTime Timestamp { get; private set; }
        public RSAKey Key { get; private set; }
    }

    [MessageType(description: "收到了UserRSAKeyUpdated Mq消息后")]
    public class UserRSAKeyUpdatedMqEvent : UserMqEvent {
        public UserRSAKeyUpdatedMqEvent(string appId, string loginName, DateTime timestamp) : base(appId, loginName, timestamp) {
        }
    }

    [MessageType(description: "收到了UserAdded Mq消息后")]
    public class UserAddedMqEvent : UserMqEvent {
        public UserAddedMqEvent(string appId, string loginName, DateTime timestamp) : base(appId, loginName, timestamp) {
        }
    }

    [MessageType(description: "收到了UserUpdated Mq消息后")]
    public class UserUpdatedMqEvent : UserMqEvent {
        public UserUpdatedMqEvent(string appId, string loginName, DateTime timestamp) : base(appId, loginName, timestamp) {
        }
    }

    [MessageType(description: "收到了UserRemoved Mq消息后")]
    public class UserRemovedMqEvent : UserMqEvent {
        public UserRemovedMqEvent(string appId, string loginName, DateTime timestamp) : base(appId, loginName, timestamp) {
        }
    }

    [MessageType(description: "收到了UserEnabled Mq消息后")]
    public class UserEnabledMqEvent : UserMqEvent {
        public UserEnabledMqEvent(string appId, string loginName, DateTime timestamp) : base(appId, loginName, timestamp) {
        }
    }

    [MessageType(description: "收到了UserDisabled Mq消息后")]
    public class UserDisabledMqEvent : UserMqEvent {
        public UserDisabledMqEvent(string appId, string loginName, DateTime timestamp) : base(appId, loginName, timestamp) {
        }
    }

    [MessageType(description: "收到了UserPasswordChanged Mq消息后")]
    public class UserPasswordChangedMqEvent : UserMqEvent {
        public UserPasswordChangedMqEvent(string appId, string loginName, DateTime timestamp) : base(appId, loginName, timestamp) {
        }
    }

    [MessageType(description: "收到了SpeedDatas Mq消息后")]
    public class SpeedDatasMqEvent : EventBase {
        public SpeedDatasMqEvent(string appId, ClientIdIp[] clientIdIps, DateTime timestamp) {
            this.AppId = appId;
            this.ClientIdIps = clientIdIps;
            this.Timestamp = timestamp;
        }

        public string AppId { get; private set; }
        public ClientIdIp[] ClientIdIps { get; private set; }
        public DateTime Timestamp { get; private set; }
    }

    [MessageType(description: "收到了MinerClientWsOpened Mq消息后")]
    public class MinerClientWsOpenedMqEvent : MinerClientMqEvent {
        public MinerClientWsOpenedMqEvent(string appId, Guid clientId, DateTime timestamp) : base(appId, clientId, timestamp) {
        }
    }

    [MessageType(description: "收到了MinerClientWsClosed Mq消息后")]
    public class MinerClientWsClosedMqEvent : MinerClientMqEvent {
        public MinerClientWsClosedMqEvent(string appId, Guid clientId, DateTime timestamp) : base(appId, clientId, timestamp) {
        }
    }

    [MessageType(description: "收到了MinerClientsWsBreathed Mq消息后")]
    public class MinerClientsWsBreathedMqEvent : EventBase {
        public MinerClientsWsBreathedMqEvent(string appId, Guid[] clientIds, DateTime timestamp) {
            this.AppId = appId;
            this.ClientIds = clientIds;
            this.Timestamp = timestamp;
        }

        public string AppId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Guid[] ClientIds { get; private set; }
    }

    [MessageType(description: "收到了ChangeMinerSign Mq消息后，该消息是个命令")]
    public class ChangeMinerSignMqCommand : Cmd {
        public ChangeMinerSignMqCommand(string appId, MinerSign data) {
            this.AppId = appId;
            this.Data = data;
        }

        public string AppId { get; private set; }
        public MinerSign Data { get; private set; }
    }

    [MessageType(description: "收到了QueryClientsForWs Mq消息后，该消息是个命令")]
    public class QueryClientsForWsMqCommand : Cmd {
        public QueryClientsForWsMqCommand(
            string appId, string mqMessageId, DateTime timestamp, string loginName,
            string sessionId, QueryClientsForWsRequest query) {
            this.AppId = appId;
            this.MqMessageId = mqMessageId;
            this.Timestamp = timestamp;
            this.LoginName = loginName;
            this.SessionId = sessionId;
            this.Query = query;
        }

        public string AppId { get; private set; }
        public string MqMessageId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string LoginName { get; private set; }
        public string SessionId { get; private set; }
        public QueryClientsForWsRequest Query { get; private set; }
    }

    [MessageType(description: "收到了QueryClientsForWsResponse Mq消息后，该消息是个命令")]
    public class QueryClientsForWsResponseMqEvent : EventBase {
        public QueryClientsForWsResponseMqEvent(
            string appId, string mqCorrelationId, DateTime timestamp, string loginName,
            string sessionId, QueryClientsResponse response) {
            this.AppId = appId;
            this.MqCorrelationId = mqCorrelationId;
            this.Timestamp = timestamp;
            this.LoginName = loginName;
            this.SessionId = sessionId;
            this.Response = response;
        }

        public string AppId { get; private set; }
        public string MqCorrelationId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string LoginName { get; private set; }
        public string SessionId { get; private set; }
        public QueryClientsResponse Response { get; private set; }
    }

    [MessageType(description: "收到了MinerDataAdded Mq消息后")]
    public class MinerDataAddedMqEvent : MinerDataMqEvent {
        public MinerDataAddedMqEvent(string appId, string minerId, Guid clientId, DateTime timestamp) : base(appId, minerId, clientId, timestamp) {
        }
    }

    [MessageType(description: "收到了MinerDataRemoved Mq消息后")]
    public class MinerDataRemovedMqEvent : MinerDataMqEvent {
        public MinerDataRemovedMqEvent(string appId, string minerId, Guid clientId, DateTime timestamp) : base(appId, minerId, clientId, timestamp) {
        }
    }

    [MessageType(description: "收到了MinerSignChanged Mq消息后")]
    public class MinerSignChangedMqEvent : MinerDataMqEvent {
        public MinerSignChangedMqEvent(string appId, string minerId, Guid clientId, DateTime timestamp) : base(appId, minerId, clientId, timestamp) {
        }
    }

    [MessageType(description: "收到了GetSpeed Mq消息后")]
    public class GetSpeedMqEvent : EventBase {
        public GetSpeedMqEvent(string appId, DateTime timestamp, UserGetSpeedData[] datas) {
            this.AppId = appId;
            this.Timestamp = timestamp;
            this.Datas = datas;
        }

        public string AppId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public UserGetSpeedData[] Datas { get; private set; }
    }

    [MessageType(description: "收到了GetConsoleOutLines Mq消息后")]
    public class GetConsoleOutLinesMqEvent : OperationMqEvent<long> {
        public GetConsoleOutLinesMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, long afterTime)
            : base(appId, loginName, timestamp, clientId, afterTime) {
        }
    }

    [MessageType(description: "收到了ConsoleOutLines Mq消息后")]
    public class ConsoleOutLinesMqEvent : OperationMqEvent<List<ConsoleOutLine>> {
        public ConsoleOutLinesMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, List<ConsoleOutLine> data)
            : base(appId, loginName, timestamp, clientId, data) {
        }
    }

    [MessageType(description: "收到了GetLocalMessages Mq消息后")]
    public class GetLocalMessagesMqEvent : OperationMqEvent<long> {
        public GetLocalMessagesMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, long afterTime)
            : base(appId, loginName, timestamp, clientId, afterTime) {
        }
    }

    [MessageType(description: "收到了LocalMessages Mq消息后")]
    public class LocalMessagesMqEvent : OperationMqEvent<List<LocalMessageDto>> {
        public LocalMessagesMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, List<LocalMessageDto> data)
            : base(appId, loginName, timestamp, clientId, data) {
        }
    }

    [MessageType(description: "收到了GetDrives Mq消息后")]
    public class GetDrivesMqEvent : OperationMqEvent {
        public GetDrivesMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了Drives Mq消息后")]
    public class DrivesMqEvent : OperationMqEvent<List<DriveDto>> {
        public DrivesMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, List<DriveDto> data)
            : base(appId, loginName, timestamp, clientId, data) {
        }
    }

    [MessageType(description: "收到了GetLocalIps Mq消息后")]
    public class GetLocalIpsMqEvent : OperationMqEvent {
        public GetLocalIpsMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了LocalIps Mq消息后")]
    public class LocalIpsMqEvent : OperationMqEvent<List<LocalIpDto>> {
        public LocalIpsMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, List<LocalIpDto> data)
            : base(appId, loginName, timestamp, clientId, data) {
        }
    }

    [MessageType(description: "收到了OperationResults Mq消息后")]
    public class GetOperationResultsMqEvent : OperationMqEvent<long> {
        public GetOperationResultsMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, long afterTime)
            : base(appId, loginName, timestamp, clientId, afterTime) {
        }
    }

    [MessageType(description: "收到了OperationResults Mq消息后")]
    public class OperationResultsMqEvent : OperationMqEvent<List<OperationResultData>> {
        public OperationResultsMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, List<OperationResultData> data)
            : base(appId, loginName, timestamp, clientId, data) {
        }
    }

    [MessageType(description: "收到了OperationReceived Mq消息后")]
    public class OperationReceivedMqEvent : OperationMqEvent {
        public OperationReceivedMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了EnableRemoteDesktop Mq消息后")]
    public class EnableRemoteDesktopMqEvent : OperationMqEvent {
        public EnableRemoteDesktopMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了BlockWAU Mq消息后")]
    public class BlockWAUMqEvent : OperationMqEvent {
        public BlockWAUMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了SetVirtualMemory Mq消息后")]
    public class SetVirtualMemoryMqEvent : OperationMqEvent<Dictionary<string, int>> {
        public SetVirtualMemoryMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, Dictionary<string, int> data)
            : base(appId, loginName, timestamp, clientId, data) {
        }
    }

    [MessageType(description: "收到了SetLocalIps Mq消息后")]
    public class SetLocalIpsMqEvent : OperationMqEvent<List<LocalIpInput>> {
        public SetLocalIpsMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, List<LocalIpInput> data)
            : base(appId, loginName, timestamp, clientId, data) {
        }
    }

    [MessageType(description: "收到了SwitchRadeonGpu Mq消息后")]
    public class SwitchRadeonGpuMqEvent : OperationMqEvent {
        public SwitchRadeonGpuMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, bool on)
            : base(appId, loginName, timestamp, clientId) {
            this.On = on;
        }

        public bool On { get; private set; }
    }

    [MessageType(description: "收到了GetSelfWorkLocalJson Mq消息后")]
    public class GetSelfWorkLocalJsonMqEvent : OperationMqEvent {
        public GetSelfWorkLocalJsonMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了LocalJson Mq消息后")]
    public class LocalJsonMqEvent : OperationMqEvent<string> {
        public LocalJsonMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, string json)
            : base(appId, loginName, timestamp, clientId, json) {
        }
    }

    [MessageType(description: "收到了GetGpuProfilesJson Mq消息后")]
    public class GetGpuProfilesJsonMqEvent : OperationMqEvent {
        public GetGpuProfilesJsonMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了SaveSelfWorkLocalJson Mq消息后")]
    public class SaveSelfWorkLocalJsonMqEvent : OperationMqEvent<WorkRequest> {
        public SaveSelfWorkLocalJsonMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, WorkRequest request)
            : base(appId, loginName, timestamp, clientId, request) {
        }
    }

    [MessageType(description: "收到了GpuProfilesJson Mq消息后")]
    public class GpuProfilesJsonMqEvent : OperationMqEvent<string> {
        public GpuProfilesJsonMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, string json)
            : base(appId, loginName, timestamp, clientId, json) {
        }
    }

    [MessageType(description: "收到了SaveGpuProfilesJson Mq消息后")]
    public class SaveGpuProfilesJsonMqEvent : OperationMqEvent<string> {
        public SaveGpuProfilesJsonMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, string json)
            : base(appId, loginName, timestamp, clientId, json) {
        }
    }

    [MessageType(description: "收到了SetAutoBootStart Mq消息后")]
    public class SetAutoBootStartMqEvent : OperationMqEvent<SetAutoBootStartRequest> {
        public SetAutoBootStartMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, SetAutoBootStartRequest body)
            : base(appId, loginName, timestamp, clientId, body) {
        }
    }

    [MessageType(description: "收到了RestartWindows Mq消息后")]
    public class RestartWindowsMqEvent : OperationMqEvent {
        public RestartWindowsMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了ShutdownWindows Mq消息后")]
    public class ShutdownWindowsMqEvent : OperationMqEvent {
        public ShutdownWindowsMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了UpgradeNTMiner Mq消息后")]
    public class UpgradeNTMinerMqEvent : OperationMqEvent<string> {
        public UpgradeNTMinerMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, string ntminerFileName)
            : base(appId, loginName, timestamp, clientId, ntminerFileName) {
        }
    }

    [MessageType(description: "收到了StartMine Mq消息后")]
    public class StartMineMqEvent : OperationMqEvent<Guid> {
        public StartMineMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, Guid workId)
            : base(appId, loginName, timestamp, clientId, workId) {
        }
    }

    [MessageType(description: "收到了StartWorkMine Mq消息后")]
    public class StartWorkMineMqEvent : OperationMqEvent<WorkRequest> {
        public StartWorkMineMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId, WorkRequest workRequest)
            : base(appId, loginName, timestamp, clientId, workRequest) {
        }
    }

    [MessageType(description: "收到了StopMine Mq消息后")]
    public class StopMineMqEvent : OperationMqEvent {
        public StopMineMqEvent(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }
}

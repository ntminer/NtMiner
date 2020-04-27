using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using NTMiner.Hub;
using NTMiner.Report;
using NTMiner.VirtualMemory;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    #region abstract classes
    public abstract class WsServerNodeMqMessage : EventBase {
        public WsServerNodeMqMessage(string appId, string wsServerAddress) {
            this.AppId = appId;
            this.WsServerAddress = wsServerAddress;
        }

        public string AppId { get; private set; }
        public string WsServerAddress { get; private set; }
    }

    public abstract class UserMqMessage : EventBase {
        public UserMqMessage(string appId, string loginName, DateTime timestamp) {
            this.AppId = appId;
            this.LoginName = loginName;
            this.Timestamp = timestamp;
        }

        public string AppId { get; private set; }
        public string LoginName { get; private set; }
        public DateTime Timestamp { get; private set; }
    }

    public abstract class MinerClientMqMessage : EventBase {
        public MinerClientMqMessage(string appId, Guid clientId, DateTime timestamp) {
            this.AppId = appId;
            this.Timestamp = timestamp;
            this.ClientId = clientId;
        }

        public string AppId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Guid ClientId { get; private set; }
    }

    public abstract class MinerDataMqMessage : EventBase {
        public MinerDataMqMessage(string appId, string minerId, DateTime timestamp) {
            this.AppId = appId;
            this.Timestamp = timestamp;
            this.MinerId = minerId;
        }

        public string AppId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string MinerId { get; private set; }
    }

    public abstract class OperationMqMessage : EventBase {
        public OperationMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId) {
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

    public abstract class OperationMqMessage<T> : OperationMqMessage {
        public OperationMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, T data)
            : base(appId, loginName, timestamp, clientId) {
            this.Data = data;
        }

        public T Data { get; private set; }
    }
    #endregion

    [MessageType(description: "收到了WsServerNodeAdded Mq消息后")]
    public class WsServerNodeAddedMqMessage : WsServerNodeMqMessage {
        public WsServerNodeAddedMqMessage(string appId, string wsServerAddress) : base(appId, wsServerAddress) {
        }
    }

    [MessageType(description: "收到了WsServerNodeRemoved Mq消息后")]
    public class WsServerNodeRemovedMqMessage : WsServerNodeMqMessage {
        public WsServerNodeRemovedMqMessage(string appId, string wsServerAddress) : base(appId, wsServerAddress) {
        }
    }

    [MessageType(description: "收到了UpdateUserRSAKey Mq消息后，注意该消息是命令")]
    public class UpdateUserRSAKeyMqMessage : Cmd {
        public UpdateUserRSAKeyMqMessage(string appId, string loginName, DateTime timestamp, Cryptography.RSAKey key) {
            this.AppId = appId;
            this.LoginName = loginName;
            this.Timestamp = timestamp;
            this.Key = key;
        }

        public string AppId { get; private set; }
        public string LoginName { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Cryptography.RSAKey Key { get; private set; }
    }

    [MessageType(description: "收到了UserRSAKeyUpdated Mq消息后")]
    public class UserRSAKeyUpdatedMqMessage : UserMqMessage {
        public UserRSAKeyUpdatedMqMessage(string appId, string loginName, DateTime timestamp) : base(appId, loginName, timestamp) {
        }
    }

    [MessageType(description: "收到了UserAdded Mq消息后")]
    public class UserAddedMqMessage : UserMqMessage {
        public UserAddedMqMessage(string appId, string loginName, DateTime timestamp) : base(appId, loginName, timestamp) {
        }
    }

    [MessageType(description: "收到了UserUpdated Mq消息后")]
    public class UserUpdatedMqMessage : UserMqMessage {
        public UserUpdatedMqMessage(string appId, string loginName, DateTime timestamp) : base(appId, loginName, timestamp) {
        }
    }

    [MessageType(description: "收到了UserRemoved Mq消息后")]
    public class UserRemovedMqMessage : UserMqMessage {
        public UserRemovedMqMessage(string appId, string loginName, DateTime timestamp) : base(appId, loginName, timestamp) {
        }
    }

    [MessageType(description: "收到了UserEnabled Mq消息后")]
    public class UserEnabledMqMessage : UserMqMessage {
        public UserEnabledMqMessage(string appId, string loginName, DateTime timestamp) : base(appId, loginName, timestamp) {
        }
    }

    [MessageType(description: "收到了UserDisabled Mq消息后")]
    public class UserDisabledMqMessage : UserMqMessage {
        public UserDisabledMqMessage(string appId, string loginName, DateTime timestamp) : base(appId, loginName, timestamp) {
        }
    }

    [MessageType(description: "收到了UserPasswordChanged Mq消息后")]
    public class UserPasswordChangedMqMessage : UserMqMessage {
        public UserPasswordChangedMqMessage(string appId, string loginName, DateTime timestamp) : base(appId, loginName, timestamp) {
        }
    }

    [MessageType(description: "收到了SpeedData Mq消息后")]
    public class SpeedDataMqMessage : MinerClientMqMessage {
        public SpeedDataMqMessage(string appId, Guid clientId, string minerIp, DateTime timestamp) : base(appId, clientId, timestamp) {
            this.MinerIp = minerIp;
        }

        public string MinerIp { get; private set; }
    }

    [MessageType(description: "收到了MinerClientWsOpened Mq消息后")]
    public class MinerClientWsOpenedMqMessage : MinerClientMqMessage {
        public MinerClientWsOpenedMqMessage(string appId, Guid clientId, DateTime timestamp) : base(appId, clientId, timestamp) {
        }
    }

    [MessageType(description: "收到了MinerClientWsClosed Mq消息后")]
    public class MinerClientWsClosedMqMessage : MinerClientMqMessage {
        public MinerClientWsClosedMqMessage(string appId, Guid clientId, DateTime timestamp) : base(appId, clientId, timestamp) {
        }
    }

    [MessageType(description: "收到了MinerClientWsBreathed Mq消息后")]
    public class MinerClientWsBreathedMqMessage : MinerClientMqMessage {
        public MinerClientWsBreathedMqMessage(string appId, Guid clientId, DateTime timestamp) : base(appId, clientId, timestamp) {
        }
    }

    [MessageType(description: "收到了ChangeMinerSign Mq消息后，该消息是个命令")]
    public class ChangeMinerSignMqMessage : Cmd {
        public ChangeMinerSignMqMessage(MinerSign data) {
            this.Data = data;
        }

        public MinerSign Data { get; private set; }
    }

    [MessageType(description: "收到了MinerDataAdded Mq消息后")]
    public class MinerDataAddedMqMessage : MinerDataMqMessage {
        public MinerDataAddedMqMessage(string appId, string minerId, DateTime timestamp) : base(appId, minerId, timestamp) {
        }
    }

    [MessageType(description: "收到了MinerDataRemoved Mq消息后")]
    public class MinerDataRemovedMqMessage : MinerDataMqMessage {
        public MinerDataRemovedMqMessage(string appId, string minerId, DateTime timestamp) : base(appId, minerId, timestamp) {
        }
    }

    [MessageType(description: "收到了MinerSignChanged Mq消息后")]
    public class MinerSignChangedMqMessage : MinerDataMqMessage {
        public MinerSignChangedMqMessage(string appId, string minerId, DateTime timestamp) : base(appId, minerId, timestamp) {
        }
    }

    [MessageType(description: "收到了GetSpeed Mq消息后")]
    public class GetSpeedMqMessage : EventBase {
        public GetSpeedMqMessage(string appId, string loginName, DateTime timestamp, List<Guid> clientIds) {
            this.AppId = appId;
            this.LoginName = loginName;
            this.Timestamp = timestamp;
            this.ClientIds = clientIds;
        }

        public string AppId { get; private set; }
        public string LoginName { get; private set; }
        public DateTime Timestamp { get; private set; }
        public List<Guid> ClientIds { get; private set; }
    }

    [MessageType(description: "收到了GetConsoleOutLines Mq消息后")]
    public class GetConsoleOutLinesMqMessage : OperationMqMessage<long> {
        public GetConsoleOutLinesMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, long afterTime)
            : base(appId, loginName, timestamp, clientId, afterTime) {
        }
    }

    [MessageType(description: "收到了ConsoleOutLines Mq消息后")]
    public class ConsoleOutLinesMqMessage : OperationMqMessage<List<ConsoleOutLine>> {
        public ConsoleOutLinesMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, List<ConsoleOutLine> data)
            : base(appId, loginName, timestamp, clientId, data) {
        }
    }

    [MessageType(description: "收到了GetLocalMessages Mq消息后")]
    public class GetLocalMessagesMqMessage : OperationMqMessage<long> {
        public GetLocalMessagesMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, long afterTime)
            : base(appId, loginName, timestamp, clientId, afterTime) {
        }
    }

    [MessageType(description: "收到了LocalMessages Mq消息后")]
    public class LocalMessagesMqMessage : OperationMqMessage<List<LocalMessageDto>> {
        public LocalMessagesMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, List<LocalMessageDto> data)
            : base(appId, loginName, timestamp, clientId, data) {
        }
    }

    [MessageType(description: "收到了GetDrives Mq消息后")]
    public class GetDrivesMqMessage : OperationMqMessage {
        public GetDrivesMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了Drives Mq消息后")]
    public class DrivesMqMessage : OperationMqMessage<List<DriveDto>> {
        public DrivesMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, List<DriveDto> data)
            : base(appId, loginName, timestamp, clientId, data) {
        }
    }

    [MessageType(description: "收到了GetLocalIps Mq消息后")]
    public class GetLocalIpsMqMessage : OperationMqMessage {
        public GetLocalIpsMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了LocalIps Mq消息后")]
    public class LocalIpsMqMessage : OperationMqMessage<List<LocalIpDto>> {
        public LocalIpsMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, List<LocalIpDto> data)
            : base(appId, loginName, timestamp, clientId, data) {
        }
    }

    [MessageType(description: "收到了OperationResults Mq消息后")]
    public class GetOperationResultsMqMessage : OperationMqMessage<long> {
        public GetOperationResultsMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, long afterTime)
            : base(appId, loginName, timestamp, clientId, afterTime) {
        }
    }

    [MessageType(description: "收到了OperationResults Mq消息后")]
    public class OperationResultsMqMessage : OperationMqMessage<List<OperationResultData>> {
        public OperationResultsMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, List<OperationResultData> data)
            : base(appId, loginName, timestamp, clientId, data) {
        }
    }

    [MessageType(description: "收到了OperationReceived Mq消息后")]
    public class OperationReceivedMqMessage : OperationMqMessage {
        public OperationReceivedMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了EnableRemoteDesktop Mq消息后")]
    public class EnableRemoteDesktopMqMessage : OperationMqMessage {
        public EnableRemoteDesktopMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了BlockWAU Mq消息后")]
    public class BlockWAUMqMessage : OperationMqMessage {
        public BlockWAUMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了SetVirtualMemory Mq消息后")]
    public class SetVirtualMemoryMqMessage : OperationMqMessage<Dictionary<string, int>> {
        public SetVirtualMemoryMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, Dictionary<string, int> data)
            : base(appId, loginName, timestamp, clientId, data) {
        }
    }

    [MessageType(description: "收到了SetLocalIps Mq消息后")]
    public class SetLocalIpsMqMessage : OperationMqMessage<List<LocalIpInput>> {
        public SetLocalIpsMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, List<LocalIpInput> data)
            : base(appId, loginName, timestamp, clientId, data) {
        }
    }

    [MessageType(description: "收到了AtikmdagPatcher Mq消息后")]
    public class AtikmdagPatcherMqMessage : OperationMqMessage {
        public AtikmdagPatcherMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了SwitchRadeonGpu Mq消息后")]
    public class SwitchRadeonGpuMqMessage : OperationMqMessage {
        public SwitchRadeonGpuMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, bool on)
            : base(appId, loginName, timestamp, clientId) {
            this.On = on;
        }

        public bool On { get; private set; }
    }

    [MessageType(description: "收到了GetSelfWorkLocalJson Mq消息后")]
    public class GetSelfWorkLocalJsonMqMessage : OperationMqMessage {
        public GetSelfWorkLocalJsonMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了LocalJson Mq消息后")]
    public class LocalJsonMqMessage : OperationMqMessage<string> {
        public LocalJsonMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, string json)
            : base(appId, loginName, timestamp, clientId, json) {
        }
    }

    [MessageType(description: "收到了GetGpuProfilesJson Mq消息后")]
    public class GetGpuProfilesJsonMqMessage : OperationMqMessage {
        public GetGpuProfilesJsonMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了GpuProfilesJson Mq消息后")]
    public class GpuProfilesJsonMqMessage : OperationMqMessage<string> {
        public GpuProfilesJsonMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, string json)
            : base(appId, loginName, timestamp, clientId, json) {
        }
    }

    [MessageType(description: "收到了SaveGpuProfilesJson Mq消息后")]
    public class SaveGpuProfilesJsonMqMessage : OperationMqMessage<string> {
        public SaveGpuProfilesJsonMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, string json)
            : base(appId, loginName, timestamp, clientId, json) {
        }
    }

    [MessageType(description: "收到了SetAutoBootStart Mq消息后")]
    public class SetAutoBootStartMqMessage : OperationMqMessage<SetAutoBootStartRequest> {
        public SetAutoBootStartMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, SetAutoBootStartRequest body)
            : base(appId, loginName, timestamp, clientId, body) {
        }
    }

    [MessageType(description: "收到了RestartWindows Mq消息后")]
    public class RestartWindowsMqMessage : OperationMqMessage {
        public RestartWindowsMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了ShutdownWindows Mq消息后")]
    public class ShutdownWindowsMqMessage : OperationMqMessage {
        public ShutdownWindowsMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }

    [MessageType(description: "收到了UpgradeNTMiner Mq消息后")]
    public class UpgradeNTMinerMqMessage : OperationMqMessage<string> {
        public UpgradeNTMinerMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, string ntminerFileName)
            : base(appId, loginName, timestamp, clientId, ntminerFileName) {
        }
    }

    [MessageType(description: "收到了StartMine Mq消息后")]
    public class StartMineMqMessage : OperationMqMessage<Guid> {
        public StartMineMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId, Guid workId)
            : base(appId, loginName, timestamp, clientId, workId) {
        }
    }

    [MessageType(description: "收到了StopMine Mq消息后")]
    public class StopMineMqMessage : OperationMqMessage {
        public StopMineMqMessage(string appId, string loginName, DateTime timestamp, Guid clientId)
            : base(appId, loginName, timestamp, clientId) {
        }
    }
}

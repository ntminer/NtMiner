using NTMiner.Core.Daemon;
using NTMiner.Core.MinerServer;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class OperationMqMessagePath : AbstractMqMessagePath<UserSetInitedEvent, MinerSignSetInitedEvent> {
        public OperationMqMessagePath(string queue) : base(queue) {
        }

        protected override Dictionary<string, Action<BasicDeliverEventArgs>> GetPaths() {
            return new Dictionary<string, Action<BasicDeliverEventArgs>> {
                [WsMqKeyword.GetConsoleOutLinesRoutingKey] = ea => {
                    string appId = ea.BasicProperties.AppId;
                    AfterTimeRequest[] requests = OperationMqBodyUtil.GetAfterTimeRequestMqReceiveBody(ea.Body);
                    if (requests != null && requests.Length != 0) {
                        foreach (var request in requests) {
                            VirtualRoot.RaiseEvent(new GetConsoleOutLinesMqEvent(appId, request.LoginName, ea.GetTimestamp(), request.ClientId, request.AfterTime));
                        }
                    }
                },
                [WsMqKeyword.FastGetConsoleOutLinesRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    long afterTimestamp = OperationMqBodyUtil.GetFastGetConsoleOutLinesMqReceiveBody(ea.Body);
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new GetConsoleOutLinesMqEvent(appId, loginName, ea.GetTimestamp(), clientId, afterTimestamp));
                    }
                },
                [WsMqKeyword.ConsoleOutLinesRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    var data = OperationMqBodyUtil.GetConsoleOutLinesMqReceiveBody(ea.Body);
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new ConsoleOutLinesMqEvent(appId, loginName, ea.GetTimestamp(), clientId, data));
                    }
                },
                [WsMqKeyword.GetLocalMessagesRoutingKey] = ea => {
                    string appId = ea.BasicProperties.AppId;
                    AfterTimeRequest[] requests = OperationMqBodyUtil.GetAfterTimeRequestMqReceiveBody(ea.Body);
                    if (requests != null && requests.Length != 0) {
                        foreach (var request in requests) {
                            VirtualRoot.RaiseEvent(new GetLocalMessagesMqEvent(appId, request.LoginName, ea.GetTimestamp(), request.ClientId, request.AfterTime));
                        }
                    }
                },
                [WsMqKeyword.FastGetLocalMessagesRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    long afterTimestamp = OperationMqBodyUtil.GetFastGetLocalMessagesMqReceiveBody(ea.Body);
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new GetLocalMessagesMqEvent(appId, loginName, ea.GetTimestamp(), clientId, afterTimestamp));
                    }
                },
                [WsMqKeyword.LocalMessagesRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    var data = OperationMqBodyUtil.GetLocalMessagesMqReceiveBody(ea.Body);
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new LocalMessagesMqEvent(appId, loginName, ea.GetTimestamp(), clientId, data));
                    }
                },
                [WsMqKeyword.GetOperationResultsRoutingKey] = ea => {
                    string appId = ea.BasicProperties.AppId;
                    AfterTimeRequest[] requests = OperationMqBodyUtil.GetAfterTimeRequestMqReceiveBody(ea.Body);
                    if (requests != null && requests.Length != 0) {
                        foreach (var request in requests) {
                            VirtualRoot.RaiseEvent(new GetOperationResultsMqEvent(appId, request.LoginName, ea.GetTimestamp(), request.ClientId, request.AfterTime));
                        }
                    }
                },
                [WsMqKeyword.FastGetOperationResultsRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    long afterTimestamp = OperationMqBodyUtil.GetFastGetOperationResultsMqReceiveBody(ea.Body);
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new GetOperationResultsMqEvent(appId, loginName, ea.GetTimestamp(), clientId, afterTimestamp));
                    }
                },
                [WsMqKeyword.GetDrivesRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new GetDrivesMqEvent(appId, loginName, ea.GetTimestamp(), clientId));
                    }
                },
                [WsMqKeyword.DrivesRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    var data = OperationMqBodyUtil.GetDrivesMqReceiveBody(ea.Body);
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new DrivesMqEvent(appId, loginName, ea.GetTimestamp(), clientId, data));
                    }
                },
                [WsMqKeyword.SetVirtualMemoryRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        var data = OperationMqBodyUtil.GetSetVirtualMemoryMqReceiveBody(ea.Body);
                        VirtualRoot.RaiseEvent(new SetVirtualMemoryMqEvent(appId, loginName, ea.GetTimestamp(), clientId, data));
                    }
                },
                [WsMqKeyword.GetLocalIpsRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new GetLocalIpsMqEvent(appId, loginName, ea.GetTimestamp(), clientId));
                    }
                },
                [WsMqKeyword.SetLocalIpsRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        var data = OperationMqBodyUtil.GetSetLocalIpsMqReceiveBody(ea.Body);
                        VirtualRoot.RaiseEvent(new SetLocalIpsMqEvent(appId, loginName, ea.GetTimestamp(), clientId, data));
                    }
                },
                [WsMqKeyword.LocalIpsRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    var data = OperationMqBodyUtil.GetLocalIpsMqReceiveBody(ea.Body);
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new LocalIpsMqEvent(appId, loginName, ea.GetTimestamp(), clientId, data));
                    }
                },
                [WsMqKeyword.OperationResultsRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    var data = OperationMqBodyUtil.GetOperationResultsMqReceiveBody(ea.Body);
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new OperationResultsMqEvent(appId, loginName, ea.GetTimestamp(), clientId, data));
                    }
                },
                [WsMqKeyword.OperationReceivedRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new OperationReceivedMqEvent(appId, loginName, ea.GetTimestamp(), clientId));
                    }
                },
                [WsMqKeyword.GetSpeedRoutingKey] = ea => {
                    string appId = ea.BasicProperties.AppId;
                    UserGetSpeedRequest[] requests = OperationMqBodyUtil.GetGetSpeedMqReceiveBody(ea.Body);
                    if (requests != null && requests.Length != 0) {
                        VirtualRoot.RaiseEvent(new GetSpeedMqEvent(appId, ea.GetTimestamp(), requests));
                    }
                },
                [WsMqKeyword.EnableRemoteDesktopRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new EnableRemoteDesktopMqEvent(appId, loginName, ea.GetTimestamp(), clientId));
                    }
                },
                [WsMqKeyword.BlockWAURoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new BlockWAUMqEvent(appId, loginName, ea.GetTimestamp(), clientId));
                    }
                },
                [WsMqKeyword.SwitchRadeonGpuRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        bool on = OperationMqBodyUtil.GetSwitchRadeonGpuMqReceiveBody(ea.Body);
                        VirtualRoot.RaiseEvent(new SwitchRadeonGpuMqEvent(appId, loginName, ea.GetTimestamp(), clientId, on));
                    }
                },
                [WsMqKeyword.GetSelfWorkLocalJsonRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new GetSelfWorkLocalJsonMqEvent(appId, loginName, ea.GetTimestamp(), clientId));
                    }
                },
                [WsMqKeyword.SelfWorkLocalJsonRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        string json = OperationMqBodyUtil.GetSelfWorkLocalJsonMqReceiveBody(ea.Body);
                        VirtualRoot.RaiseEvent(new LocalJsonMqEvent(appId, loginName, ea.GetTimestamp(), clientId, json));
                    }
                },
                [WsMqKeyword.SaveSelfWorkLocalJsonRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        WorkRequest request = OperationMqBodyUtil.GetSaveSelfWorkLocalJsonMqReceiveBody(ea.Body);
                        VirtualRoot.RaiseEvent(new SaveSelfWorkLocalJsonMqEvent(appId, loginName, ea.GetTimestamp(), clientId, request));
                    }
                },
                [WsMqKeyword.GetGpuProfilesJsonRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new GetGpuProfilesJsonMqEvent(appId, loginName, ea.GetTimestamp(), clientId));
                    }
                },
                [WsMqKeyword.GpuProfilesJsonRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        string json = OperationMqBodyUtil.GetGpuProfilesJsonMqReceiveBody(ea.Body);
                        VirtualRoot.RaiseEvent(new GpuProfilesJsonMqEvent(appId, loginName, ea.GetTimestamp(), clientId, json));
                    }
                },
                [WsMqKeyword.SaveGpuProfilesJsonRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        string json = OperationMqBodyUtil.GetSaveGpuProfilesJsonMqReceiveBody(ea.Body);
                        VirtualRoot.RaiseEvent(new SaveGpuProfilesJsonMqEvent(appId, loginName, ea.GetTimestamp(), clientId, json));
                    }
                },
                [WsMqKeyword.SetAutoBootStartRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        SetAutoBootStartRequest body = OperationMqBodyUtil.GetSetAutoBootStartMqReceiveBody(ea.Body);
                        if (body != null) {
                            VirtualRoot.RaiseEvent(new SetAutoBootStartMqEvent(appId, loginName, ea.GetTimestamp(), clientId, body));
                        }
                    }
                },
                [WsMqKeyword.RestartWindowsRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new RestartWindowsMqEvent(appId, loginName, ea.GetTimestamp(), clientId));
                    }
                },
                [WsMqKeyword.ShutdownWindowsRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new ShutdownWindowsMqEvent(appId, loginName, ea.GetTimestamp(), clientId));
                    }
                },
                [WsMqKeyword.UpgradeNTMinerRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        string ntminerFileName = OperationMqBodyUtil.GetUpgradeNTMinerMqReceiveBody(ea.Body);
                        if (!string.IsNullOrEmpty(ntminerFileName)) {
                            VirtualRoot.RaiseEvent(new UpgradeNTMinerMqEvent(appId, loginName, ea.GetTimestamp(), clientId, ntminerFileName));
                        }
                    }
                },
                [MqKeyword.StartMineRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        Guid workId = OperationMqBodyUtil.GetStartMineMqReceiveBody(ea.Body);
                        VirtualRoot.RaiseEvent(new StartMineMqEvent(appId, loginName, ea.GetTimestamp(), clientId, workId));
                    }
                },
                [MqKeyword.StartWorkMineRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        WorkRequest request = OperationMqBodyUtil.GetStartWorkMineMqReceiveBody(ea.Body);
                        VirtualRoot.RaiseEvent(new StartWorkMineMqEvent(appId, loginName, ea.GetTimestamp(), clientId, request));
                    }
                },
                [WsMqKeyword.StopMineRoutingKey] = ea => {
                    string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                    string appId = ea.BasicProperties.AppId;
                    if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                        VirtualRoot.RaiseEvent(new StopMineMqEvent(appId, loginName, ea.GetTimestamp(), clientId));
                    }
                }
            };
        }
    }
}

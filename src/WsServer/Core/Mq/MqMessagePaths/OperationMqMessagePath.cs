using NTMiner.Core.Daemon;
using NTMiner.Core.MinerServer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class OperationMqMessagePath : AbstractMqMessagePath<UserSetInitedEvent, MinerSignSetInitedEvent> {
        public OperationMqMessagePath(string queue) : base(queue) {
        }

        protected override void Build(IModel channal) {
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.MinerSignSetedRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.GetConsoleOutLinesRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.FastGetConsoleOutLinesRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.ConsoleOutLinesRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.GetLocalMessagesRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.FastGetLocalMessagesRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.LocalMessagesRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.GetOperationResultsRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.FastGetOperationResultsRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.GetDrivesRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.DrivesRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.SetVirtualMemoryRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.GetLocalIpsRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.SetLocalIpsRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.LocalIpsRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.OperationResultsRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.OperationReceivedRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.GetSpeedRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.EnableRemoteDesktopRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.BlockWAURoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.SwitchRadeonGpuRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.GetSelfWorkLocalJsonRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.SelfWorkLocalJsonRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.SaveSelfWorkLocalJsonRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.GetGpuProfilesJsonRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.GpuProfilesJsonRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.SaveGpuProfilesJsonRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.SetAutoBootStartRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.RestartWindowsRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.ShutdownWindowsRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.UpgradeNTMinerRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.StartMineRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: MqKeyword.StartWorkMineRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.StopMineRoutingKey, arguments: null);

            NTMinerConsole.UserOk("OperationMq QueueBind成功");
        }

        public override bool Go(BasicDeliverEventArgs ea) {
            switch (ea.RoutingKey) {
                case MqKeyword.MinerSignSetedRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        MinerSign minerSign = MinerClientMqBodyUtil.GetMinerSignMqReceiveBody(ea.Body);
                        if (minerSign != null) {
                            VirtualRoot.RaiseEvent(new MinerSignSetedMqEvent(appId, minerSign, timestamp));
                        }
                    }
                    break;
                case WsMqKeyword.GetConsoleOutLinesRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        AfterTimeRequest[] requests = OperationMqBodyUtil.GetAfterTimeRequestMqReceiveBody(ea.Body);
                        if (requests != null && requests.Length != 0) {
                            foreach (var request in requests) {
                                VirtualRoot.RaiseEvent(new GetConsoleOutLinesMqEvent(appId, request.LoginName, timestamp, request.ClientId, request.AfterTime));
                            }
                        }
                    }
                    break;
                case WsMqKeyword.FastGetConsoleOutLinesRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        long afterTimestamp = OperationMqBodyUtil.GetFastGetConsoleOutLinesMqReceiveBody(ea.Body);
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new GetConsoleOutLinesMqEvent(appId, loginName, timestamp, clientId, afterTimestamp));
                        }
                    }
                    break;
                case WsMqKeyword.ConsoleOutLinesRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        var data = OperationMqBodyUtil.GetConsoleOutLinesMqReceiveBody(ea.Body);
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new ConsoleOutLinesMqEvent(appId, loginName, timestamp, clientId, data));
                        }
                    }
                    break;
                case WsMqKeyword.GetLocalMessagesRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        AfterTimeRequest[] requests = OperationMqBodyUtil.GetAfterTimeRequestMqReceiveBody(ea.Body);
                        if (requests != null && requests.Length != 0) {
                            foreach (var request in requests) {
                                VirtualRoot.RaiseEvent(new GetLocalMessagesMqEvent(appId, request.LoginName, timestamp, request.ClientId, request.AfterTime));
                            }
                        }
                    }
                    break;
                case WsMqKeyword.FastGetLocalMessagesRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        long afterTimestamp = OperationMqBodyUtil.GetFastGetLocalMessagesMqReceiveBody(ea.Body);
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new GetLocalMessagesMqEvent(appId, loginName, timestamp, clientId, afterTimestamp));
                        }
                    }
                    break;
                case WsMqKeyword.LocalMessagesRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        var data = OperationMqBodyUtil.GetLocalMessagesMqReceiveBody(ea.Body);
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new LocalMessagesMqEvent(appId, loginName, timestamp, clientId, data));
                        }
                    }
                    break;
                case WsMqKeyword.GetOperationResultsRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        AfterTimeRequest[] requests = OperationMqBodyUtil.GetAfterTimeRequestMqReceiveBody(ea.Body);
                        if (requests != null && requests.Length != 0) {
                            foreach (var request in requests) {
                                VirtualRoot.RaiseEvent(new GetOperationResultsMqEvent(appId, request.LoginName, timestamp, request.ClientId, request.AfterTime));
                            }
                        }
                    }
                    break;
                case WsMqKeyword.FastGetOperationResultsRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        long afterTimestamp = OperationMqBodyUtil.GetFastGetOperationResultsMqReceiveBody(ea.Body);
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new GetOperationResultsMqEvent(appId, loginName, timestamp, clientId, afterTimestamp));
                        }
                    }
                    break;
                case WsMqKeyword.OperationResultsRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        var data = OperationMqBodyUtil.GetOperationResultsMqReceiveBody(ea.Body);
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new OperationResultsMqEvent(appId, loginName, timestamp, clientId, data));
                        }
                    }
                    break;
                case WsMqKeyword.GetDrivesRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new GetDrivesMqEvent(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.DrivesRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        var data = OperationMqBodyUtil.GetDrivesMqReceiveBody(ea.Body);
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new DrivesMqEvent(appId, loginName, timestamp, clientId, data));
                        }
                    }
                    break;
                case WsMqKeyword.GetLocalIpsRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new GetLocalIpsMqEvent(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.LocalIpsRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        var data = OperationMqBodyUtil.GetLocalIpsMqReceiveBody(ea.Body);
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new LocalIpsMqEvent(appId, loginName, timestamp, clientId, data));
                        }
                    }
                    break;
                case WsMqKeyword.OperationReceivedRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new OperationReceivedMqEvent(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.GetSpeedRoutingKey: {
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        UserGetSpeedRequest[] requests = OperationMqBodyUtil.GetGetSpeedMqReceiveBody(ea.Body);
                        if (requests != null && requests.Length != 0) {
                            VirtualRoot.RaiseEvent(new GetSpeedMqEvent(appId, timestamp, requests));
                        }
                    }
                    break;
                case WsMqKeyword.EnableRemoteDesktopRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new EnableRemoteDesktopMqEvent(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.BlockWAURoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new BlockWAUMqEvent(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.SetVirtualMemoryRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            var data = OperationMqBodyUtil.GetSetVirtualMemoryMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new SetVirtualMemoryMqEvent(appId, loginName, timestamp, clientId, data));
                        }
                    }
                    break;
                case WsMqKeyword.SetLocalIpsRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            var data = OperationMqBodyUtil.GetSetLocalIpsMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new SetLocalIpsMqEvent(appId, loginName, timestamp, clientId, data));
                        }
                    }
                    break;
                case WsMqKeyword.SwitchRadeonGpuRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            bool on = OperationMqBodyUtil.GetSwitchRadeonGpuMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new SwitchRadeonGpuMqEvent(appId, loginName, timestamp, clientId, on));
                        }
                    }
                    break;
                case WsMqKeyword.GetSelfWorkLocalJsonRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new GetSelfWorkLocalJsonMqEvent(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.SelfWorkLocalJsonRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            string json = OperationMqBodyUtil.GetSelfWorkLocalJsonMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new LocalJsonMqEvent(appId, loginName, timestamp, clientId, json));
                        }
                    }
                    break;
                case WsMqKeyword.SaveSelfWorkLocalJsonRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            WorkRequest request = OperationMqBodyUtil.GetSaveSelfWorkLocalJsonMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new SaveSelfWorkLocalJsonMqEvent(appId, loginName, timestamp, clientId, request));
                        }
                    }
                    break;
                case WsMqKeyword.GetGpuProfilesJsonRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new GetGpuProfilesJsonMqEvent(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.GpuProfilesJsonRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            string json = OperationMqBodyUtil.GetGpuProfilesJsonMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new GpuProfilesJsonMqEvent(appId, loginName, timestamp, clientId, json));
                        }
                    }
                    break;
                case WsMqKeyword.SaveGpuProfilesJsonRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            string json = OperationMqBodyUtil.GetSaveGpuProfilesJsonMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new SaveGpuProfilesJsonMqEvent(appId, loginName, timestamp, clientId, json));
                        }
                    }
                    break;
                case WsMqKeyword.SetAutoBootStartRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            SetAutoBootStartRequest body = OperationMqBodyUtil.GetSetAutoBootStartMqReceiveBody(ea.Body);
                            if (body != null) {
                                VirtualRoot.RaiseEvent(new SetAutoBootStartMqEvent(appId, loginName, timestamp, clientId, body));
                            }
                        }
                    }
                    break;
                case WsMqKeyword.RestartWindowsRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new RestartWindowsMqEvent(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.ShutdownWindowsRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new ShutdownWindowsMqEvent(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.UpgradeNTMinerRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            string ntminerFileName = OperationMqBodyUtil.GetUpgradeNTMinerMqReceiveBody(ea.Body);
                            if (!string.IsNullOrEmpty(ntminerFileName)) {
                                VirtualRoot.RaiseEvent(new UpgradeNTMinerMqEvent(appId, loginName, timestamp, clientId, ntminerFileName));
                            }
                        }
                    }
                    break;
                case MqKeyword.StartMineRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            Guid workId = OperationMqBodyUtil.GetStartMineMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new StartMineMqEvent(appId, loginName, timestamp, clientId, workId));
                        }
                    }
                    break;
                case MqKeyword.StartWorkMineRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            WorkRequest request = OperationMqBodyUtil.GetStartWorkMineMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new StartWorkMineMqEvent(appId, loginName, timestamp, clientId, request));
                        }
                    }
                    break;
                case WsMqKeyword.StopMineRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(MqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new StopMineMqEvent(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                default:
                    return false;
            }
            return true;
        }
    }
}

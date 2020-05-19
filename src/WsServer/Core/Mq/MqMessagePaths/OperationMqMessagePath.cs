using NTMiner.Core.Daemon;
using NTMiner.Core.MinerServer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public class OperationMqMessagePath : AbstractMqMessagePath<UserSetInitedEvent, MinerSignSetInitedEvent> {
        public OperationMqMessagePath(string queue) : base(queue) {
        }

        protected override void Build(IModel channal) {
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.GetConsoleOutLinesRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.ConsoleOutLinesRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.GetLocalMessagesRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.LocalMessagesRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.GetDrivesRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.DrivesRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.SetVirtualMemoryRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.GetLocalIpsRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.SetLocalIpsRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.LocalIpsRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.GetOperationResultsRoutingKey, arguments: null);
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
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.StartMineRoutingKey, arguments: null);
            channal.QueueBind(queue: Queue, exchange: MqKeyword.NTMinerExchange, routingKey: WsMqKeyword.StopMineRoutingKey, arguments: null);

            Write.UserOk("OperationMq QueueBind成功");
        }

        public override void Go(BasicDeliverEventArgs ea) {
            switch (ea.RoutingKey) {
                case WsMqKeyword.GetConsoleOutLinesRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        long afterTimestamp = OperationMqBodyUtil.GetGetConsoleOutLinesMqReceiveBody(ea.Body);
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new GetConsoleOutLinesMqMessage(appId, loginName, timestamp, clientId, afterTimestamp));
                        }
                    }
                    break;
                case WsMqKeyword.ConsoleOutLinesRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        var data = OperationMqBodyUtil.GetConsoleOutLinesMqReceiveBody(ea.Body);
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new ConsoleOutLinesMqMessage(appId, loginName, timestamp, clientId, data));
                        }
                    }
                    break;
                case WsMqKeyword.GetLocalMessagesRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        long afterTimestamp = OperationMqBodyUtil.GetGetLocalMessagesMqReceiveBody(ea.Body);
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new GetLocalMessagesMqMessage(appId, loginName, timestamp, clientId, afterTimestamp));
                        }
                    }
                    break;
                case WsMqKeyword.LocalMessagesRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        var data = OperationMqBodyUtil.GetLocalMessagesMqReceiveBody(ea.Body);
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new LocalMessagesMqMessage(appId, loginName, timestamp, clientId, data));
                        }
                    }
                    break;
                case WsMqKeyword.GetDrivesRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new GetDrivesMqMessage(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.DrivesRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        var data = OperationMqBodyUtil.GetDrivesMqReceiveBody(ea.Body);
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new DrivesMqMessage(appId, loginName, timestamp, clientId, data));
                        }
                    }
                    break;
                case WsMqKeyword.GetLocalIpsRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new GetLocalIpsMqMessage(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.LocalIpsRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        var data = OperationMqBodyUtil.GetLocalIpsMqReceiveBody(ea.Body);
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new LocalIpsMqMessage(appId, loginName, timestamp, clientId, data));
                        }
                    }
                    break;
                case WsMqKeyword.GetOperationResultsRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        long afterTimestamp = OperationMqBodyUtil.GetGetOperationResultsMqReceiveBody(ea.Body);
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new GetOperationResultsMqMessage(appId, loginName, timestamp, clientId, afterTimestamp));
                        }
                    }
                    break;
                case WsMqKeyword.OperationResultsRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        var data = OperationMqBodyUtil.GetOperationResultsMqReceiveBody(ea.Body);
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new OperationResultsMqMessage(appId, loginName, timestamp, clientId, data));
                        }
                    }
                    break;
                case WsMqKeyword.OperationReceivedRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new OperationReceivedMqMessage(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.GetSpeedRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        List<Guid> clientIds = OperationMqBodyUtil.GetGetSpeedMqReceiveBody(ea.Body);
                        if (clientIds != null && clientIds.Count != 0) {
                            VirtualRoot.RaiseEvent(new GetSpeedMqMessage(appId, loginName, timestamp, clientIds));
                        }
                    }
                    break;
                case WsMqKeyword.EnableRemoteDesktopRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new EnableRemoteDesktopMqMessage(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.BlockWAURoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new BlockWAUMqMessage(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.SetVirtualMemoryRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            var data = OperationMqBodyUtil.GetSetVirtualMemoryMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new SetVirtualMemoryMqMessage(appId, loginName, timestamp, clientId, data));
                        }
                    }
                    break;
                case WsMqKeyword.SetLocalIpsRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            var data = OperationMqBodyUtil.GetSetLocalIpsMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new SetLocalIpsMqMessage(appId, loginName, timestamp, clientId, data));
                        }
                    }
                    break;
                case WsMqKeyword.SwitchRadeonGpuRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            bool on = OperationMqBodyUtil.GetSwitchRadeonGpuMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new SwitchRadeonGpuMqMessage(appId, loginName, timestamp, clientId, on));
                        }
                    }
                    break;
                case WsMqKeyword.GetSelfWorkLocalJsonRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new GetSelfWorkLocalJsonMqMessage(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.SelfWorkLocalJsonRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            string json = OperationMqBodyUtil.GetSelfWorkLocalJsonMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new LocalJsonMqMessage(appId, loginName, timestamp, clientId, json));
                        }
                    }
                    break;
                case WsMqKeyword.SaveSelfWorkLocalJsonRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            WorkRequest request = OperationMqBodyUtil.GetSaveSelfWorkLocalJsonMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new SaveSelfWorkLocalJsonMqMessage(appId, loginName, timestamp, clientId, request));
                        }
                    }
                    break;
                case WsMqKeyword.GetGpuProfilesJsonRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new GetGpuProfilesJsonMqMessage(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.GpuProfilesJsonRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            string json = OperationMqBodyUtil.GetGpuProfilesJsonMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new GpuProfilesJsonMqMessage(appId, loginName, timestamp, clientId, json));
                        }
                    }
                    break;
                case WsMqKeyword.SaveGpuProfilesJsonRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            string json = OperationMqBodyUtil.GetSaveGpuProfilesJsonMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new SaveGpuProfilesJsonMqMessage(appId, loginName, timestamp, clientId, json));
                        }
                    }
                    break;
                case WsMqKeyword.SetAutoBootStartRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            SetAutoBootStartRequest body = OperationMqBodyUtil.GetSetAutoBootStartMqReceiveBody(ea.Body);
                            if (body != null) {
                                VirtualRoot.RaiseEvent(new SetAutoBootStartMqMessage(appId, loginName, timestamp, clientId, body));
                            }
                        }
                    }
                    break;
                case WsMqKeyword.RestartWindowsRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new RestartWindowsMqMessage(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.ShutdownWindowsRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new ShutdownWindowsMqMessage(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                case WsMqKeyword.UpgradeNTMinerRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            string ntminerFileName = OperationMqBodyUtil.GetUpgradeNTMinerMqReceiveBody(ea.Body);
                            if (!string.IsNullOrEmpty(ntminerFileName)) {
                                VirtualRoot.RaiseEvent(new UpgradeNTMinerMqMessage(appId, loginName, timestamp, clientId, ntminerFileName));
                            }
                        }
                    }
                    break;
                case WsMqKeyword.StartMineRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            Guid workId = OperationMqBodyUtil.GetStartMineMqReceiveBody(ea.Body);
                            VirtualRoot.RaiseEvent(new StartMineMqMessage(appId, loginName, timestamp, clientId, workId));
                        }
                    }
                    break;
                case WsMqKeyword.StopMineRoutingKey: {
                        string loginName = ea.BasicProperties.ReadHeaderString(MqKeyword.LoginNameHeaderName);
                        DateTime timestamp = Timestamp.FromTimestamp(ea.BasicProperties.Timestamp.UnixTime);
                        string appId = ea.BasicProperties.AppId;
                        if (ea.BasicProperties.ReadHeaderGuid(WsMqKeyword.ClientIdHeaderName, out Guid clientId)) {
                            VirtualRoot.RaiseEvent(new StopMineMqMessage(appId, loginName, timestamp, clientId));
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

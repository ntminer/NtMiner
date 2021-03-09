using NTMiner.Core.Daemon;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using NTMiner.VirtualMemory;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Mq.Senders.Impl {
    public class OperationMqSender : IOperationMqSender {
        private static readonly byte[] _emptyBody = new byte[0];
        private readonly IMq _mq;
        public OperationMqSender(IMq mq) {
            _mq = mq;
        }

        public void SendGetConsoleOutLines(string loginName, Guid clientId, long asterTime) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.GetConsoleOutLinesRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetGetConsoleOutLinesMqSendBody(asterTime));
        }

        public void SendConsoleOutLines(string loginName, Guid clientId, List<ConsoleOutLine> datas) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty || datas == null || datas.Count == 0) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.ConsoleOutLinesRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetConsoleOutLinesMqSendBody(datas));
        }

        public void SendGetLocalMessages(string loginName, Guid clientId, long afterTime) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.GetLocalMessagesRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetGetLocalMessagesMqSendBody(afterTime));
        }

        public void SendLocalMessages(string loginName, Guid clientId, List<LocalMessageDto> datas) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty || datas == null || datas.Count == 0) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.LocalMessagesRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetLocalMessagesMqSendBody(datas));
        }

        public void SendGetDrives(string loginName, Guid clientId) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.GetDrivesRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: _emptyBody);
        }

        public void SendDrives(string loginName, Guid clientId, List<DriveDto> datas) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty || datas == null || datas.Count == 0) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.DrivesRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetDrivesMqSendBody(datas));
        }

        public void SendGetLocalIps(string loginName, Guid clientId) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.GetLocalIpsRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: _emptyBody);
        }

        public void SendLocalIps(string loginName, Guid clientId, List<LocalIpDto> datas) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty || datas == null || datas.Count == 0) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.LocalIpsRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetLocalIpsMqSendBody(datas));
        }

        public void SendGetOperationResults(string loginName, Guid clientId, long afterTime) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.GetOperationResultsRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetGetOperationResultsMqSendBody(afterTime));
        }

        public void SendOperationResults(string loginName, Guid clientId, List<OperationResultData> datas) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty || datas == null || datas.Count == 0) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.OperationResultsRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetOperationResultsMqSendBody(datas));
        }

        public void SendOperationReceived(string loginName, Guid clientId) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.OperationReceivedRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: _emptyBody);
        }

        public void SendGetSpeed(string loginName, List<Guid> clientIds) {
            if (string.IsNullOrEmpty(loginName) || clientIds == null || clientIds.Count == 0) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.GetSpeedRoutingKey,
                basicProperties: CreateBasicProperties(loginName),
                body: OperationMqBodyUtil.GetGetSpeedMqSendBody(clientIds));
        }

        public void SendEnableRemoteDesktop(string loginName, Guid clientId) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.EnableRemoteDesktopRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: _emptyBody);
        }

        public void SendBlockWAU(string loginName, Guid clientId) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.BlockWAURoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: _emptyBody);
        }

        public void SendSwitchRadeonGpu(string loginName, Guid clientId, bool on) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.SwitchRadeonGpuRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetSwitchRadeonGpuMqSendBody(on));
        }

        public void SendSetVirtualMemory(string loginName, Guid clientId, Dictionary<string, int> datas) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.SetVirtualMemoryRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetSetVirtualMemoryMqSendBody(datas));
        }

        public void SendSetLocalIps(string loginName, Guid clientId, List<LocalIpInput> datas) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.SetLocalIpsRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetSetLocalIpsMqSendBody(datas));
        }

        public void SendGetSelfWorkLocalJson(string loginName, Guid clientId) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.GetSelfWorkLocalJsonRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: _emptyBody);
        }

        public void SendSaveSelfWorkLocalJson(string loginName, Guid clientId, WorkRequest request) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty || request == null) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.SaveSelfWorkLocalJsonRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetSaveSelfWorkLocalJsonMqSendBody(request));
        }

        public void SendGetGpuProfilesJson(string loginName, Guid clientId) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.GetGpuProfilesJsonRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: _emptyBody);
        }

        public void SendSelfWorkLocalJson(string loginName, Guid clientId, string json) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.SelfWorkLocalJsonRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetSelfWorkLocalJsonMqSendBody(json));
        }

        public void SendGpuProfilesJson(string loginName, Guid clientId, string json) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.GpuProfilesJsonRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetGpuProfilesJsonMqSendBody(json));
        }

        public void SendSaveGpuProfilesJson(string loginName, Guid clientId, string json) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty || string.IsNullOrEmpty(json)) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.SaveGpuProfilesJsonRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetSaveGpuProfilesJsonMqSendBody(json));
        }

        public void SendSetAutoBootStart(string loginName, Guid clientId, SetAutoBootStartRequest body) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.SetAutoBootStartRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetSetAutoBootStartMqSendBody(body));
        }

        public void SendRestartWindows(string loginName, Guid clientId) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.RestartWindowsRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: _emptyBody);
        }

        public void SendShutdownWindows(string loginName, Guid clientId) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.ShutdownWindowsRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: _emptyBody);
        }

        public void SendUpgradeNTMiner(string loginName, Guid clientId, string ntminerFileName) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty || string.IsNullOrEmpty(ntminerFileName)) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.UpgradeNTMinerRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetUpgradeNTMinerMqSendBody(ntminerFileName));
        }

        public void SendStartMine(string loginName, Guid clientId, Guid workId) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: MqKeyword.StartMineRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: OperationMqBodyUtil.GetStartMineMqSendBody(workId));
        }

        public void SendStopMine(string loginName, Guid clientId) {
            if (string.IsNullOrEmpty(loginName) || clientId == Guid.Empty) {
                return;
            }
            _mq.MqChannel.BasicPublish(
                exchange: MqKeyword.NTMinerExchange,
                routingKey: WsMqKeyword.StopMineRoutingKey,
                basicProperties: CreateBasicProperties(loginName, clientId),
                body: _emptyBody);
        }

        private IBasicProperties CreateBasicProperties(string loginName, Guid clientId) {
            var basicProperties = CreateBasicProperties(loginName);
            basicProperties.Headers[MqKeyword.ClientIdHeaderName] = clientId.ToString();

            return basicProperties;
        }

        private IBasicProperties CreateBasicProperties(string loginName) {
            var basicProperties = _mq.MqChannel.CreateBasicProperties();
            basicProperties.Persistent = false;
            basicProperties.Expiration = MqKeyword.Expiration36sec;
            basicProperties.Timestamp = new AmqpTimestamp(Timestamp.GetTimestamp());
            basicProperties.AppId = ServerRoot.HostConfig.ThisServerAddress;
            basicProperties.Headers = new Dictionary<string, object> {
                [MqKeyword.LoginNameHeaderName] = loginName
            };

            return basicProperties;
        }
    }
}

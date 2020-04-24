using NTMiner.Core.MinerServer;
using NTMiner.Core.Mq.Senders;
using NTMiner.Core.Redis;
using NTMiner.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NTMiner.Core.Impl {
    public class ClientDataSet : ClientDataSetBase, IClientDataSet {
        private const string _safeIgnoreMessage = "该消息发生的时间早于本节点启动时间1分钟，安全忽略";

        private readonly IMinerRedis _minerRedis;
        private readonly ISpeedDataRedis _speedDataRedis;
        private readonly IMinerClientMqSender _mqSender;
        public ClientDataSet(IMinerRedis minerRedis, ISpeedDataRedis speedDataRedis, IMinerClientMqSender mqSender) : base(isPull: false, getDatas: callback => {
            var getMinersTask = minerRedis.GetAllAsync();
            var getSpeedsTask = speedDataRedis.GetAllAsync();
            Task.WhenAll(getMinersTask, getSpeedsTask).ContinueWith(t => {
                var speedDatas = getSpeedsTask.Result;
                List<ClientData> clientDatas = new List<ClientData>();
                foreach (var minerData in getMinersTask.Result) {
                    var clientData = ClientData.Create(minerData);
                    clientDatas.Add(clientData);
                    var speedData = speedDatas.FirstOrDefault(a => a.ClientId == minerData.ClientId);
                    if (speedData != null) {
                        clientData.Update(speedData);
                    }
                }
                callback?.Invoke(clientDatas);
            });
        }) {
            _minerRedis = minerRedis;
            _speedDataRedis = speedDataRedis;
            _mqSender = mqSender;
            // 收到Mq消息之前一定已经初始化完成，因为Mq消费者在ClientSetInitedEvent事件之后才会创建
            VirtualRoot.AddEventPath<SpeedDataMqMessage>("收到SpeedDataMq消息后更新ClientData内存", LogEnum.None, action: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                if (message.ClientId == Guid.Empty) {
                    return;
                }
                if (IsOldMqMessage(message.Timestamp)) {
                    Write.UserOk(_safeIgnoreMessage);
                    return;
                }
                speedDataRedis.GetByClientIdAsync(message.ClientId).ContinueWith(t => {
                    ReportSpeed(t.Result, message.MinerIp, isFromWsServerNode: true);
                });
            }, this.GetType());
            VirtualRoot.AddEventPath<MinerClientWsOpenedMqMessage>("收到MinerClientWsOpenedMq消息后更新NetActiveOn和IsOnline", LogEnum.None, action: message => {
                if (IsOldMqMessage(message.Timestamp)) {
                    Write.UserOk(_safeIgnoreMessage);
                    return;
                }
                if (_dicByClientId.TryGetValue(message.ClientId, out ClientData clientData)) {
                    clientData.NetActiveOn = message.Timestamp;
                    clientData.IsOnline = true;
                }
            }, this.GetType());
            VirtualRoot.AddEventPath<MinerClientWsClosedMqMessage>("收到MinerClientWsClosedMq消息后更新NetActiveOn和IsOnline", LogEnum.None, action: message => {
                if (IsOldMqMessage(message.Timestamp)) {
                    Write.UserOk(_safeIgnoreMessage);
                    return;
                }
                if (_dicByClientId.TryGetValue(message.ClientId, out ClientData clientData)) {
                    clientData.NetActiveOn = message.Timestamp;
                    clientData.IsOnline = false;
                }
            }, this.GetType());
            VirtualRoot.AddEventPath<MinerClientWsBreathedMqMessage>("收到MinerClientWsBreathedMq消息后更新NetActiveOn", LogEnum.None, action: message => {
                if (IsOldMqMessage(message.Timestamp)) {
                    Write.UserOk(_safeIgnoreMessage);
                    return;
                }
                if (_dicByClientId.TryGetValue(message.ClientId, out ClientData clientData)) {
                    clientData.NetActiveOn = message.Timestamp;
                    clientData.IsOnline = true;
                }
            }, this.GetType());
            VirtualRoot.AddCmdPath<ChangeMinerSignMqMessage>(action: message => {
                if (_dicByObjectId.TryGetValue(message.Data.Id, out ClientData clientData)) {
                    clientData.Update(message.Data, out bool isChanged);
                    if (isChanged) {
                        var minerData = MinerData.Create(clientData);
                        _minerRedis.SetAsync(minerData).ContinueWith(t => {
                            _mqSender.SendMinerSignChanged(minerData.Id);
                        });
                    }
                }
                else {
                    Add(ClientData.Create(MinerData.Create(message.Data)));
                }
            }, this.GetType(), LogEnum.DevConsole);
        }

        private bool IsOldMqMessage(DateTime mqMessageTimestamp) {
            // 考虑到服务器间时钟可能不完全同步，如果消息发生的时间比_initedOn的时间早了
            // 一分多钟则可以视为Init时已经包含了该Mq消息所表达的事情就不需要再访问Redis了
            if (mqMessageTimestamp.AddMinutes(1) < base.InitedOn) {
                return true;
            }
            return false;
        }

        public void ReportSpeed(SpeedData speedData, string minerIp, bool isFromWsServerNode) {
            if (!IsReadied) {
                return;
            }
            if (speedData == null || speedData.ClientId == Guid.Empty) {
                return;
            }
            if (string.IsNullOrEmpty(minerIp)) {
                return;
            }
            if (!isFromWsServerNode) {
                _speedDataRedis.SetAsync(speedData);
            }
            ClientData clientData = GetByClientId(speedData.ClientId);
            if (clientData == null) {
                clientData = ClientData.Create(speedData, minerIp);
                Add(clientData);
            }
            else {
                clientData.Update(speedData, minerIp, out bool isMinerDataChanged);
                if (isMinerDataChanged) {
                    DoUpdateSave(MinerData.Create(clientData));
                }
            }
        }

        public void ReportState(ReportState state, string minerIp, bool isFromWsServerNode) {
            if (!IsReadied) {
                return;
            }
            if (state == null || state.ClientId == Guid.Empty) {
                return;
            }
            if (string.IsNullOrEmpty(minerIp)) {
                return;
            }
            ClientData clientData = GetByClientId(state.ClientId);
            if (clientData == null) {
                clientData = ClientData.Create(state, minerIp);
                Add(clientData);
            }
            else {
                clientData.Update(state, minerIp, out bool isMinerDataChanged);
                if (isMinerDataChanged) {
                    DoUpdateSave(MinerData.Create(clientData));
                }
            }
            if (!isFromWsServerNode) {
                var speedData = clientData.ToSpeedData();
                _speedDataRedis.SetAsync(speedData);
            }
        }

        private void Add(ClientData clientData) {
            if (!IsReadied) {
                return;
            }
            if (clientData.ClientId == Guid.Empty) {
                return;
            }
            if (!_dicByClientId.ContainsKey(clientData.ClientId)) {
                _dicByClientId.Add(clientData.ClientId, clientData);
                if (!_dicByObjectId.ContainsKey(clientData.Id)) {
                    _dicByObjectId.Add(clientData.Id, clientData);
                }
                var minerData = MinerData.Create(clientData);
                _minerRedis.SetAsync(minerData).ContinueWith(t => {
                    _mqSender.SendMinerDataAdded(minerData.Id);
                });
            }
        }

        public override void UpdateClient(string objectId, string propertyName, object value) {
            if (!IsReadied) {
                return;
            }
            if (objectId == null) {
                return;
            }
            if (_dicByObjectId.TryGetValue(objectId, out ClientData clientData)) {
                PropertyInfo propertyInfo = typeof(ClientData).GetProperty(propertyName);
                if (propertyInfo != null) {
                    value = VirtualRoot.ConvertValue(propertyInfo.PropertyType, value);
                    var oldValue = propertyInfo.GetValue(clientData, null);
                    if (oldValue != value) {
                        propertyInfo.SetValue(clientData, value, null);
                        DoUpdateSave(MinerData.Create(clientData));
                    }
                }
            }
        }

        public override void UpdateClients(string propertyName, Dictionary<string, object> values) {
            if (!IsReadied) {
                return;
            }
            if (values.Count == 0) {
                return;
            }
            PropertyInfo propertyInfo = typeof(ClientData).GetProperty(propertyName);
            if (propertyInfo != null) {
                values.ChangeValueType(propertyInfo.PropertyType);
                List<MinerData> minerDatas = new List<MinerData>();
                foreach (var kv in values) {
                    string objectId = kv.Key;
                    object value = kv.Value;
                    if (_dicByObjectId.TryGetValue(objectId, out ClientData clientData)) {
                        var oldValue = propertyInfo.GetValue(clientData, null);
                        if (oldValue != value) {
                            propertyInfo.SetValue(clientData, value, null);
                            minerDatas.Add(MinerData.Create(clientData));
                        }
                    }
                }
                DoUpdateSave(minerDatas);
            }
        }

        protected override void DoUpdateSave(MinerData minerData) {
            if (!IsReadied) {
                return;
            }
            _minerRedis.SetAsync(minerData);
        }

        protected override void DoUpdateSave(IEnumerable<MinerData> minerDatas) {
            if (!IsReadied) {
                return;
            }
            foreach (var minerData in minerDatas) {
                DoUpdateSave(minerData);
            }
        }

        protected override void DoRemoveSave(MinerData minerData) {
            if (!IsReadied) {
                return;
            }
            _minerRedis.DeleteAsync(minerData).ContinueWith(t => {
                _mqSender.SendMinerDataRemoved(minerData.Id);
            });
        }


        protected override void DoCheckIsOnline(IEnumerable<ClientData> clientDatas) {
            DateTime time = DateTime.Now.AddSeconds(-180);
            // 一定时间未上报算力视为0算力
            foreach (var clientData in clientDatas) {
                if (clientData.MinerActiveOn < time) {
                    clientData.DualCoinSpeed = 0;
                    clientData.MainCoinSpeed = 0;
                }
            }
        }
    }
}

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
        private readonly IMinerRedis _minerRedis;
        private readonly ISpeedDataRedis _speedDataRedis;
        private readonly IMinerClientMqSender _mqSender;
        public ClientDataSet(IMinerRedis minerRedis, ISpeedDataRedis speedDataRedis, IMinerClientMqSender mqSender) : base(isPull: false, getDatas: callback => {
            var getMinersTask = minerRedis.GetAllAsync();
            var getSpeedsTask = speedDataRedis.GetAllAsync();
            Task.WhenAll(getMinersTask, getSpeedsTask).ContinueWith(t => {
                NTMinerConsole.UserInfo($"从redis加载了 {getMinersTask.Result.Count} 条MinerData，和 {getSpeedsTask.Result.Count} 条SpeedData");
                var speedDatas = getSpeedsTask.Result;
                List<ClientData> clientDatas = new List<ClientData>();
                DateTime speedOn = DateTime.Now.AddMinutes(-3);
                foreach (var minerData in getMinersTask.Result) {
                    var clientData = ClientData.Create(minerData);
                    // 该属性没有持久化而只在内存中，启动时将该属性值视为当前日期的前一天的零时加上CreatedOn的时间，别处有个周期清理7天不活跃矿机的任务
                    clientData.MinerActiveOn = DateTime.Today.AddDays(-1) + minerData.CreatedOn.TimeOfDay;
                    clientDatas.Add(clientData);
                    var speedData = speedDatas.FirstOrDefault(a => a.ClientId == minerData.ClientId);
                    if (speedData != null && speedData.SpeedOn > speedOn) {
                        clientData.Update(speedData, out bool _);
                    }
                }
                callback?.Invoke(clientDatas);
            });
        }) {
            _minerRedis = minerRedis;
            _speedDataRedis = speedDataRedis;
            _mqSender = mqSender;
            VirtualRoot.BuildEventPath<Per1MinuteEvent>("周期清理Redis中不活跃的来自挖矿端上报的算力记录", LogEnum.DevConsole, path: message => {
                DateTime time = message.BornOn.AddSeconds(-130);
                var toRemoveSpeed = _dicByClientId.Where(a => a.Value.MinerActiveOn != DateTime.MinValue && a.Value.MinerActiveOn <= time).ToArray();
                _speedDataRedis.DeleteByClientIdsAsync(toRemoveSpeed.Select(a => a.Key).ToArray());

                // 删除一周没有活跃过的客户端
                time = message.BornOn.AddDays(-7);
                var toRemoveClient = _dicByObjectId.Where(a => a.Value.MinerActiveOn <= time).ToArray();
                foreach (var kv in toRemoveClient) {
                    base.RemoveByObjectId(kv.Key);
                }
            }, this.GetType());
            // 收到Mq消息之前一定已经初始化完成，因为Mq消费者在ClientSetInitedEvent事件之后才会创建
            VirtualRoot.BuildEventPath<SpeedDataMqMessage>("收到SpeedDataMq消息后更新ClientData内存", LogEnum.None, path: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                if (message.ClientId == Guid.Empty) {
                    return;
                }
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(nameof(SpeedDataMqMessage) + ":" + MqKeyword.SafeIgnoreMessage);
                    return;
                }
                speedDataRedis.GetByClientIdAsync(message.ClientId).ContinueWith(t => {
                    ReportSpeed(t.Result.SpeedDto, message.MinerIp, isFromWsServerNode: true);
                });
            }, this.GetType());
            VirtualRoot.BuildEventPath<MinerClientWsOpenedMqMessage>("收到MinerClientWsOpenedMq消息后更新NetActiveOn和IsOnline", LogEnum.None, path: message => {
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(nameof(MinerClientWsOpenedMqMessage) + ":" + MqKeyword.SafeIgnoreMessage);
                    return;
                }
                if (_dicByClientId.TryGetValue(message.ClientId, out ClientData clientData)) {
                    clientData.NetActiveOn = message.Timestamp;
                    clientData.IsOnline = true;
                }
            }, this.GetType());
            VirtualRoot.BuildEventPath<MinerClientWsClosedMqMessage>("收到MinerClientWsClosedMq消息后更新NetActiveOn和IsOnline", LogEnum.None, path: message => {
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(nameof(MinerClientWsClosedMqMessage) + ":" + MqKeyword.SafeIgnoreMessage);
                    return;
                }
                if (_dicByClientId.TryGetValue(message.ClientId, out ClientData clientData)) {
                    clientData.NetActiveOn = message.Timestamp;
                    clientData.IsOnline = false;
                }
            }, this.GetType());
            VirtualRoot.BuildEventPath<MinerClientWsBreathedMqMessage>("收到MinerClientWsBreathedMq消息后更新NetActiveOn", LogEnum.None, path: message => {
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(nameof(MinerClientWsBreathedMqMessage) + ":" + MqKeyword.SafeIgnoreMessage);
                    return;
                }
                if (_dicByClientId.TryGetValue(message.ClientId, out ClientData clientData)) {
                    clientData.NetActiveOn = message.Timestamp;
                    clientData.IsOnline = true;
                }
            }, this.GetType());
            VirtualRoot.BuildCmdPath<ChangeMinerSignMqMessage>(path: message => {
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
                    // 此时该矿机是第一次在服务端出现
                    NTMinerConsole.UserWarn("该矿机首次出现于WsServer:" + VirtualRoot.JsonSerializer.Serialize(message.Data));
                    clientData = ClientData.Create(MinerData.Create(message.Data));
                    clientData.NetActiveOn = DateTime.Now;
                    clientData.IsOnline = true;
                    clientData.IsOuterUserEnabled = true;
                    Add(clientData);
                }
            }, this.GetType(), LogEnum.None);
            VirtualRoot.BuildCmdPath<QueryClientsForWsMqMessage>(path: message => {
                QueryClientsResponse response = AppRoot.QueryClientsForWs(message.Query);
                _mqSender.SendResponseClientsForWs(message.AppId, message.LoginName, message.SessionId, response);
            }, this.GetType(), LogEnum.None);
        }

        private bool IsOldMqMessage(DateTime mqMessageTimestamp) {
            // 考虑到服务器间时钟可能不完全同步，如果消息发生的时间比_initedOn的时间早了
            // 一分多钟则可以视为Init时已经包含了该Mq消息所表达的事情就不需要再访问Redis了
            if (mqMessageTimestamp.AddMinutes(1) < base.InitedOn) {
                return true;
            }
            return false;
        }

        public void ReportSpeed(SpeedDto speedDto, string minerIp, bool isFromWsServerNode) {
            if (!IsReadied) {
                return;
            }
            if (speedDto == null || speedDto.ClientId == Guid.Empty) {
                return;
            }
            if (string.IsNullOrEmpty(minerIp)) {
                return;
            }
            if (!isFromWsServerNode) {
                _speedDataRedis.SetAsync(new SpeedData(speedDto, DateTime.Now));
            }
            ClientData clientData = GetByClientId(speedDto.ClientId);
            if (clientData == null) {
                clientData = ClientData.Create(speedDto, minerIp);
                Add(clientData);
            }
            else {
                clientData.Update(speedDto, minerIp, out bool isMinerDataChanged);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="propertyName">propertyName是客户端传入的白名单属性</param>
        /// <param name="value"></param>
        public override void UpdateClient(string objectId, string propertyName, object value) {
            if (!IsReadied) {
                return;
            }
            if (objectId == null) {
                return;
            }
            if (_dicByObjectId.TryGetValue(objectId, out ClientData clientData) && ClientData.TryGetReflectionUpdateProperty(propertyName, out PropertyInfo propertyInfo)) {
                value = VirtualRoot.ConvertValue(propertyInfo.PropertyType, value);
                var oldValue = propertyInfo.GetValue(clientData, null);
                if (oldValue != value) {
                    propertyInfo.SetValue(clientData, value, null);
                    DoUpdateSave(MinerData.Create(clientData));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName">propertyName是客户端传入的白名单属性</param>
        /// <param name="values"></param>
        public override void UpdateClients(string propertyName, Dictionary<string, object> values) {
            if (!IsReadied) {
                return;
            }
            if (values.Count == 0) {
                return;
            }
            if (ClientData.TryGetReflectionUpdateProperty(propertyName, out PropertyInfo propertyInfo)) {
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
    }
}

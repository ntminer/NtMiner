using NTMiner.Core.MinerServer;
using NTMiner.Core.Mq.Senders;
using NTMiner.Core.Redis;
using NTMiner.Report;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Core.Impl {
    public class ClientDataSet : ClientDataSetBase, IClientDataSet {
        private readonly IMinerDataRedis _minerRedis;
        private readonly IClientActiveOnRedis _clientActiveOnRedis;
        private readonly ISpeedDataRedis _speedDataRedis;
        private readonly IMinerClientMqSender _mqSender;
        public ClientDataSet(
            IMinerDataRedis minerRedis, IClientActiveOnRedis clientActiveOnRedis,
            ISpeedDataRedis speedDataRedis, IMinerClientMqSender mqSender)
            : base(isPull: false, getDatas: callback => {
                var getMinersTask = minerRedis.GetAllAsync();
                var getClientActiveOnsTask = clientActiveOnRedis.GetAllAsync();
                var getSpeedsTask = speedDataRedis.GetAllAsync();
                Task.WhenAll(getMinersTask, getClientActiveOnsTask, getSpeedsTask).ContinueWith(t => {
                    NTMinerConsole.UserInfo($"从redis加载了 {getMinersTask.Result.Count} 条MinerData，和 {getSpeedsTask.Result.Count} 条SpeedData");
                    Dictionary<Guid, SpeedData> speedDataDic = getSpeedsTask.Result;
                    Dictionary<string, DateTime> clientActiveOnDic = getClientActiveOnsTask.Result;
                    List<ClientData> clientDatas = new List<ClientData>();
                    DateTime speedOn = DateTime.Now.AddMinutes(-3);
                    foreach (var minerData in getMinersTask.Result) {
                        var clientData = ClientData.Create(minerData);
                        if (clientActiveOnDic.TryGetValue(minerData.Id, out DateTime activeOn)) {
                            clientData.MinerActiveOn = activeOn;
                        }
                        clientDatas.Add(clientData);
                        if (speedDataDic.TryGetValue(minerData.ClientId, out SpeedData speedData) && speedData.SpeedOn > speedOn) {
                            clientData.Update(speedData, out bool _);
                        }
                    }
                    callback?.Invoke(clientDatas);
                });
            }) {
            _minerRedis = minerRedis;
            _clientActiveOnRedis = clientActiveOnRedis;
            _speedDataRedis = speedDataRedis;
            _mqSender = mqSender;
            VirtualRoot.BuildEventPath<Per100MinuteEvent>("周期将矿机的MinerActiveOn或NetActiveOn时间戳持久到redis", LogEnum.DevConsole, message => {
                var minerCients = _dicByObjectId.Values.ToArray();
                DateTime time = message.BornOn.AddSeconds(-message.Seconds);
                int count = 0;
                foreach (var minerClient in minerCients) {
                    // 如果活跃则更新到redis，否则就不更新了
                    DateTime activeOn = minerClient.GetActiveOn();
                    if (activeOn > time) {
                        clientActiveOnRedis.SetAsync(minerClient.Id, activeOn);
                        count++;
                    }
                }
                NTMinerConsole.DevWarn($"{count.ToString()} 条活跃矿机的时间戳被持久化");
            }, this.GetType());
            // 上面的持久化时间戳到redis的目的主要是为了下面那个周期找出不活跃的矿机记录删除掉的逻辑能够在重启WebApiServer服务进程后不中断
            VirtualRoot.BuildEventPath<Per2MinuteEvent>("周期找出用不活跃的矿机记录删除掉", LogEnum.DevConsole, message => {
                var clientDatas = _dicByObjectId.Values.ToArray();
                Dictionary<string, List<ClientData>> dicByMACAddress = new Dictionary<string, List<ClientData>>();
                DateTime minerClientExpireTime = message.BornOn.AddDays(-7);
                // 因为SpeedData尺寸较大，时效性较短，可以比CientData更早删除
                DateTime minerSpeedExpireTime = message.BornOn.AddMinutes(-3);
                int count = 0;
                foreach (var clientData in clientDatas) {
                    DateTime activeOn = clientData.GetActiveOn();
                    // 如果7天都没有活跃了
                    if (activeOn <= minerClientExpireTime) {
                        RemoveByObjectId(clientData.Id);
                        count++;
                    }
                    if (activeOn <= minerSpeedExpireTime && activeOn >= minerSpeedExpireTime.AddSeconds(-message.Seconds)) {
                        _speedDataRedis.DeleteByClientIdAsync(clientData.ClientId);
                    }
                    if (!string.IsNullOrEmpty(clientData.MACAddress)) {
                        if (!dicByMACAddress.TryGetValue(clientData.MACAddress, out List<ClientData> list)) {
                            list = new List<ClientData>();
                            dicByMACAddress.Add(clientData.MACAddress, list);
                        }
                        list.Add(clientData);
                    }
                }
                if (count > 0) {
                    NTMinerConsole.DevWarn($"{count.ToString()} 条不活跃的矿机记录被删除");
                }
                List<string> toRemoveIds = new List<string>();
                foreach (var kv in dicByMACAddress) {
                    if (kv.Value.Count > 1) {
                        toRemoveIds.AddRange(kv.Value.Select(a => a.Id));
                    }
                }
                if (toRemoveIds.Count > 0) {
                    RemoveByObjectIds(toRemoveIds);
                    NTMinerConsole.DevWarn($"{toRemoveIds.Count.ToString()} 条MAC地址重复的矿机记录被删除");
                }

                NTMinerConsole.DevDebug($"QueryClients平均耗时 {AverageQueryClientsMilliseconds.ToString()} 毫秒");
            }, this.GetType());
            // 收到Mq消息之前一定已经初始化完成，因为Mq消费者在ClientSetInitedEvent事件之后才会创建
            VirtualRoot.BuildEventPath<SpeedDatasMqEvent>("收到SpeedDatasMq消息后更新ClientData内存", LogEnum.None, path: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                if (message.ClientIdIps == null || message.ClientIdIps.Length == 0) {
                    return;
                }
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(nameof(SpeedDatasMqEvent) + ":" + MqKeyword.SafeIgnoreMessage);
                    return;
                }
                speedDataRedis.GetByClientIdsAsync(message.ClientIdIps.Select(a => a.ClientId).ToArray()).ContinueWith(t => {
                    if (t.Result != null && t.Result.Length != 0) {
                        foreach (var item in t.Result) {
                            string minerIp = string.Empty;
                            var clientIdIp = message.ClientIdIps.FirstOrDefault(a => a.ClientId == item.ClientId);
                            if (clientIdIp != null) {
                                minerIp = clientIdIp.MinerIp;
                            }
                            ReportSpeed(item.SpeedDto, minerIp, isFromWsServerNode: true);
                        }
                    }
                });
            }, this.GetType());
            VirtualRoot.BuildEventPath<MinerClientWsClosedMqEvent>("收到MinerClientWsClosedMq消息后更新NetActiveOn和IsOnline", LogEnum.None, path: message => {
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(nameof(MinerClientWsClosedMqEvent) + ":" + MqKeyword.SafeIgnoreMessage);
                    return;
                }
                if (_dicByClientId.TryGetValue(message.ClientId, out ClientData clientData)) {
                    clientData.NetActiveOn = message.Timestamp;
                    clientData.IsOnline = false;
                }
            }, this.GetType());
            VirtualRoot.BuildEventPath<MinerClientsWsBreathedMqEvent>("收到MinerClientsWsBreathedMq消息后更新NetActiveOn", LogEnum.None, path: message => {
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(nameof(MinerClientsWsBreathedMqEvent) + ":" + MqKeyword.SafeIgnoreMessage);
                    return;
                }
                if (message.ClientIds != null && message.ClientIds.Length != 0) {
                    foreach (var clientId in message.ClientIds) {
                        if (_dicByClientId.TryGetValue(clientId, out ClientData clientData)) {
                            clientData.NetActiveOn = message.Timestamp;
                            clientData.IsOnline = true;
                        }
                    }
                }
            }, this.GetType());
            VirtualRoot.BuildEventPath<MinerSignSetedMqEvent>("更新内存中的MinerData的MinerSign部分", LogEnum.None, path: message => {
                if (_dicByObjectId.TryGetValue(message.Data.Id, out ClientData clientData)) {
                    clientData.Update(message.Data);
                }
                else {
                    clientData = ClientData.Create(message.Data);
                    Add(clientData);
                }
                clientData.NetActiveOn = DateTime.Now;
                clientData.IsOnline = true;
                clientData.IsOuterUserEnabled = true;
            }, this.GetType());
            VirtualRoot.BuildCmdPath<QueryClientsForWsMqCommand>(path: message => {
                QueryClientsResponse response = AppRoot.QueryClientsForWs(message.Query);
                _mqSender.SendResponseClientsForWs(message.AppId, message.LoginName, message.SessionId, message.MqMessageId, response);
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
                if (speedDto.IsOuterUserEnabled && !string.IsNullOrEmpty(speedDto.ReportOuterUserId)) {
                    UserData userData = AppRoot.UserSet.GetUser(UserId.Create(speedDto.ReportOuterUserId));
                    if (userData != null) {
                        clientData.LoginName = userData.LoginName;
                    }
                }
                Add(clientData);
            }
            else {
                bool isLoginNameChanged = false;
                if (speedDto.IsOuterUserEnabled &&
                    !string.IsNullOrEmpty(speedDto.ReportOuterUserId)
                    && (string.IsNullOrEmpty(clientData.LoginName) || clientData.OuterUserId != speedDto.ReportOuterUserId)) {
                    UserData userData = AppRoot.UserSet.GetUser(UserId.Create(speedDto.ReportOuterUserId));
                    if (userData != null) {
                        isLoginNameChanged = true;
                        clientData.LoginName = userData.LoginName;
                    }
                }
                clientData.Update(speedDto, minerIp, out bool isMinerDataChanged);
                if (isMinerDataChanged || isLoginNameChanged) {
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
            _dicByClientId.TryAdd(clientData.ClientId, clientData);
            _dicByObjectId.TryAdd(clientData.Id, clientData);
        }

        protected override void DoUpdateSave(IEnumerable<MinerData> minerDatas) {
            if (!IsReadied) {
                return;
            }
            foreach (var minerData in minerDatas) {
                _minerRedis.SetAsync(minerData);
            }
        }

        protected override void DoRemoveSave(MinerData minerData) {
            if (!IsReadied) {
                return;
            }
            _minerRedis.DeleteAsync(minerData).ContinueWith(t => {
                _mqSender.SendMinerDataRemoved(minerData.Id, minerData.ClientId);
            });
            _clientActiveOnRedis.DeleteAsync(minerData.Id);
            _speedDataRedis.DeleteByClientIdAsync(minerData.ClientId);
        }

        protected override void DoRemoveSave(MinerData[] minerDatas) {
            if (!IsReadied) {
                return;
            }
            _minerRedis.DeleteAsync(minerDatas).ContinueWith(t => {
                _mqSender.SendMinerDatasRemoved(minerDatas.Select(a => a.ClientId).ToArray());
            });
            _clientActiveOnRedis.DeleteAsync(minerDatas.Select(a => a.Id).ToArray());
            _speedDataRedis.DeleteByClientIdAsync(minerDatas.Select(a => a.ClientId).ToArray());
        }
    }
}

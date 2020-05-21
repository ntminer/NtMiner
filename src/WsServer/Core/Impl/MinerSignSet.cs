using NTMiner.Core.MinerServer;
using NTMiner.Core.Redis;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class MinerSignSet : IMinerSignSet {
        private const string _safeIgnoreMessage = "该消息发生的时间早于本节点启动时间1分钟，安全忽略";
        private readonly Dictionary<string, MinerSign> _dicByMinerId = new Dictionary<string, MinerSign>();
        private readonly Dictionary<Guid, MinerSign> _dicByClientId = new Dictionary<Guid, MinerSign>();
        private DateTime _initedOn = DateTime.MinValue;
        public bool IsReadied {
            get; private set;
        }

        public MinerSignSet(IReadOnlyMinerRedis redis) {
            redis.GetAllAsync().ContinueWith(t => {
                _initedOn = DateTime.Now;
                foreach (var item in t.Result) {
                    Add(MinerSign.Create(item));
                }
                IsReadied = true;
                NTMinerConsole.UserOk("矿机签名集就绪");
                VirtualRoot.RaiseEvent(new MinerSignSetInitedEvent());
            });
            // 收到Mq消息之前一定已经初始化完成，因为Mq消费者在MinerSignSetInitedEvent事件之后才会创建
            VirtualRoot.AddEventPath<MinerDataRemovedMqMessage>("收到MinerClientRemovedMq消息后移除内存中对应的记录", LogEnum.None, action: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                if (string.IsNullOrEmpty(message.MinerId)) {
                    return;
                }
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(_safeIgnoreMessage);
                    return;
                }
                if (_dicByMinerId.TryGetValue(message.MinerId, out MinerSign minerSign)) {
                    _dicByMinerId.Remove(message.MinerId);
                    _dicByClientId.Remove(minerSign.ClientId);
                }
            }, this.GetType());
            VirtualRoot.AddEventPath<MinerDataAddedMqMessage>("收到MinerDataAddedMq消息后更新内存中对应的记录", LogEnum.None, action: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                if (string.IsNullOrEmpty(message.MinerId)) {
                    return;
                }
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(_safeIgnoreMessage);
                    return;
                }
                redis.GetByIdAsync(message.MinerId).ContinueWith(t => {
                    if (t.Result != null) {
                        if (_dicByMinerId.TryGetValue(message.MinerId, out MinerSign minerSign)) {
                            minerSign.Update(t.Result);
                        }
                        else {
                            Add(MinerSign.Create(t.Result));
                        }
                    }
                });
            }, this.GetType());
            VirtualRoot.AddEventPath<MinerSignChangedMqMessage>("收到MinerSignChangedMq消息后更新内存中对应的记录", LogEnum.None, action: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                if (string.IsNullOrEmpty(message.MinerId)) {
                    return;
                }
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(_safeIgnoreMessage);
                    return;
                }
                redis.GetByIdAsync(message.MinerId).ContinueWith(t => {
                    if (t.Result != null) {
                        if (_dicByMinerId.TryGetValue(message.MinerId, out MinerSign minerSign)) {
                            minerSign.Update(t.Result);
                        }
                        else {
                            Add(MinerSign.Create(t.Result));
                        }
                    }
                });
            }, this.GetType());
        }

        private bool IsOldMqMessage(DateTime mqMessageTimestamp) {
            // 考虑到服务器间时钟可能不完全同步，如果消息发生的时间比_initedOn的时间早了
            // 一分多钟则可以视为Init时已经包含了该Mq消息所表达的事情就不需要再访问Redis了
            if (mqMessageTimestamp.AddMinutes(1) < _initedOn) {
                return true;
            }
            return false;
        }

        private void Add(MinerSign minerSign) {
            if (!_dicByMinerId.ContainsKey(minerSign.Id)) {
                _dicByMinerId.Add(minerSign.Id, minerSign);
            }
            if (!_dicByClientId.ContainsKey(minerSign.ClientId)) {
                _dicByClientId.Add(minerSign.ClientId, minerSign);
            }
        }

        public bool TryGetByClientId(Guid clientId, out MinerSign minerSign) {
            minerSign = null;
            if (!IsReadied) {
                return false;
            }
            return _dicByClientId.TryGetValue(clientId, out minerSign);
        }

        public bool TryGetByMinerId(string minerId, out MinerSign minerSign) {
            minerSign = null;
            if (!IsReadied) {
                return false;
            }
            return _dicByMinerId.TryGetValue(minerId, out minerSign);
        }
    }
}

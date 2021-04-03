using NTMiner.Core.MinerServer;
using NTMiner.Core.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NTMiner.Core.Impl {
    public class MinerSignSet : IMinerSignSet {
        private readonly ConcurrentDictionary<Guid, MinerSign> _dicByClientId = new ConcurrentDictionary<Guid, MinerSign>();
        private DateTime _initedOn = DateTime.MinValue;
        public bool IsReadied {
            get; private set;
        }

        private readonly IMinerDataRedis _redis;
        public MinerSignSet(IMinerDataRedis redis) {
            _redis = redis;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            redis.GetAllAsync().ContinueWith(t => {
                _initedOn = DateTime.Now;
                foreach (var item in t.Result) {
                    _dicByClientId[item.ClientId] = MinerSign.Create(item);
                }
                IsReadied = true;
                stopwatch.Stop();
                NTMinerConsole.UserOk($"矿机签名集就绪，耗时 {stopwatch.GetElapsedSeconds().ToString("f2")} 秒");
                VirtualRoot.RaiseEvent(new MinerSignSetInitedEvent());
            });
            // 收到Mq消息之前一定已经初始化完成，因为Mq消费者在MinerSignSetInitedEvent事件之后才会创建
            VirtualRoot.BuildEventPath<MinerDataRemovedMqEvent>("收到MinerClientRemovedMq消息后移除内存中对应的记录", LogEnum.None, path: message => {
                #region
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(nameof(MinerDataRemovedMqEvent) + ":" + MqKeyword.SafeIgnoreMessage);
                    return;
                }
                if (_dicByClientId.TryRemove(message.ClientId, out MinerSign minerSign)) {
                    if (AppRoot.MinerClientSessionSet.TryGetByClientId(minerSign.ClientId, out IMinerClientSession ntminerSession)) {
                        ntminerSession.CloseAsync(WsCloseCode.Normal, "服务端移除了该矿机");
                    }
                }
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<MinerDatasRemovedMqEvent>("收到MinerClientsRemovedMq消息后移除内存中对应的记录", LogEnum.None, path: message => {
                #region
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(nameof(MinerDataRemovedMqEvent) + ":" + MqKeyword.SafeIgnoreMessage);
                    return;
                }
                foreach (var clientId in message.ClientIds) {
                    if (_dicByClientId.TryRemove(clientId, out MinerSign minerSign)) {
                        if (AppRoot.MinerClientSessionSet.TryGetByClientId(clientId, out IMinerClientSession ntminerSession)) {
                            ntminerSession.CloseAsync(WsCloseCode.Normal, "服务端移除了该矿机");
                        }
                    }
                }
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<Per1SecondEvent>("每秒钟将暂存的新设置的MinerSign发送到Mq", LogEnum.None, message => {
                Task.Factory.StartNew(() => {
                    MinerSign[] minerSignsSeted;
                    lock (_lockForMinerSignsSeted) {
                        minerSignsSeted = _minerSignsSeted.ToArray();
                        _minerSignsSeted.Clear();
                    }
                    AppRoot.MinerClientMqSender.SendMinerSignsSeted(minerSignsSeted);
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

        public bool TryGetByClientId(Guid clientId, out MinerSign minerSign) {
            minerSign = null;
            if (!IsReadied) {
                return false;
            }
            return _dicByClientId.TryGetValue(clientId, out minerSign);
        }

        private readonly List<MinerSign> _minerSignsSeted = new List<MinerSign>();
        private readonly object _lockForMinerSignsSeted = new object();
        public void SetMinerSign(MinerSign minerSign) {
            _dicByClientId[minerSign.ClientId] = minerSign;
            _redis.UpdateAsync(minerSign);
            lock (_lockForMinerSignsSeted) {
                _minerSignsSeted.Add(minerSign);
            }
        }
    }
}

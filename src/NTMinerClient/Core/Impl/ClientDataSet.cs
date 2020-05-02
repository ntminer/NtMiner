using LiteDB;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Core.Impl {
    public class ClientDataSet : ClientDataSetBase, IClientDataSet {
        private DateTime _getSpeedOn = DateTime.MinValue;
        public ClientDataSet() : base(isPull: true, getDatas: callback => {
            Task.Factory.StartNew(() => {
                using (LiteDatabase db = CreateLocalDb()) {
                    var col = db.GetCollection<MinerData>();
                    callback?.Invoke(col.FindAll().Select(a => ClientData.Create(a)));
                }
            });
        }) {
            VirtualRoot.AddOnecePath<ClientSetInitedEvent>("矿机集初始化后开始拉取矿机算力的进程", LogEnum.None, action: message => {
                PullSpeedInit();
            }, PathId.Empty, this.GetType());
        }

        private static LiteDatabase CreateLocalDb() {
            return new LiteDatabase($"filename={HomePath.LocalDbFileFullName}");
        }

        public void AddClient(string minerIp) {
            if (!IsReadied) {
                return;
            }
            MinerData minerData = MinerData.Create(minerIp);
            var clientData = ClientData.Create(minerData);
            if (!_dicByObjectId.ContainsKey(clientData.Id)) {
                _dicByObjectId.Add(clientData.Id, clientData);
            }
            if (!_dicByClientId.ContainsKey(clientData.ClientId)) {
                _dicByClientId.Add(clientData.ClientId, clientData);
            }
            DoUpdateSave(minerData);
        }

        protected override void DoRemoveSave(MinerData minerData) {
            using (LiteDatabase db = CreateLocalDb()) {
                var col = db.GetCollection<MinerData>();
                col.Delete(minerData.Id);
            }
        }

        private bool _isPullSpeedInited = false;
        private void PullSpeedInit() {
            if (_isPullSpeedInited) {
                return;
            }
            _isPullSpeedInited = true;
            Task.Factory.StartNew(() => {
                while (true) {
                    DateTime now = DateTime.Now;
                    if (_getSpeedOn.AddSeconds(10) <= now) {
                        Write.DevDebug("周期拉取数据更新拍照源数据");
                        ClientData[] clientDatas = _dicByObjectId.Values.ToArray();
                        Task[] tasks = clientDatas.Select(CreatePullTask).ToArray();
                        Task.WaitAll(tasks, 5 * 1000);
                        _getSpeedOn = now;

                        #region 将NoSpeedSeconds秒内更新过的记录持久化到磁盘
                        List<MinerData> minerDatas = new List<MinerData>();
                        DateTime time = now.AddSeconds(-20);
                        foreach (var clientData in _dicByObjectId.Values) {
                            if (clientData.MinerActiveOn > time) {
                                minerDatas.Add(MinerData.Create(clientData));
                            }
                            else {
                                clientData.IsMining = false;
                                clientData.MainCoinSpeed = 0;
                                clientData.DualCoinSpeed = 0;
                                foreach (var item in clientData.GpuTable) {
                                    item.MainCoinSpeed = 0;
                                    item.DualCoinSpeed = 0;
                                }
                            }
                        }
                        if (minerDatas.Count > 0) {
                            DoUpdateSave(minerDatas);
                        }
                        #endregion
                    }
                    else {
                        System.Threading.Thread.Sleep((int)(_getSpeedOn.AddSeconds(10) - now).TotalMilliseconds);
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        private Task CreatePullTask(ClientData clientData) {
            return Task.Factory.StartNew(() => {
                JsonRpcRoot.Client.MinerClientService.GetSpeedAsync(clientData, (speedData, exception) => {
                    if (exception != null) {
                        clientData.IsOnline = false;
                        clientData.IsMining = false;
                        clientData.MainCoinSpeed = 0;
                        clientData.DualCoinSpeed = 0;
                        foreach (var item in clientData.GpuTable) {
                            item.MainCoinSpeed = 0;
                            item.DualCoinSpeed = 0;
                        }
                    }
                    else {
                        if (speedData.ClientId != clientData.ClientId) {
                            _dicByClientId.Remove(clientData.ClientId);
                            _dicByClientId.Add(speedData.ClientId, clientData);
                        }
                        clientData.NetActiveOn = DateTime.Now;
                        clientData.IsOnline = true;
                        clientData.Update(speedData, out _);
                    }
                });
            });
        }

        protected override void DoUpdateSave(MinerData minerData) {
            using (LiteDatabase db = CreateLocalDb()) {
                var col = db.GetCollection<MinerData>();
                col.Upsert(minerData);
            }
        }

        protected override void DoUpdateSave(IEnumerable<MinerData> minerDatas) {
            using (LiteDatabase db = CreateLocalDb()) {
                var col = db.GetCollection<MinerData>();
                col.Upsert(minerDatas);
            }
        }
    }
}

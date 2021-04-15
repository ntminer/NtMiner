using LiteDB;
using NTMiner.Core.MinerServer;
using NTMiner.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Core.Impl {
    public class ClientDataSet : ClientDataSetBase, IClientDataSet {
        public ClientDataSet() : base(isPull: true, getDatas: callback => {
            Task.Factory.StartNew(() => {
                using (LiteDatabase db = CreateLocalDb()) {
                    var col = db.GetCollection<MinerData>();
                    callback?.Invoke(col.FindAll().Select(a => ClientData.Create(a)));
                }
            });
        }) {
            VirtualRoot.BuildOnecePath<ClientSetInitedEvent>("开始拉取矿机算力的进程", LogEnum.None, PathId.Empty, this.GetType(), PathPriority.Normal, path: message => {
                PullSpeedInit();
            });
        }

        private static LiteDatabase CreateLocalDb() {
            return new LiteDatabase($"filename={HomePath.LocalDbFileFullName}");
        }

        public void AddClient(string minerIp) {
            if (!IsReadied) {
                return;
            }
            var clientData = ClientData.Create(minerIp, out MinerData minerData);
            _dicByObjectId.TryAdd(clientData.Id, clientData);
            // 因为ClientId是服务端随机生成的，所以需要等待获取挖矿端的ClientId
            DoUpdateSave(minerData);
        }

        protected override void DoRemoveSave(IMinerData minerData) {
            using (LiteDatabase db = CreateLocalDb()) {
                var col = db.GetCollection<MinerData>();
                col.Delete(minerData.Id);
            }
        }

        protected override void DoRemoveSave(IEnumerable<IMinerData> minerDatas) {
            if (minerDatas == null) {
                return;
            }
            using (LiteDatabase db = CreateLocalDb()) {
                var col = db.GetCollection<MinerData>();
                foreach (var item in minerDatas) {
                    col.Delete(item.Id);
                }
            }
        }

        private bool _isPullSpeedInited = false;
        private void PullSpeedInit() {
            if (_isPullSpeedInited) {
                return;
            }
            _isPullSpeedInited = true;
            VirtualRoot.BuildEventPath<Per10SecondEvent>("周期拉取矿机状态", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, message => {
                if (RpcRoot.IsOuterNet) {
                    return;
                }
                Task.Factory.StartNew(() => {
                    NTMinerConsole.DevDebug("周期拉取数据更新拍照源数据");
                    ClientData[] clientDatas = _dicByObjectId.Values.ToArray();
                    Task[] tasks = clientDatas.Select(CreatePullTask).ToArray();
                    Task.WaitAll(tasks, 5 * 1000);

                    if (RpcRoot.IsOuterNet) {
                        return;
                    }

                    #region 将NoSpeedSeconds秒内更新过的记录持久化到磁盘
                    List<MinerData> minerDatas = new List<MinerData>();
                    DateTime time = message.BornOn.AddSeconds(-20);
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
                });
            });
        }

        private Task CreatePullTask(ClientData clientData) {
            return Task.Factory.StartNew(() => {
                RpcRoot.Client.MinerClientService.GetSpeedAsync(clientData, (speedData, exception) => {
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
                            if (_dicByClientId.TryGetValue(clientData.ClientId, out ClientData value)) {
                                _dicByClientId.TryRemove(clientData.ClientId, out _);
                            }
                            if (_dicByClientId.TryAdd(speedData.ClientId, clientData)) {
                                _dicByObjectId.TryAdd(clientData.Id, clientData);
                            }
                        }
                        clientData.NetActiveOn = DateTime.Now;
                        clientData.IsOnline = true;
                        clientData.Update(new SpeedData(speedData, DateTime.Now), out _);
                    }
                });
            });
        }

        protected override void DoUpdateSave(IEnumerable<MinerData> minerDatas) {
            using (LiteDatabase db = CreateLocalDb()) {
                var col = db.GetCollection<MinerData>();
                col.Upsert(minerDatas);
            }
        }
    }
}

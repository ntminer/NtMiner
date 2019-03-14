using LiteDB;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Data.Impl {
    public class CoinSnapshotSet : ICoinSnapshotSet {
        private readonly IHostRoot _root;
        // 内存中保留最近20分钟的快照
        private readonly List<CoinSnapshotData> _dataList = new List<CoinSnapshotData>();
        private DateTime _snapshotOn = DateTime.Now;
        internal CoinSnapshotSet(IHostRoot root) {
            _root = root;
            Snapshot();
            VirtualRoot.On<Per1MinuteEvent>(
                "周期性拍摄快照",
                LogEnum.Console,
                action: message => {
                    DateTime time = DateTime.Now.AddMinutes(-20);
                    List<CoinSnapshotData> toRemoves = _dataList.Where(a => a.Timestamp < time).ToList();
                    foreach (var item in toRemoves) {
                        _dataList.Remove(item);
                    }
                });
        }

        private void Snapshot() {
            Task.Factory.StartNew(() => {
                while (true) {
                    DateTime now = DateTime.Now;
                    if (_snapshotOn.AddSeconds(10) <= now) {
                        Snapshot(now);
                        _snapshotOn = now;
                    }
                    else {
                        System.Threading.Thread.Sleep((int)(_snapshotOn.AddSeconds(10) - now).TotalMilliseconds);
                    }
                }
            });
        }

        private bool _isInited = false;
        private readonly object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    // 将最近20分钟的快照载入内存
                    DateTime now = DateTime.Now;
                    using (LiteDatabase db = HostRoot.CreateReportDb()) {
                        var col = db.GetCollection<CoinSnapshotData>();
                        col.EnsureIndex(nameof(CoinSnapshotData.Timestamp), unique: false);
                        foreach (var item in col.Find(
                            Query.And(
                                Query.GT(nameof(CoinSnapshotData.Timestamp), now.AddMinutes(-20)),
                                Query.LTE(nameof(CoinSnapshotData.Timestamp), now)))) {
                            _dataList.Add(item);
                        }
                    }
                    Write.DevLine("将最近20分钟的快照载入内存");
                    _isInited = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Snapshot(DateTime now) {
            InitOnece();
            try {
                Dictionary<string, CoinSnapshotData> dicByCoinCode = new Dictionary<string, CoinSnapshotData>();
                foreach (var clientData in _root.ClientSet) {
                    if (clientData.ModifiedOn.AddMinutes(3) < now) {
                        continue;
                    }

                    if (string.IsNullOrEmpty(clientData.MainCoinCode)) {
                        continue;
                    }

                    CoinSnapshotData mainCoinSnapshotData;
                    if (!dicByCoinCode.TryGetValue(clientData.MainCoinCode, out mainCoinSnapshotData)) {
                        mainCoinSnapshotData = new CoinSnapshotData() {
                            Id = ObjectId.NewObjectId(),
                            Timestamp = now,
                            CoinCode = clientData.MainCoinCode
                        };
                        dicByCoinCode.Add(clientData.MainCoinCode, mainCoinSnapshotData);
                    }

                    if (clientData.IsMining) {
                        mainCoinSnapshotData.MainCoinMiningCount += 1;
                        mainCoinSnapshotData.Speed += clientData.MainCoinSpeed;
                        mainCoinSnapshotData.ShareDelta += clientData.GetMainCoinShareDelta();
                        mainCoinSnapshotData.RejectShareDelta += clientData.GetMainCoinRejectShareDelta();
                    }

                    mainCoinSnapshotData.MainCoinOnlineCount += 1;

                    if (!string.IsNullOrEmpty(clientData.DualCoinCode) && clientData.IsDualCoinEnabled) {
                        CoinSnapshotData dualCoinSnapshotData;
                        if (!dicByCoinCode.TryGetValue(clientData.DualCoinCode, out dualCoinSnapshotData)) {
                            dualCoinSnapshotData = new CoinSnapshotData() {
                                Id = ObjectId.NewObjectId(),
                                Timestamp = now,
                                CoinCode = clientData.DualCoinCode
                            };
                            dicByCoinCode.Add(clientData.DualCoinCode, dualCoinSnapshotData);
                        }

                        if (clientData.IsMining) {
                            dualCoinSnapshotData.DualCoinMiningCount += 1;
                            dualCoinSnapshotData.Speed += clientData.DualCoinSpeed;
                            dualCoinSnapshotData.ShareDelta += clientData.GetDualCoinShareDelta();
                            dualCoinSnapshotData.RejectShareDelta += clientData.GetDualCoinRejectShareDelta();
                        }

                        dualCoinSnapshotData.DualCoinOnlineCount += 1;
                    }
                }

                if (dicByCoinCode.Count > 0) {
                    _dataList.AddRange(dicByCoinCode.Values);
                    using (LiteDatabase db = HostRoot.CreateReportDb()) {
                        var col = db.GetCollection<CoinSnapshotData>();
                        col.Insert(dicByCoinCode.Values);
                    }
                    Write.DevLine("拍摄快照" + dicByCoinCode.Count + "张，快照时间戳：" + now.ToString("yyyy-MM-dd HH:mm:ss fff"));
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        public List<CoinSnapshotData> GetLatestSnapshots(
            int limit,
            out int totalMiningCount,
            out int totalOnlineCount) {
            InitOnece();
            ClientCount count = HostRoot.Current.ClientSet.Count();
            totalMiningCount = count.MiningCount;
            totalOnlineCount = count.OnlineCount;
            DateTime rightTime = DateTime.Now.AddSeconds(-5);
            DateTime leftTime = rightTime.AddSeconds(-limit * 10 - 5);
            List<CoinSnapshotData> results = _dataList.Where(a => a.Timestamp > leftTime && a.Timestamp <= rightTime).OrderByDescending(a => a.Timestamp).ToList();
            return results;
        }
    }
}

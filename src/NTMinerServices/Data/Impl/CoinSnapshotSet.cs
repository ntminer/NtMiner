using LiteDB;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Data.Impl {
    public class CoinSnapshotSet : ICoinSnapshotSet {
        private readonly IHostRoot _root;
        // 内存中保留最近20分钟的快照
        private readonly List<CoinSnapshotData> _dataList = new List<CoinSnapshotData>();
        internal CoinSnapshotSet(IHostRoot root) {
            _root = root;
            VirtualRoot.BuildEventPath<Per10SecondEvent>("周期性拍摄快照", LogEnum.DevConsole,
                action: message => {
                    Snapshot(message.BornOn);
                });
            VirtualRoot.BuildEventPath<Per2MinuteEvent>("周期性拍摄快照", LogEnum.DevConsole,
                action: message => {
                    DateTime time = DateTime.Now.AddMinutes(-20);
                    List<CoinSnapshotData> toRemoves = _dataList.Where(a => a.Timestamp < time).ToList();
                    foreach (var item in toRemoves) {
                        _dataList.Remove(item);
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
                    Write.DevDebug("将最近20分钟的快照载入内存");
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
                int onlineCount = 0;
                int miningCount = 0;
                Dictionary<string, CoinSnapshotData> dicByCoinCode = new Dictionary<string, CoinSnapshotData>();
                foreach (var clientData in _root.ClientSet.AsEnumerable()) {
                    if (HostRoot.Instance.HostConfig.IsPull) {
                        if (clientData.ModifiedOn.AddSeconds(15) < now) {
                            continue;
                        }
                    }
                    else {
                        if (clientData.ModifiedOn.AddSeconds(130) < now) {
                            continue;
                        }
                    }

                    onlineCount++;

                    if (string.IsNullOrEmpty(clientData.MainCoinCode)) {
                        continue;
                    }

                    CoinSnapshotData mainCoinSnapshotData;
                    if (!dicByCoinCode.TryGetValue(clientData.MainCoinCode, out mainCoinSnapshotData)) {
                        mainCoinSnapshotData = new CoinSnapshotData() {
                            Timestamp = now,
                            CoinCode = clientData.MainCoinCode
                        };
                        dicByCoinCode.Add(clientData.MainCoinCode, mainCoinSnapshotData);
                    }

                    if (clientData.IsMining) {
                        miningCount++;
                        mainCoinSnapshotData.MainCoinMiningCount += 1;
                        mainCoinSnapshotData.Speed += clientData.MainCoinSpeed;
                        mainCoinSnapshotData.ShareDelta += clientData.GetMainCoinShareDelta(HostRoot.Instance.HostConfig.IsPull);
                        mainCoinSnapshotData.RejectShareDelta += clientData.GetMainCoinRejectShareDelta(HostRoot.Instance.HostConfig.IsPull);
                    }

                    mainCoinSnapshotData.MainCoinOnlineCount += 1;

                    if (!string.IsNullOrEmpty(clientData.DualCoinCode) && clientData.IsDualCoinEnabled) {
                        CoinSnapshotData dualCoinSnapshotData;
                        if (!dicByCoinCode.TryGetValue(clientData.DualCoinCode, out dualCoinSnapshotData)) {
                            dualCoinSnapshotData = new CoinSnapshotData() {
                                Timestamp = now,
                                CoinCode = clientData.DualCoinCode
                            };
                            dicByCoinCode.Add(clientData.DualCoinCode, dualCoinSnapshotData);
                        }

                        if (clientData.IsMining) {
                            dualCoinSnapshotData.DualCoinMiningCount += 1;
                            dualCoinSnapshotData.Speed += clientData.DualCoinSpeed;
                            dualCoinSnapshotData.ShareDelta += clientData.GetDualCoinShareDelta(HostRoot.Instance.HostConfig.IsPull);
                            dualCoinSnapshotData.RejectShareDelta += clientData.GetDualCoinRejectShareDelta(HostRoot.Instance.HostConfig.IsPull);
                        }

                        dualCoinSnapshotData.DualCoinOnlineCount += 1;
                    }
                }

                HostRoot.ClientCount.Update(onlineCount, miningCount);
                if (dicByCoinCode.Count > 0) {
                    _dataList.AddRange(dicByCoinCode.Values);
                    using (LiteDatabase db = HostRoot.CreateReportDb()) {
                        var col = db.GetCollection<CoinSnapshotData>();
                        col.Insert(dicByCoinCode.Values);
                    }
                    Write.DevDebug("拍摄快照" + dicByCoinCode.Count + "张，快照时间戳：" + now.ToString("yyyy-MM-dd HH:mm:ss fff"));
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        public List<CoinSnapshotData> GetLatestSnapshots(
            int limit,
            out int totalMiningCount,
            out int totalOnlineCount) {
            InitOnece();
            ClientCount count = HostRoot.ClientCount;
            totalMiningCount = count.MiningCount;
            totalOnlineCount = count.OnlineCount;
            DateTime rightTime = DateTime.Now.AddSeconds(-5);
            DateTime leftTime = rightTime.AddSeconds(-limit * 10 - 5);
            List<CoinSnapshotData> results = _dataList.Where(a => a.Timestamp > leftTime && a.Timestamp <= rightTime).OrderByDescending(a => a.Timestamp).ToList();
            return results;
        }
    }
}

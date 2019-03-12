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
            VirtualRoot.On<Per10SecondEvent>(
                "周期性拍摄快照",
                LogEnum.Console,
                action: message => {
                    DateTime leftTime = message.Timestamp.AddSeconds(-message.Seconds);
                    Snapshot(leftTime, seconds: 10);
                });
        }

        private bool _isInited = false;
        private object _locker = new object();

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

        private static readonly Dictionary<string, int> STotalShareByCoin = new Dictionary<string, int>();
        private static readonly Dictionary<string, int> SRejectShareByCoin = new Dictionary<string, int>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftTime"></param>
        /// <param name="seconds">快照周期：秒</param>
        private void Snapshot(DateTime leftTime, int seconds) {
            InitOnece();
            if (seconds > 120) {
                throw new InvalidProgramException("客户端每两分钟上报一次数据，所以快照拍摄周期不能大于2分钟，否则一个快照中同一个人可能出现两次");
            }
            DateTime rightTime = leftTime.AddSeconds(seconds);
            Write.DevLine($"快照时间区间{{{leftTime} - {rightTime}]");
            DateTime now = DateTime.Now;
            if (rightTime <= now) {
                try {
                    Dictionary<string, int> totalShareDic = new Dictionary<string, int>();
                    Dictionary<string, int> rejectShareDic = new Dictionary<string, int>();
                    Dictionary<string, CoinSnapshotData> dicByCoinCode = new Dictionary<string, CoinSnapshotData>();
                    foreach (var clientData in _root.ClientSet) {
                        if (clientData.ModifiedOn.AddMinutes(3) < now) {
                            continue;
                        }

                        if (string.IsNullOrEmpty(clientData.MainCoinCode)) {
                            continue;
                        }

                        if (!STotalShareByCoin.ContainsKey(clientData.MainCoinCode)) {
                            STotalShareByCoin.Add(clientData.MainCoinCode, 0);
                        }

                        if (!SRejectShareByCoin.ContainsKey(clientData.MainCoinCode)) {
                            SRejectShareByCoin.Add(clientData.MainCoinCode, 0);
                        }

                        if (!totalShareDic.ContainsKey(clientData.MainCoinCode)) {
                            totalShareDic.Add(clientData.MainCoinCode, 0);
                        }

                        if (!rejectShareDic.ContainsKey(clientData.MainCoinCode)) {
                            rejectShareDic.Add(clientData.MainCoinCode, 0);
                        }

                        CoinSnapshotData mainCoinSnapshotData;
                        if (!dicByCoinCode.TryGetValue(clientData.MainCoinCode, out mainCoinSnapshotData)) {
                            mainCoinSnapshotData = new CoinSnapshotData() {
                                Id = ObjectId.NewObjectId(),
                                Timestamp = rightTime,
                                CoinCode = clientData.MainCoinCode
                            };
                            dicByCoinCode.Add(clientData.MainCoinCode, mainCoinSnapshotData);
                        }

                        if (clientData.IsMining) {
                            mainCoinSnapshotData.MainCoinMiningCount += 1;
                            mainCoinSnapshotData.Speed += clientData.MainCoinSpeed;
                            totalShareDic[clientData.MainCoinCode] += clientData.MainCoinTotalShare;
                            rejectShareDic[clientData.MainCoinCode] += clientData.MainCoinRejectShare;
                        }

                        mainCoinSnapshotData.MainCoinOnlineCount += 1;

                        if (!string.IsNullOrEmpty(clientData.DualCoinCode) && clientData.IsDualCoinEnabled) {
                            if (!STotalShareByCoin.ContainsKey(clientData.DualCoinCode)) {
                                STotalShareByCoin.Add(clientData.DualCoinCode, 0);
                            }

                            if (!SRejectShareByCoin.ContainsKey(clientData.DualCoinCode)) {
                                SRejectShareByCoin.Add(clientData.DualCoinCode, 0);
                            }

                            if (!totalShareDic.ContainsKey(clientData.DualCoinCode)) {
                                totalShareDic.Add(clientData.DualCoinCode, 0);
                            }

                            if (!rejectShareDic.ContainsKey(clientData.DualCoinCode)) {
                                rejectShareDic.Add(clientData.DualCoinCode, 0);
                            }

                            CoinSnapshotData dualCoinSnapshotData;
                            if (!dicByCoinCode.TryGetValue(clientData.DualCoinCode, out dualCoinSnapshotData)) {
                                dualCoinSnapshotData = new CoinSnapshotData() {
                                    Id = ObjectId.NewObjectId(),
                                    Timestamp = rightTime,
                                    CoinCode = clientData.DualCoinCode
                                };
                                dicByCoinCode.Add(clientData.DualCoinCode, dualCoinSnapshotData);
                            }

                            if (clientData.IsMining) {
                                dualCoinSnapshotData.DualCoinMiningCount += 1;
                                dualCoinSnapshotData.Speed += clientData.DualCoinSpeed;
                                totalShareDic[clientData.DualCoinCode] += clientData.DualCoinTotalShare;
                                rejectShareDic[clientData.DualCoinCode] += clientData.DualCoinRejectShare;
                            }

                            dualCoinSnapshotData.DualCoinOnlineCount += 1;
                        }
                    }

                    foreach (var item in dicByCoinCode.Values) {
                        item.ShareDelta = (totalShareDic[item.CoinCode] - rejectShareDic[item.CoinCode]) -
                            (STotalShareByCoin[item.CoinCode] - SRejectShareByCoin[item.CoinCode]);
                        item.RejectShareDelta = rejectShareDic[item.CoinCode] - SRejectShareByCoin[item.CoinCode];
                        STotalShareByCoin[item.CoinCode] = totalShareDic[item.CoinCode];
                        SRejectShareByCoin[item.CoinCode] = rejectShareDic[item.CoinCode];
                    }
                    DateTime time = rightTime.AddMinutes(-20);
                    List<CoinSnapshotData> toRemoves = _dataList.Where(a => a.Timestamp < time).ToList();
                    foreach (var item in toRemoves) {
                        _dataList.Remove(item);
                    }
                    if (dicByCoinCode.Count > 0) {
                        using (LiteDatabase db = HostRoot.CreateReportDb()) {
                            var col = db.GetCollection<CoinSnapshotData>();
                            col.Insert(dicByCoinCode.Values);
                        }
                        Write.DevLine("拍摄快照" + dicByCoinCode.Count + "张，快照时间戳：" + rightTime.ToString("yyyy-MM-dd HH:mm:ss fff"));
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                }
            }
            else {
                Write.DevLine("快照已拍摄到当前时间");
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
            List<CoinSnapshotData> results;
            DateTime rightTime = DateTime.Now;
            DateTime leftTime = rightTime.AddSeconds(-limit * 10 - 10);
            if (leftTime > HostRoot.Current.StartedOn) {
                lock (_locker) {
                    results = _dataList.Where(a => a.Timestamp > leftTime && a.Timestamp <= rightTime).OrderByDescending(a => a.Timestamp).ToList();
                }
            }
            else {
                using (LiteDatabase db = HostRoot.CreateReportDb()) {
                    var col = db.GetCollection<CoinSnapshotData>();
                    results = col.Find(
                        Query.And(
                            Query.GT(nameof(CoinSnapshotData.Timestamp), leftTime),
                            Query.LTE(nameof(CoinSnapshotData.Timestamp), rightTime))).OrderByDescending(a => a.Timestamp).ToList();
                }
            }
            return results;
        }
    }
}

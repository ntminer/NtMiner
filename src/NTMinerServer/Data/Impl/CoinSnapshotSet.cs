using LiteDB;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Data.Impl {
    public class CoinSnapshotSet : ICoinSnapshotSet {
        private class CoinShare {
            public CoinShare(int totalShare, int rejectShare, DateTime time) {
                this.TotalShare = totalShare;
                this.RejectShare = rejectShare;
                this.Time = time;
            }

            public void Update(ShareData data, DateTime time) {
                this.TotalShare = data.TotalShare;
                this.RejectShare = data.RejectShare;
                this.Time = time;
            }

            public DateTime Time { get; private set; }
            public int TotalShare { get; private set; }
            public int RejectShare { get; private set; }
        }

        public class ShareData {
            public ShareData(int totalShare, int rejectShare) {
                TotalShare = totalShare;
                RejectShare = rejectShare;
            }

            public int TotalShare;
            public int RejectShare;
        }

        private readonly IHostRoot _root;
        // 内存中保留最近20分钟的快照
        private readonly List<CoinSnapshotData> _dataList = new List<CoinSnapshotData>();

        internal CoinSnapshotSet(IHostRoot root) {
            _root = root;
            VirtualRoot.On<Per10SecondEvent>(
                "周期性拍摄快照",
                LogEnum.Console,
                action: message => {
                    Snapshot(message.Timestamp);
                    DateTime time = message.Timestamp.AddMinutes(-20);
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
                    Write.DevLine("将最近20分钟的快照载入内存");
                    _isInited = true;
                }
            }
        }

        private static readonly Dictionary<string, CoinShare> SShareDicByCoin = new Dictionary<string, CoinShare>();
        /// <summary>
        /// 
        /// </summary>
        private void Snapshot(DateTime now) {
            InitOnece();
            try {
                Dictionary<string, ShareData> shareDicByCoin = new Dictionary<string, ShareData>();
                Dictionary<string, CoinSnapshotData> dicByCoinCode = new Dictionary<string, CoinSnapshotData>();
                foreach (var clientData in _root.ClientSet) {
                    if (clientData.ModifiedOn.AddMinutes(3) < now) {
                        continue;
                    }

                    if (string.IsNullOrEmpty(clientData.MainCoinCode)) {
                        continue;
                    }

                    if (!SShareDicByCoin.ContainsKey(clientData.MainCoinCode)) {
                        SShareDicByCoin.Add(clientData.MainCoinCode, new CoinShare(0, 0, now));
                    }

                    if (!shareDicByCoin.ContainsKey(clientData.MainCoinCode)) {
                        shareDicByCoin.Add(clientData.MainCoinCode, new ShareData(0, 0));
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
                        ShareData shareData = shareDicByCoin[clientData.MainCoinCode];
                        shareData.TotalShare += clientData.MainCoinTotalShare;
                        shareData.RejectShare += clientData.MainCoinRejectShare;
                    }

                    mainCoinSnapshotData.MainCoinOnlineCount += 1;

                    if (!string.IsNullOrEmpty(clientData.DualCoinCode) && clientData.IsDualCoinEnabled) {
                        if (!SShareDicByCoin.ContainsKey(clientData.DualCoinCode)) {
                            SShareDicByCoin.Add(clientData.DualCoinCode, new CoinShare(0, 0, now));
                        }

                        if (!shareDicByCoin.ContainsKey(clientData.DualCoinCode)) {
                            shareDicByCoin.Add(clientData.DualCoinCode, new ShareData(0, 0));
                        }

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
                            ShareData shareData = shareDicByCoin[clientData.DualCoinCode];
                            shareData.TotalShare += clientData.DualCoinTotalShare;
                            shareData.RejectShare += clientData.DualCoinRejectShare;
                        }

                        dualCoinSnapshotData.DualCoinOnlineCount += 1;
                    }
                }

                foreach (var item in dicByCoinCode.Values) {
                    CoinShare oldShare = SShareDicByCoin[item.CoinCode];
                    ShareData shareData = shareDicByCoin[item.CoinCode];
                    if (oldShare.Time.AddSeconds(60) > now) {
                        int preShare = oldShare.TotalShare - oldShare.RejectShare;
                        // 如果没有preShare说明是第一次拍照，此时不计算shareDelta
                        if (preShare != 0) {
                            item.ShareDelta = shareData.TotalShare - shareData.RejectShare - preShare;
                            item.RejectShareDelta = shareData.RejectShare - oldShare.RejectShare;
                        }
                    }
                    oldShare.Update(shareData, now);
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

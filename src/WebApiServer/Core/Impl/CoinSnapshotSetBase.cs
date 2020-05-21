using LiteDB;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NTMiner.Core.Impl {
    public abstract class CoinSnapshotSetBase : ICoinSnapshotSet {
        // 内存中保留最近20分钟的快照
        private readonly List<CoinSnapshotData> _dataList = new List<CoinSnapshotData>();
        private readonly IClientDataSetBase _clientSet;
        private readonly bool _isPull;
        public CoinSnapshotSetBase(bool isPull, IClientDataSetBase clientSet) {
            _isPull = isPull;
            _clientSet = clientSet;
            LogEnum logType = isPull ? LogEnum.None : LogEnum.UserConsole;
            VirtualRoot.AddEventPath<Per10SecondEvent>("周期性拍摄快照", logType,
                action: message => {
                    Snapshot(message.BornOn);
                }, location: this.GetType());
            VirtualRoot.AddEventPath<Per2MinuteEvent>("周期性移除内存中20分钟前的快照", logType,
                action: message => {
                    DateTime time = message.BornOn.AddMinutes(-20);
                    var toRemoves = _dataList.Where(a => a.Timestamp < time).ToArray();
                    foreach (var item in toRemoves) {
                        _dataList.Remove(item);
                    }
                }, location: this.GetType());
            VirtualRoot.AddEventPath<HasBoot1MinuteEvent>("启动一会儿后清理一下很长时间之前的算力报告数据库文件", logType, action: message => {
                ClearReportDbFiles(message.BornOn);
            }, this.GetType());
            VirtualRoot.AddEventPath<Per24HourEvent>("每24小时清理一下很长时间之前的算力报告数据库文件", logType, action: message => {
                ClearReportDbFiles(message.BornOn);
            }, this.GetType());
        }

        private void ClearReportDbFiles(DateTime now) {
            try {
                foreach (var fileFullName in Directory.GetFiles(Path.Combine(HomePath.AppDomainBaseDirectory), "report????-??-??.litedb")) {
                    string yyyyMMdd = fileFullName.Substring(fileFullName.Length - "????-??-??.litedb".Length, "????-??-??".Length);
                    if (DateTime.TryParse(yyyyMMdd, out DateTime dateTime)) {
                        if (dateTime.AddDays(10) < now) {
                            try {
                                File.Delete(fileFullName);
                            }
                            catch {
                            }
                        }
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static LiteDatabase CreateReportDb() {
            string dbFileFullName = Path.Combine(HomePath.HomeDirFullName, $"report{DateTime.Now.ToString("yyyy-MM-dd")}.litedb");
            return new LiteDatabase($"filename={dbFileFullName}");
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
                    using (LiteDatabase db = CreateReportDb()) {
                        var col = db.GetCollection<CoinSnapshotData>();
                        col.EnsureIndex(nameof(CoinSnapshotData.Timestamp), unique: false);
                        foreach (var item in col.Find(
                            Query.And(
                                Query.GT(nameof(CoinSnapshotData.Timestamp), now.AddMinutes(-20)),
                                Query.LTE(nameof(CoinSnapshotData.Timestamp), now)))) {
                            _dataList.Add(item);
                        }
                    }
                    Write.UserLine("将最近20分钟的快照载入内存", _isPull ? MessageType.Debug : MessageType.Info);
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
                var coinSnapshots = VirtualRoot.CreateCoinSnapshots(_isPull, now, _clientSet.AsEnumerable().ToArray(), out int onlineCount, out int miningCount);

                _clientSet.ClientCount.Update(onlineCount, miningCount);
                if (coinSnapshots.Count > 0) {
                    _dataList.AddRange(coinSnapshots);
                    using (LiteDatabase db = CreateReportDb()) {
                        var col = db.GetCollection<CoinSnapshotData>();
                        col.Insert(coinSnapshots);
                    }
                    Write.UserLine("拍摄快照" + coinSnapshots.Count + "张，快照时间戳：" + now.ToString("yyyy-MM-dd HH:mm:ss fff"), _isPull ? MessageType.Debug : MessageType.Info);
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
            var count = _clientSet.ClientCount;
            totalMiningCount = count.MiningCount;
            totalOnlineCount = count.OnlineCount;
            DateTime rightTime = DateTime.Now.AddSeconds(-5);
            DateTime leftTime = rightTime.AddSeconds(-limit * 10 - 5);
            List<CoinSnapshotData> results = _dataList.Where(a => a.Timestamp > leftTime && a.Timestamp <= rightTime).OrderByDescending(a => a.Timestamp).ToList();
            return results;
        }

        public List<CoinSnapshotData> GetLatestSnapshots(out int totalMiningCount, out int totalOnlineCount) {
            InitOnece();
            var count = _clientSet.ClientCount;
            totalMiningCount = count.MiningCount;
            totalOnlineCount = count.OnlineCount;
            Dictionary<string, CoinSnapshotData> dicByCoinCode = new Dictionary<string, CoinSnapshotData>(StringComparer.OrdinalIgnoreCase);
            DateTime now = DateTime.Now;
            foreach (var item in _dataList) {
                if (_isPull) {
                    if (item.Timestamp.AddSeconds(15) < now) {
                        continue;
                    }
                }
                else {
                    if (item.Timestamp.AddSeconds(130) < now) {
                        continue;
                    }
                }
                if (!dicByCoinCode.TryGetValue(item.CoinCode, out CoinSnapshotData data)) {
                    data = item;
                    dicByCoinCode.Add(item.CoinCode, data);
                }
                if (item.Timestamp > data.Timestamp) {
                    dicByCoinCode[item.CoinCode] = item;
                }
            }
            return dicByCoinCode.Values.ToList();
        }
    }
}

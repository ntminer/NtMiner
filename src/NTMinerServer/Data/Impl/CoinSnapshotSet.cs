using LiteDB;
using NTMiner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Data.Impl {
    public class CoinSnapshotSet : ICoinSnapshotSet {
        private readonly IHostRoot _root;
        // 内存中保留最近20分钟的快照
        private readonly List<CoinSnapshotData> _dataList = new List<CoinSnapshotData>();

        private bool _historyDataSnapshotOver = false;
        internal CoinSnapshotSet(IHostRoot root) {
            _root = root;
            Global.Access<Per10SecondEvent>(Guid.Parse(
                "ab0526fc-01ce-4b8a-942d-d998d0a71a3b"),
                "周期性拍摄快照",
                LogEnum.Console,
                action: message => {
                    DateTime leftTime = message.Timestamp.AddSeconds(-message.Seconds);
                    Snapshot(leftTime, seconds: 10);
                    if (_historyDataSnapshotOver) {
                        DateTime rightTime = leftTime.AddSeconds(10);
                        SnapshotTimestamp.SetSnapshotTimestamp(rightTime);
                    }
                });

            Task.Factory.StartNew(() => {
                DateTime leftTime = SnapshotTimestamp.GetSnapshotTimestamp();
                while (leftTime > DateTime.Now.AddMonths(-1) && leftTime.AddSeconds(10) < DateTime.Now) {
                    Snapshot(leftTime, seconds: 10);
                    DateTime rightTime = leftTime.AddSeconds(10);
                    SnapshotTimestamp.SetSnapshotTimestamp(rightTime);
                    leftTime = rightTime;
                }
                _historyDataSnapshotOver = true;
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
                    Global.WriteDevLine("将最近20分钟的快照载入内存");
                    _isInited = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seconds">快照周期：秒</param>
        private void Snapshot(DateTime leftTime, int seconds) {
            if (seconds > 120) {
                throw new InvalidProgramException("客户端每两分钟上报一次数据，所以快照拍摄周期不能大于2分钟，否则一个快照中同一个人可能出现两次");
            }
            if (leftTime == DateTime.MinValue) {
                Global.WriteDevLine("尚没有数据源，暂不拍摄");
                return;
            }
            DateTime rightTime = leftTime.AddSeconds(seconds);
            Global.WriteDevLine($"快照时间区间{{{leftTime} - {rightTime}]");
            if (rightTime <= DateTime.Now) {
                try {
                    if (IsSnapshoted(leftTime, rightTime)) {
                        return;
                    }
                    // 根据3分钟的数据进行统计因为客户端每两分钟上报一次算力
                    List<ClientCoinSnapshotData> clientCoinSpeedList = _root.ClientCoinSnapshotSet.GetClientCoinSnapshots(leftTime.AddSeconds(-180), rightTime);
                    if (clientCoinSpeedList.Count > 0) {
                        Snapshot(clientCoinSpeedList, rightTime);
                    }
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                }
            }
            else {
                Global.WriteDevLine("快照已拍摄到当前时间");
            }
        }

        private void Snapshot(List<ClientCoinSnapshotData> clientCoinSpeedList, DateTime timestamp) {
            InitOnece();
            Dictionary<string, List<ClientCoinSnapshotData>> clientCoinSpeedDic = clientCoinSpeedList.GroupBy(a => a.CoinCode).ToDictionary(a => a.Key, a => a.ToList());
            List<CoinSnapshotData> data = new List<CoinSnapshotData>();
            lock (_locker) {
                foreach (var item in clientCoinSpeedDic) {
                    Dictionary<Guid, ClientCoinSnapshotData> dic = new Dictionary<Guid, ClientCoinSnapshotData>();
                    foreach (var clientCoinSnapshotData in item.Value) {
                        if (!dic.ContainsKey(clientCoinSnapshotData.ClientId)) {
                            dic.Add(clientCoinSnapshotData.ClientId, clientCoinSnapshotData);
                        }
                        else {
                            dic[clientCoinSnapshotData.ClientId] = clientCoinSnapshotData;
                        }
                    }
                    long speed = dic.Values.Sum(a => a.Speed);
                    int shareDelta = dic.Values.Sum(a => a.ShareDelta);
                    CoinSnapshotData snapshotdData = new CoinSnapshotData {
                        Id = ObjectId.NewObjectId(),
                        CoinCode = item.Key,
                        MainCoinMiningCount = _root.ClientSet.CountMainCoinMining(item.Key),
                        MainCoinOnlineCount = _root.ClientSet.CountMainCoinOnline(item.Key),
                        DualCoinMiningCount = _root.ClientSet.CountDualCoinMining(item.Key),
                        DualCoinOnlineCount = _root.ClientSet.CountDualCoinOnline(item.Key),
                        Timestamp = timestamp,
                        ShareDelta = shareDelta,
                        Speed = speed
                    };
                    data.Add(snapshotdData);
                }
                _dataList.AddRange(data);
                DateTime time = timestamp.AddMinutes(-20);
                List<CoinSnapshotData> toRemoves = _dataList.Where(a => a.Timestamp < time).ToList();
                foreach (var item in toRemoves) {
                    _dataList.Remove(item);
                }
            }
            if (data.Count > 0) {
                using (LiteDatabase db = HostRoot.CreateReportDb()) {
                    var col = db.GetCollection<CoinSnapshotData>();
                    col.Insert(data);
                }
                Global.WriteDevLine("拍摄快照" + data.Count + "张，快照时间戳：" + timestamp.ToString("yyyy-MM-dd HH:mm:ss fff"));
            }
        }

        private bool IsSnapshoted(DateTime leftTime, DateTime rightTime) {
            InitOnece();
            if (leftTime > DateTime.Now.AddMinutes(-20)) {
                lock (_locker) {
                    return _dataList.Any(a => a.Timestamp > leftTime && a.Timestamp <= rightTime);
                }
            }
            using (LiteDatabase db = HostRoot.CreateReportDb()) {
                var col = db.GetCollection<CoinSnapshotData>();
                return col.Exists(
                    Query.And(
                        Query.GT(nameof(CoinSnapshotData.Timestamp), leftTime),
                        Query.LTE(nameof(CoinSnapshotData.Timestamp), rightTime)));
            }
        }

        public List<CoinSnapshotData> GetLatestSnapshots(
            int limit,
            List<string> coinCodes,
            out int totalMiningCount,
            out int totalOnlineCount) {
            InitOnece();
            totalMiningCount = HostRoot.Current.ClientSet.MiningCount;
            totalOnlineCount = HostRoot.Current.ClientSet.OnlineCount;
            List<CoinSnapshotData> results = new List<CoinSnapshotData>();
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
            List<CoinSnapshotData> list = new List<CoinSnapshotData>(limit * coinCodes.Count);
            for (int i = 1; i <= limit; i++) {
                DateTime time = rightTime.AddSeconds(-10 * i - 10);
                DateTime time2 = rightTime.AddSeconds(-10 * i + 10);
                foreach (var coinCode in coinCodes) {
                    var dataItem = results.FirstOrDefault(a => a.Timestamp > time && a.Timestamp <= time2 && a.CoinCode == coinCode);
                    if (dataItem != null) {
                        list.Add(dataItem);
                    }
                    else {
                        list.Add(new CoinSnapshotData {
                            Id = ObjectId.NewObjectId(),
                            CoinCode = coinCode,
                            MainCoinMiningCount = 0,
                            MainCoinOnlineCount = 0,
                            DualCoinMiningCount = 0,
                            DualCoinOnlineCount = 0,
                            ShareDelta = 0,
                            Speed = 0,
                            Timestamp = time2
                        });
                    }
                }
            }
            return list;
        }
    }
}

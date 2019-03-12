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

        private bool _historyDataSnapshotOver = false;
        internal CoinSnapshotSet(IHostRoot root) {
            _root = root;
            VirtualRoot.On<Per10SecondEvent>(
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
                    Write.DevLine("将最近20分钟的快照载入内存");
                    _isInited = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftTime"></param>
        /// <param name="seconds">快照周期：秒</param>
        private void Snapshot(DateTime leftTime, int seconds) {
            if (seconds > 120) {
                throw new InvalidProgramException("客户端每两分钟上报一次数据，所以快照拍摄周期不能大于2分钟，否则一个快照中同一个人可能出现两次");
            }
            if (leftTime == DateTime.MinValue) {
                Write.DevLine("尚没有数据源，暂不拍摄");
                return;
            }
            DateTime rightTime = leftTime.AddSeconds(seconds);
            Write.DevLine($"快照时间区间{{{leftTime} - {rightTime}]");
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
                    Logger.ErrorDebugLine(e.Message, e);
                }
            }
            else {
                Write.DevLine("快照已拍摄到当前时间");
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
                    double speed = dic.Values.Sum(a => a.Speed);
                    int shareDelta = dic.Values.Sum(a => a.ShareDelta);
                    int rejectShareDelta = dic.Values.Sum(a => a.RejectShareDelta);
                    ClientCoinCount count = _root.ClientSet.Count(item.Key);
                    CoinSnapshotData snapshotData = new CoinSnapshotData {
                        Id = ObjectId.NewObjectId(),
                        CoinCode = item.Key,
                        MainCoinMiningCount = count.MainCoinMiningCount,
                        MainCoinOnlineCount = count.MainCoinOnlineCount,
                        DualCoinMiningCount = count.DualCoinMiningCount,
                        DualCoinOnlineCount = count.DualCoinOnlineCount,
                        Timestamp = timestamp,
                        ShareDelta = shareDelta,
                        RejectShareDelta = rejectShareDelta,
                        Speed = speed
                    };
                    data.Add(snapshotData);
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
                Write.DevLine("拍摄快照" + data.Count + "张，快照时间戳：" + timestamp.ToString("yyyy-MM-dd HH:mm:ss fff"));
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

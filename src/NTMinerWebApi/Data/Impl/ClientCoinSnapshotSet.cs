using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Data.Impl {
    public class ClientCoinSnapshotSet : IClientCoinSnapshotSet {
        // 内存中保留最近4分钟的数据
        private readonly List<ClientCoinSnapshotData> _dataList = new List<ClientCoinSnapshotData>();

        private readonly IHostRoot _root;
        internal ClientCoinSnapshotSet(IHostRoot root) {
            _root = root;
            Global.Access<Per10SecondEvent>(
                Guid.Parse("e093f476-e79d-45ba-b527-95ca71c3b737"),
                "周期性将内存中的CientCoinSpeedData列表刷到磁盘",
                LogEnum.Console,
                action: message => {
                    InitOnece();
                    if (message.Seconds > 4 * 60) {
                        throw new InvalidProgramException("内存中保留4分钟的数据，所以刷新周期不能大于4分钟");
                    }
                    ClientCoinSnapshotData[] values = null;
                    using (LiteDatabase db = HostRoot.CreateReportDb()) {
                        var col = db.GetCollection<ClientCoinSnapshotData>();
                        lock (_locker) {
                            // 将最近一个周期生成的数据刷入磁盘
                            DateTime time = message.Timestamp.AddSeconds(-message.Seconds);
                            values = _dataList.Where(a => a.Timestamp > time).ToArray();
                            // 将4分钟之前的数据从内存中清除
                            time = message.Timestamp.AddMinutes(-4);
                            List<ClientCoinSnapshotData> toRemoves = _dataList.Where(a => a.Timestamp < time).ToList();
                            foreach (var item in toRemoves) {
                                _dataList.Remove(item);
                            }
                        }
                        if (values.Length != 0) {
                            col.Upsert(values);
                        }
                        Global.WriteDevLine("刷了" + values.Length + "条");
                    }
                });
            Global.Access<Per2MinuteEvent>(
                Guid.Parse("033f4391-6b37-4232-abf4-c12117b4716b"),
                "周期性清除已经拍过快照的CientCoinSpeedData源数据",
                LogEnum.Console,
                action: message => {
                    InitOnece();
                    DateTime timestamp = SnapshotTimestamp.GetSnapshotTimestamp();
                    if (timestamp == DateTime.MinValue) {
                        Global.WriteDevLine("尚没有拍摄过快照，无需清除");
                        return;
                    }
                    using (LiteDatabase db = HostRoot.CreateReportDb()) {
                        var col = db.GetCollection<ClientCoinSnapshotData>();
                        int r = col.Delete(Query.LT(nameof(ClientCoinSnapshotData.Timestamp), timestamp));
                        if (r > 0) {
                            db.Shrink();
                        }
                    }
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
                    using (LiteDatabase db = HostRoot.CreateReportDb()) {
                        var col = db.GetCollection<ClientCoinSnapshotData>();
                        // 将最近4分钟的数据载入内存
                        DateTime now = DateTime.Now;
                        foreach (var item in col.Find(
                            Query.And(
                                Query.GT(nameof(ClientCoinSnapshotData.Timestamp), now.AddMinutes(-4)),
                                Query.LTE(nameof(ClientCoinSnapshotData.Timestamp), now)))) {
                            _dataList.Add(item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public DateTime GetMinTimestamp() {
            InitOnece();
            DateTime t = DateTime.MinValue;
            using (LiteDatabase db = HostRoot.CreateReportDb()) {
                int minId = db.GetCollection<ClientCoinSnapshotData>().Min(a => a.Id).AsInt32;
                if (minId != 0) {
                    var clientCoinSpeedData = db.GetCollection<ClientCoinSnapshotData>().FindById(minId);
                    if (clientCoinSpeedData != null) {
                        t = clientCoinSpeedData.Timestamp;
                    }
                }
            }
            if (t == DateTime.MinValue) {
                lock (_locker) {
                    if (_dataList.Count > 0) {
                        return _dataList.Min(a => a.Timestamp);
                    }
                }
            }
            return t;
        }

        public void Add(ClientCoinSnapshotData clientCoinSnapshotData) {
            InitOnece();
            lock (_locker) {
                _dataList.Add(clientCoinSnapshotData);
            }
        }

        public List<ClientCoinSnapshotData> GetClientCoinSnapshots(DateTime leftTime, DateTime rightTime) {
            InitOnece();
            if (leftTime > DateTime.Now.AddMinutes(-4)) {
                lock (_locker) {
                    return _dataList.Where(a => a.Timestamp > leftTime && a.Timestamp <= rightTime).ToList();
                }
            }
            else {
                using (LiteDatabase db = HostRoot.CreateReportDb()) {
                    var col = db.GetCollection<ClientCoinSnapshotData>();
                    return col.Find(
                        Query.And(
                            Query.GT(nameof(ClientCoinSnapshotData.Timestamp), leftTime),
                            Query.LTE(nameof(ClientCoinSnapshotData.Timestamp), rightTime))).ToList();
                }
            }
        }
    }
}

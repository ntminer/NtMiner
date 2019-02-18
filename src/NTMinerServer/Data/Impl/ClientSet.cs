using LiteDB;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTMiner.Data.Impl {
    public class ClientSet : IClientSet {
        // 内存中保留20分钟内活跃的客户端
        private readonly Dictionary<Guid, ClientData> _dicById = new Dictionary<Guid, ClientData>();

        private readonly IHostRoot _root;
        internal ClientSet(IHostRoot root) {
            _root = root;
            VirtualRoot.Access<Per20SecondEvent>(
                Guid.Parse("ea795e07-7f4b-4284-aa72-aa00c17c89d8"),
                "周期性将内存中的ClientData列表刷入磁盘",
                LogEnum.Console,
                action: message => {
                    InitOnece();
                    lock (_locker) {
                        DateTime time = message.Timestamp.AddMinutes(-20);
                        // 移除20分钟内未活跃的客户端缓存
                        List<Guid> toRemoves = _dicById.Where(a => a.Value.ModifiedOn < time).Select(a => a.Key).ToList();
                        foreach (var clientId in toRemoves) {
                            _dicById.Remove(clientId);
                        }
                        time = message.Timestamp.AddSeconds(-message.Seconds);
                        using (LiteDatabase db = HostRoot.CreateReportDb()) {
                            var col = db.GetCollection<ClientData>();
                            // 更新一个周期内活跃的客户端
                            col.Upsert(_dicById.Values.Where(a => a.ModifiedOn > time));
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
                        var col = db.GetCollection<ClientData>();
                        DateTime time = DateTime.Now.AddMinutes(-20);
                        foreach (var item in col.Find(Query.GT(nameof(ClientData.ModifiedOn), time))) {
                            _dicById.Add(item.Id, item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public ClientCount Count() {
            InitOnece();
            // 因为客户端每120秒上报一次数据所以将140秒内活跃的客户端视为在线
            DateTime time = DateTime.Now.AddSeconds(-140);
            int onlineCount = 0;
            int miningCount = 0;
            lock (_locker) {
                foreach (var clientData in _dicById.Values) {
                    if (clientData.ModifiedOn > time) {
                        onlineCount++;
                        if (clientData.IsMining) {
                            miningCount++;
                        }
                    }
                }
            }
            return new ClientCount {
                OnlineCount = onlineCount,
                MiningCount = miningCount
            };
        }

        public ClientCoinCount Count(string coinCode) {
            InitOnece();
            DateTime time = DateTime.Now.AddSeconds(-140);
            int mainCoinOnlineCount = 0;
            int mainCoinMiningCount = 0;
            int dualCoinOnlineCount = 0;
            int dualCoinMiningCount = 0;
            lock (_locker) {
                foreach (var clientData in _dicById.Values) {
                    if (clientData.ModifiedOn > time) {
                        if (clientData.MainCoinCode == coinCode) {
                            mainCoinOnlineCount++;
                            if (clientData.IsMining) {
                                mainCoinMiningCount++;
                            }
                        }
                        if (clientData.DualCoinCode == coinCode) {
                            dualCoinOnlineCount++;
                            if (clientData.IsMining) {
                                dualCoinMiningCount++;
                            }
                        }
                    }
                }
            }
            return new ClientCoinCount {
                MainCoinOnlineCount = mainCoinOnlineCount,
                MainCoinMiningCount = mainCoinMiningCount,
                DualCoinOnlineCount = dualCoinOnlineCount,
                DualCoinMiningCount = dualCoinMiningCount
            };
        }

        public void Add(ClientData clientData) {
            InitOnece();
            if (!_dicById.ContainsKey(clientData.Id)) {
                lock (_locker) {
                    if (!_dicById.ContainsKey(clientData.Id)) {
                        _dicById.Add(clientData.Id, clientData);
                    }
                }
            }
        }

        public List<ClientData> QueryClients(
            int pageIndex,
            int pageSize,
            Guid? groupId,
            Guid? workId,
            string minerIp,
            string minerName,
            MineStatus mineState,
            string mainCoin,
            string mainCoinPool,
            string mainCoinWallet,
            string dualCoin,
            string dualCoinPool,
            string dualCoinWallet,
            string version,
            string kernel,
            out int total) {
            InitOnece();
            lock (_locker) {
                IQueryable<ClientData> query = _dicById.Values.AsQueryable();
                if (groupId != null && groupId.Value != Guid.Empty) {
                    query = query.Where(a => a.GroupId == groupId.Value);
                }
                if (workId != null && workId.Value != Guid.Empty) {
                    query = query.Where(a => a.WorkId == workId.Value);
                }
                else {
                    if (workId != null) {
                        query = query.Where(a => a.WorkId == workId.Value);
                    }
                    if (!string.IsNullOrEmpty(mainCoin)) {
                        query = query.Where(a => a.MainCoinCode == mainCoin);
                    }
                    if (!string.IsNullOrEmpty(mainCoinPool)) {
                        query = query.Where(a => a.MainCoinPool == mainCoinPool);
                    }
                    if (!string.IsNullOrEmpty(dualCoin)) {
                        if (dualCoin == "*") {
                            query = query.Where(a => a.IsDualCoinEnabled);
                        }
                        else {
                            query = query.Where(a => a.DualCoinCode == dualCoin);
                        }
                    }
                    if (!string.IsNullOrEmpty(dualCoinPool)) {
                        query = query.Where(a => a.DualCoinPool == dualCoinPool);
                    }
                    if (!string.IsNullOrEmpty(mainCoinWallet)) {
                        query = query.Where(a => a.MainCoinWallet == mainCoinWallet);
                    }
                    if (!string.IsNullOrEmpty(dualCoinWallet)) {
                        query = query.Where(a => a.DualCoinWallet == dualCoinWallet);
                    }
                }
                if (!string.IsNullOrEmpty(minerIp)) {
                    query = query.Where(a => a.MinerIp == minerIp);
                }
                if (!string.IsNullOrEmpty(minerName)) {
                    query = query.Where(a => a.MinerName.IndexOf(minerName, StringComparison.OrdinalIgnoreCase) >= 0);
                }
                if (mineState != MineStatus.All) {
                    if (mineState == MineStatus.Mining) {
                        query = query.Where(a => a.IsMining == true);
                    }
                    else {
                        query = query.Where(a => a.IsMining == false);
                    }
                }
                if (!string.IsNullOrEmpty(version)) {
                    query = query.Where(a => a.Version != null && a.Version.StartsWith(version, StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(kernel)) {
                    query = query.Where(a => a.Kernel != null && a.Kernel.StartsWith(kernel, StringComparison.OrdinalIgnoreCase));
                }
                total = query.Count();
                return query.OrderBy(a => a.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        public List<ClientData> LoadClients(IEnumerable<Guid> clientIds) {
            InitOnece();
            List<ClientData> results = new List<ClientData>();
            foreach (var clientId in clientIds) {
                ClientData clientData = LoadClient(clientId);
                if (clientData != null) {
                    results.Add(clientData);
                }
            }
            return results;
        }

        public ClientData LoadClient(Guid clientId) {
            InitOnece();
            ClientData clientData = null;
            lock (_locker) {
                if (_dicById.TryGetValue(clientId, out clientData)) {
                    return clientData;
                }
            }
            using (LiteDatabase db = HostRoot.CreateReportDb()) {
                var col = db.GetCollection<ClientData>();
                clientData = col.FindById(clientId);
                if (clientData != null) {
                    Add(clientData);
                }
                return clientData;
            }
        }

        public void UpdateClient(Guid clientId, string propertyName, object value) {
            InitOnece();
            ClientData clientData = LoadClient(clientId);
            if (clientData != null) {
                PropertyInfo propertyInfo = typeof(ClientData).GetProperty(propertyName);
                if (propertyInfo != null) {
                    if (propertyInfo.PropertyType == typeof(Guid)) {
                        value = DictionaryExtensions.ConvertToGuid(value);
                    }
                    propertyInfo.SetValue(clientData, value, null);
                    clientData.ModifiedOn = DateTime.Now;
                }
            }
        }

        public void UpdateClientProperties(Guid clientId, Dictionary<string, object> values) {
            InitOnece();
            ClientData clientData = LoadClient(clientId);
            if (clientData != null) {
                foreach (var kv in values) {
                    object value = kv.Value;
                    PropertyInfo propertyInfo = typeof(ClientData).GetProperty(kv.Key);
                    if (propertyInfo != null) {
                        if (propertyInfo.PropertyType == typeof(Guid)) {
                            value = DictionaryExtensions.ConvertToGuid(value);
                        }
                        propertyInfo.SetValue(clientData, value, null);
                    }
                }
                clientData.ModifiedOn = DateTime.Now;
            }
        }
    }
}

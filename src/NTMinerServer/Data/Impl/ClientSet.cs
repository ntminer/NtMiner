using LiteDB;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;

namespace NTMiner.Data.Impl {
    public class ClientSet : IClientSet {
        // 内存中保留20分钟内活跃的客户端
        private readonly Dictionary<Guid, ClientData> _dicById = new Dictionary<Guid, ClientData>();

        private readonly IHostRoot _root;
        internal ClientSet(IHostRoot root) {
            _root = root;
            VirtualRoot.On<Per20SecondEvent>(
                "周期性将内存中的ClientData列表刷入磁盘",
                LogEnum.Console,
                action: message => {
                    InitOnece();
                    lock (_locker) {
                        DateTime time = message.Timestamp.AddSeconds(-message.Seconds);
                        using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                            var col = db.GetCollection<MinerClient>();
                            // 更新一个周期内活跃的客户端
                            col.Upsert(_dicById.Values.Where(a => a.ModifiedOn > time).Select(a => new MinerClient {
                                CreatedOn = a.CreatedOn,
                                GroupId = a.GroupId,
                                Id = a.Id,
                                MinerIp = a.MinerIp,
                                WindowsLoginName = a.WindowsLoginName,
                                WindowsPassword = a.WindowsPassword,
                                WorkId = a.WorkId
                            }));
                        }
                    }
                });
            VirtualRoot.On<Per10SecondEvent>(
                "周期拉取数据更新拍照源数据",
                LogEnum.Console,
                action: message => {
                    if (HostRoot.IsPull) {
                        ClientData[] clientDatas = _dicById.Values.ToArray();
                        Task[] tasks = clientDatas.Select(a => CreatePullTask(a)).ToArray();
                        Task.WaitAll(tasks, 10 * 1000);
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
                    using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                        var col = db.GetCollection<MinerClient>();
                        foreach (var item in col.FindAll()) {
                            _dicById.Add(item.Id, CreateClientData(item));
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public static ClientData CreateClientData(MinerClient data) {
            return new ClientData() {
                MinerIp = data.MinerIp,
                CreatedOn = data.CreatedOn,
                GroupId = data.GroupId,
                WorkId = data.WorkId,
                WindowsLoginName = data.WindowsLoginName,
                WindowsPassword = data.WindowsPassword,
                Id = data.Id,
                IsAutoBoot = false,
                IsAutoStart = false,
                IsAutoRestartKernel = false,
                IsNoShareRestartKernel = false,
                NoShareRestartKernelMinutes = 0,
                IsPeriodicRestartKernel = false,
                PeriodicRestartKernelHours = 0,
                IsPeriodicRestartComputer = false,
                PeriodicRestartComputerHours = 0,
                GpuDriver = String.Empty,
                GpuType = GpuType.Empty,
                OSName = String.Empty,
                OSVirtualMemoryMb = 0,
                GpuInfo = String.Empty,
                Version = String.Empty,
                IsMining = false,
                BootOn = DateTime.MinValue,
                MineStartedOn = DateTime.MinValue,
                MinerName = String.Empty,
                ModifiedOn = DateTime.MinValue,
                MainCoinCode = String.Empty,
                MainCoinTotalShare = 0,
                MainCoinRejectShare = 0,
                MainCoinSpeed = 0,
                MainCoinPool = String.Empty,
                MainCoinWallet = String.Empty,
                Kernel = String.Empty,
                IsDualCoinEnabled = false,
                DualCoinPool = String.Empty,
                DualCoinWallet = String.Empty,
                DualCoinCode = String.Empty,
                DualCoinTotalShare = 0,
                DualCoinRejectShare = 0,
                DualCoinSpeed = 0,
                KernelCommandLine = String.Empty,
                GpuTable = new GpuSpeedData[0]
            };
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
            out int total,
            out int miningCount) {
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
                miningCount = query.Count(a => a.IsMining);
                var results = query.OrderBy(a => a.MinerName).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                DateTime time = DateTime.Now.AddMinutes(-3);
                // 3分钟未上报算力视为0算力
                foreach (var clientData in results) {
                    if (clientData.ModifiedOn < time) {
                        clientData.DualCoinSpeed = 0;
                        clientData.MainCoinSpeed = 0;
                    }
                }
                return results;
            }
        }

        public ClientData LoadClient(Guid clientId) {
            InitOnece();
            ClientData clientData = null;
            _dicById.TryGetValue(clientId, out clientData);
            return clientData;
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

        public static Task<SpeedData> CreatePullTask(ClientData clientData) {
            return Client.MinerClientService.GetSpeedAsync(clientData.MinerIp, (speedData, exception) => {
                if (exception != null) {
                    Exception innerException = exception.GetInnerException();
                    if (innerException is SocketException || innerException is TaskCanceledException) {
                        clientData.IsMining = false;
                        clientData.MainCoinSpeed = 0;
                        clientData.DualCoinSpeed = 0;
                        foreach (var item in clientData.GpuTable) {
                            item.MainCoinSpeed = 0;
                            item.DualCoinSpeed = 0;
                        }
                    }
                }
                else {
                    clientData.Update(speedData);
                }
            });
        }

        public IEnumerator<ClientData> GetEnumerator() {
            InitOnece();
            foreach (var clientData in _dicById.Values) {
                yield return clientData;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}

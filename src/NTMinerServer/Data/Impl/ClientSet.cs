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
        private readonly Dictionary<ObjectId, ClientData> _dicByObjectId = new Dictionary<ObjectId, ClientData>();
        private readonly Dictionary<Guid, ClientData> _dicByClientId = new Dictionary<Guid, ClientData>();

        private DateTime _getSpeedOn = DateTime.Now;
        private readonly IHostRoot _root;
        internal ClientSet(IHostRoot root) {
            _root = root;
            GetSpeed();
            VirtualRoot.On<Per20SecondEvent>(
                "周期性将内存中的ClientData列表刷入磁盘",
                LogEnum.Console,
                action: message => {
                    InitOnece();
                    lock (_locker) {
                        DateTime time = message.Timestamp.AddMinutes(-5);
                        using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                            var col = db.GetCollection<MinerData>();
                            col.Upsert(_dicByObjectId.Values.Where(a => a.ModifiedOn > time).Select(a => new MinerData {
                                CreatedOn = a.CreatedOn,
                                GroupId = a.GroupId,
                                Id = new ObjectId(a.Id),
                                ClientId = a.ClientId,
                                MinerIp = a.MinerIp,
                                WindowsLoginName = a.WindowsLoginName,
                                WindowsPassword = a.WindowsPassword,
                                WorkId = a.WorkId
                            }));
                        }
                    }
                });
        }

        private void GetSpeed() {
            Task.Factory.StartNew(() => {
                while (true) {
                    DateTime now = DateTime.Now;
                    if (_getSpeedOn.AddSeconds(10) <= now) {
                        if (HostRoot.Current.HostConfig.IsPull) {
                            Write.DevLine("周期拉取数据更新拍照源数据");
                            Task.Factory.StartNew(() => {
                                ClientData[] clientDatas = _dicByObjectId.Values.ToArray();
                                Task[] tasks = clientDatas.Select(CreatePullTask).ToArray();
                                Task.WaitAll(tasks, 5 * 1000);
                            });
                        }
                        _getSpeedOn = now;
                    }
                    else {
                        System.Threading.Thread.Sleep((int)(_getSpeedOn.AddSeconds(10) - now).TotalMilliseconds);
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
                    using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                        var col = db.GetCollection<MinerData>();
                        foreach (var item in col.FindAll()) {
                            var data = MinerData.CreateClientData(item);
                            _dicByObjectId.Add(item.Id, data);
                            if (!_dicByClientId.ContainsKey(item.ClientId)) {
                                _dicByClientId.Add(item.ClientId, data);
                            }
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public void Add(ClientData clientData) {
            InitOnece();
            ObjectId objectId = new ObjectId(clientData.Id);
            if (!_dicByObjectId.ContainsKey(objectId)) {
                _dicByObjectId.Add(objectId, clientData);
            }

            if (!_dicByClientId.ContainsKey(clientData.ClientId)) {
                _dicByClientId.Add(clientData.ClientId, clientData);
            }
        }

        public void Remove(ObjectId objectId) {
            ClientData clientData;
            if (_dicByObjectId.TryGetValue(objectId, out clientData)) {
                _dicByObjectId.Remove(objectId);
                _dicByClientId.Remove(clientData.ClientId);
                using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                    var col = db.GetCollection<MinerData>();
                    col.Delete(new ObjectId(clientData.Id));
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
            string dualCoin,
            string dualCoinPool,
            string wallet,
            string version,
            string kernel,
            out int total,
            out int miningCount) {
            InitOnece();
            IQueryable<ClientData> query = _dicByObjectId.Values.AsQueryable();
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
                if (!string.IsNullOrEmpty(wallet)) {
                    query = query.Where(a => a.MainCoinWallet == wallet || a.DualCoinWallet == wallet);
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
            var results = query.OrderBy(a => a.MinerIp).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public ClientData GetByClientId(Guid clientId) {
            InitOnece();
            ClientData clientData = null;
            _dicByClientId.TryGetValue(clientId, out clientData);
            return clientData;
        }

        public void UpdateClient(ObjectId objectId, string propertyName, object value) {
            InitOnece();
            ClientData clientData;
            if (_dicByObjectId.TryGetValue(objectId, out clientData)) {
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

        public void UpdateClientProperties(ObjectId objectId, Dictionary<string, object> values) {
            InitOnece();
            ClientData clientData;
            if (_dicByObjectId.TryGetValue(objectId, out clientData)) {
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

        public Task<SpeedData> CreatePullTask(ClientData clientData) {
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
                    if (speedData.ClientId != clientData.ClientId) {
                        _dicByClientId.Remove(clientData.ClientId);
                        _dicByClientId.Add(speedData.ClientId, clientData);
                    }
                    clientData.Update(speedData);
                }
            });
        }

        public IEnumerator<ClientData> GetEnumerator() {
            InitOnece();
            foreach (var clientData in _dicByObjectId.Values) {
                yield return clientData;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}

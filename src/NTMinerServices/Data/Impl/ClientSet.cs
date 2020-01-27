using LiteDB;
using NTMiner.Core;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NTMiner.Data.Impl {
    public class ClientSet : IClientSet {
        private readonly Dictionary<string, ClientData> _dicByObjectId = new Dictionary<string, ClientData>();
        private readonly Dictionary<Guid, ClientData> _dicByClientId = new Dictionary<Guid, ClientData>();

        private DateTime _getSpeedOn = DateTime.Now;
        internal ClientSet() {
            GetSpeed();
            VirtualRoot.AddEventPath<Per20SecondEvent>("周期性将内存中3分钟内活跃的ClientData列表刷入磁盘", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    List<MinerData> minerDatas = new List<MinerData>();
                    lock (_locker) {
                        DateTime time = message.BornOn.AddMinutes(-3);
                        foreach (var clientData in _dicByObjectId.Values) {
                            if (clientData.ModifiedOn > time) {
                                minerDatas.Add(new MinerData {
                                    CreatedOn = clientData.CreatedOn,
                                    GroupId = clientData.GroupId,
                                    Id = clientData.Id,
                                    ClientId = clientData.ClientId,
                                    ClientName = clientData.ClientName,
                                    MinerIp = clientData.MinerIp,
                                    MACAddress = clientData.MACAddress,
                                    MinerName = clientData.MinerName,
                                    WindowsLoginName = clientData.WindowsLoginName,
                                    WindowsPassword = clientData.WindowsPassword,
                                    WorkId = clientData.WorkId
                                });
                            }
                            else {
                                clientData.IsMining = false;
                                clientData.MainCoinSpeed = 0;
                                clientData.DualCoinSpeed = 0;
                                foreach (var item in clientData.GpuTable) {
                                    item.MainCoinSpeed = 0;
                                    item.DualCoinSpeed = 0;
                                }
                            }
                        }
                    }
                    using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                        var col = db.GetCollection<MinerData>();
                        col.Upsert(minerDatas);
                    }
                }, location: this.GetType());
        }

        private void GetSpeed() {
            Task.Factory.StartNew(() => {
                while (true) {
                    DateTime now = DateTime.Now;
                    if (_getSpeedOn.AddSeconds(10) <= now) {
                        if (HostRoot.Instance.HostConfig.IsPull) {
                            Write.DevDebug("周期拉取数据更新拍照源数据");
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
            }, TaskCreationOptions.LongRunning);
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
                            var data = ClientData.CreateClientData(item);
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
            if (clientData == null || clientData.Id == null) {
                return;
            }
            if (!_dicByObjectId.ContainsKey(clientData.Id)) {
                _dicByObjectId.Add(clientData.Id, clientData);
            }

            if (!_dicByClientId.ContainsKey(clientData.ClientId)) {
                _dicByClientId.Add(clientData.ClientId, clientData);
            }
        }

        public void AddMiner(string minerIp) {
            MinerData minerData = new MinerData {
                Id = ObjectId.NewObjectId().ToString(),
                ClientId = Guid.NewGuid(),
                ClientName = string.Empty,
                CreatedOn = DateTime.Now,
                GroupId = Guid.Empty,
                MinerIp = minerIp,
                MACAddress = string.Empty,
                WindowsLoginName = string.Empty,
                WindowsPassword = String.Empty,
                WorkId = Guid.Empty
            };
            var clientData = ClientData.CreateClientData(minerData);
            Add(clientData);
            using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                var col = db.GetCollection<MinerData>();
                col.Insert(minerData);
            }
        }

        public void Remove(string objectId) {
            if (objectId == null) {
                return;
            }
            if (_dicByObjectId.TryGetValue(objectId, out ClientData clientData)) {
                _dicByObjectId.Remove(objectId);
                _dicByClientId.Remove(clientData.ClientId);
                using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                    var col = db.GetCollection<MinerData>();
                    col.Delete(clientData.Id);
                }
            }
        }

        public List<ClientData> RefreshClients(List<string> objectIds) {
            List<ClientData> clientDatas = new List<ClientData>();
            foreach (var objectId in objectIds) {
                if (_dicByObjectId.TryGetValue(objectId, out ClientData item)) {
                    clientDatas.Add(item);
                }
            }
            Task[] tasks = clientDatas.Select(CreatePullTask).ToArray();
            Task.WaitAll(tasks, 5 * 1000);
            return clientDatas;
        }

        public List<ClientData> QueryClients(
            int pageIndex,
            int pageSize,
            Guid? groupId,
            Guid? workId,
            string minerIp,
            string minerName,
            MineStatus mineState,
            string coin,
            string pool,
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
                if (!string.IsNullOrEmpty(coin)) {
                    query = query.Where(a => a.MainCoinCode == coin || a.DualCoinCode == coin);
                }
                if (!string.IsNullOrEmpty(pool)) {
                    query = query.Where(a => a.MainCoinPool == pool || a.DualCoinPool == pool);
                }
                if (!string.IsNullOrEmpty(wallet)) {
                    query = query.Where(a => a.MainCoinWallet == wallet || a.DualCoinWallet == wallet);
                }
            }
            if (!string.IsNullOrEmpty(minerIp)) {
                query = query.Where(a => a.MinerIp == minerIp);
            }
            if (!string.IsNullOrEmpty(minerName)) {
                query = query.Where(a =>
                (!string.IsNullOrEmpty(a.MinerName) && a.MinerName.IndexOf(minerName, StringComparison.OrdinalIgnoreCase) != -1)
                || (!string.IsNullOrEmpty(a.ClientName) && a.ClientName.IndexOf(minerName, StringComparison.OrdinalIgnoreCase) != -1));
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
            var results = query.OrderBy(a => a.ClientName).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
            _dicByClientId.TryGetValue(clientId, out ClientData clientData);
            return clientData;
        }

        public ClientData GetByObjectId(string objectId) {
            InitOnece();
            if (objectId == null) {
                return null;
            }
            _dicByObjectId.TryGetValue(objectId, out ClientData clientData);
            return clientData;
        }

        public void UpdateClient(string objectId, string propertyName, object value) {
            InitOnece();
            if (objectId == null) {
                return;
            }
            if (_dicByObjectId.TryGetValue(objectId, out ClientData clientData)) {
                PropertyInfo propertyInfo = typeof(ClientData).GetProperty(propertyName);
                if (propertyInfo != null) {
                    if (propertyInfo.PropertyType == typeof(Guid)) {
                        value = VirtualRoot.ConvertToGuid(value);
                    }
                    propertyInfo.SetValue(clientData, value, null);
                    clientData.ModifiedOn = DateTime.Now;
                }
            }
        }

        public void UpdateClients(string propertyName, Dictionary<string, object> values) {
            InitOnece();
            PropertyInfo propertyInfo = typeof(ClientData).GetProperty(propertyName);
            if (propertyInfo != null) {
                if (propertyInfo.PropertyType == typeof(Guid)) {
                    foreach (var kv in values) {
                        values[kv.Key] = VirtualRoot.ConvertToGuid(kv.Value);
                    }
                }
                foreach (var kv in values) {
                    string objectId = kv.Key;
                    object value = kv.Value;
                    if (_dicByObjectId.TryGetValue(objectId, out ClientData clientData)) {
                        propertyInfo.SetValue(clientData, value, null);
                        clientData.ModifiedOn = DateTime.Now;
                    }
                }
            }
        }

        public Task CreatePullTask(ClientData clientData) {
            return RpcRoot.Client.MinerClientService.GetSpeedAsync(clientData.MinerIp, (speedData, exception) => {
                if (exception != null) {
                    clientData.IsMining = false;
                    clientData.MainCoinSpeed = 0;
                    clientData.DualCoinSpeed = 0;
                    foreach (var item in clientData.GpuTable) {
                        item.MainCoinSpeed = 0;
                        item.DualCoinSpeed = 0;
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

        public bool IsAnyClientInGroup(Guid groupId) {
            InitOnece();
            return _dicByObjectId.Values.Any(a => a.GroupId == groupId);
        }

        public bool IsAnyClientInWork(Guid workId) {
            InitOnece();
            return _dicByObjectId.Values.Any(a => a.WorkId == workId);
        }

        public IEnumerable<ClientData> AsEnumerable() {
            InitOnece();
            return _dicByObjectId.Values;
        }
    }
}

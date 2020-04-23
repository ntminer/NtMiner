using NTMiner.Core.MinerServer;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTMiner.Core.Impl {
    public abstract class ClientDataSetBase {
        protected readonly Dictionary<string, ClientData> _dicByObjectId = new Dictionary<string, ClientData>();
        protected readonly Dictionary<Guid, ClientData> _dicByClientId = new Dictionary<Guid, ClientData>();

        protected DateTime InitedOn = DateTime.MinValue;
        public bool IsReadied {
            get; private set;
        }

        protected abstract void DoUpdateSave(MinerData minerData);
        protected abstract void DoUpdateSave(IEnumerable<MinerData> minerDatas);
        protected abstract void DoRemoveSave(MinerData minerData);
        protected abstract void DoCheckIsOnline(IEnumerable<ClientData> clientDatas);

        private readonly bool _isPull;
        /// <summary>
        /// 这里挺绕，逻辑是由父类向子类传入一个接收一个矿工列表的方法，然后由子类调用该方法时传入矿工列表，从而父类收到了来自子类的矿工列表。
        /// </summary>
        /// <param name="isPull"></param>
        /// <param name="getDatas"></param>
        public ClientDataSetBase(bool isPull, Action<Action<IEnumerable<MinerData>>> getDatas) {
            _isPull = isPull;
            getDatas(datas => {
                InitedOn = DateTime.Now;
                IsReadied = true;
                foreach (var item in datas) {
                    var data = ClientData.Create(item);
                    if (!_dicByObjectId.ContainsKey(item.Id)) {
                        _dicByObjectId.Add(item.Id, data);
                    }
                    if (!_dicByClientId.ContainsKey(item.ClientId)) {
                        _dicByClientId.Add(item.ClientId, data);
                    }
                }
                Write.UserOk("矿机集就绪");
                VirtualRoot.RaiseEvent(new ClientSetInitedEvent());
            });
        }

        public ClientCount ClientCount { get; private set; } = new ClientCount();

        public List<ClientData> QueryClients(
            IUser user,
            QueryClientsRequest query,
            out int total,
            out List<CoinSnapshotData> coinSnapshots,
            out int onlineCount,
            out int miningCount) {

            coinSnapshots = new List<CoinSnapshotData>();
            onlineCount = 0;
            miningCount = 0;
            if (!IsReadied) {
                total = 0;
                return new List<ClientData>();
            }
            List<ClientData> list = new List<ClientData>();
            var data = _dicByObjectId.Values.ToArray();
            bool isFilterByUser = user != null && !string.IsNullOrEmpty(user.LoginName) && !user.IsAdmin();
            bool isFilterByGroup = query.GroupId.HasValue && query.GroupId.Value != Guid.Empty;
            bool isFilterByMinerIp = !string.IsNullOrEmpty(query.MinerIp);
            bool isFilterByMinerName = !string.IsNullOrEmpty(query.MinerName);
            bool isFilterByVersion = !string.IsNullOrEmpty(query.Version);
            bool isFilterByWork = query.WorkId.HasValue && query.WorkId.Value != Guid.Empty;
            bool isFilterByCoin = !isFilterByWork && !string.IsNullOrEmpty(query.Coin);
            bool isFilterByPool = !isFilterByWork && !string.IsNullOrEmpty(query.Pool);
            bool isFilterByWallet = !isFilterByWork && !string.IsNullOrEmpty(query.Wallet);
            bool isFilterByKernel = !isFilterByWork && !string.IsNullOrEmpty(query.Kernel);
            bool isFilterByGpuType = query.GpuType != GpuType.Empty;
            bool isFilterByGpuName = !string.IsNullOrEmpty(query.GpuName);
            bool isFilterByGpuDriver = !string.IsNullOrEmpty(query.GpuDriver);
            for (int i = 0; i < data.Length; i++) {
                var item = data[i];
                bool isInclude = true;
                if (isInclude && isFilterByUser) {
                    isInclude = user.LoginName.Equals(item.LoginName);
                }
                if (isInclude && isFilterByGroup) {
                    isInclude = item.GroupId == query.GroupId.Value;
                }
                if (isInclude) {
                    switch (query.MineState) {
                        case MineStatus.All:
                            break;
                        case MineStatus.Mining:
                            isInclude = item.IsMining == true;
                            break;
                        case MineStatus.UnMining:
                            isInclude = item.IsMining == false;
                            break;
                        default:
                            break;
                    }
                }
                if (isInclude && isFilterByMinerIp) {
                    isInclude = query.MinerIp.Equals(item.MinerIp) || (!string.IsNullOrEmpty(item.LocalIp) && item.LocalIp.Contains(query.MinerIp));
                }
                if (isInclude && isFilterByMinerName) {
                    isInclude = (!string.IsNullOrEmpty(item.MinerName) && item.MinerName.IndexOf(query.MinerName, StringComparison.OrdinalIgnoreCase) != -1)
                        || (!string.IsNullOrEmpty(item.WorkerName) && item.WorkerName.IndexOf(query.MinerName, StringComparison.OrdinalIgnoreCase) != -1);
                }
                if (isInclude && isFilterByVersion) {
                    isInclude = !string.IsNullOrEmpty(item.Version) && item.Version.Contains(query.Version);
                }
                if (isInclude) {
                    if (isFilterByWork) {
                        isInclude = item.WorkId == query.WorkId.Value;
                    }
                    else {
                        if (isInclude && isFilterByCoin) {
                            isInclude = query.Coin.Equals(item.MainCoinCode) || query.Coin.Equals(item.DualCoinCode);
                        }
                        if (isInclude && isFilterByPool) {
                            isInclude = query.Pool.Equals(item.MainCoinPool) || query.Pool.Equals(item.DualCoinPool);
                        }
                        if (isInclude && isFilterByWallet) {
                            isInclude = query.Wallet.Equals(item.MainCoinWallet) || query.Wallet.Equals(item.DualCoinWallet);
                        }
                        if (isInclude && isFilterByKernel) {
                            isInclude = !string.IsNullOrEmpty(item.Kernel) && item.Kernel.IgnoreCaseContains(query.Kernel);
                        }
                    }
                }
                if (isInclude && isFilterByGpuType) {
                    isInclude = item.GpuType == query.GpuType;
                }
                if (isInclude && isFilterByGpuName) {
                    isInclude = !string.IsNullOrEmpty(item.GpuInfo) && item.GpuInfo.IgnoreCaseContains(query.GpuName);
                }
                if (isInclude && isFilterByGpuDriver) {
                    isInclude = !string.IsNullOrEmpty(item.GpuDriver) && item.GpuDriver.IgnoreCaseContains(query.GpuDriver);
                }
                if (isInclude) {
                    list.Add(item);
                }
            }
            total = list.Count();
            switch (query.SortDirection) {
                case SortDirection.Ascending:
                    list = list.OrderBy(a => a.MinerName).ToList();
                    break;
                case SortDirection.Descending:
                    list = list.OrderByDescending(a => a.MinerName).ToList();
                    break;
                default:
                    break;
            }
            coinSnapshots = VirtualRoot.CreateCoinSnapshots(_isPull, DateTime.Now, list, out onlineCount, out miningCount).ToList();
            var results = list.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToList();
            DoCheckIsOnline(results);
            return results;
        }

        public ClientData GetByClientId(Guid clientId) {
            if (!IsReadied) {
                return null;
            }
            _dicByClientId.TryGetValue(clientId, out ClientData clientData);
            return clientData;
        }

        public ClientData GetByObjectId(string objectId) {
            if (!IsReadied) {
                return null;
            }
            if (objectId == null) {
                return null;
            }
            _dicByObjectId.TryGetValue(objectId, out ClientData clientData);
            return clientData;
        }

        public virtual void UpdateClient(string objectId, string propertyName, object value) {
            if (!IsReadied) {
                return;
            }
            if (objectId == null) {
                return;
            }
            if (_dicByObjectId.TryGetValue(objectId, out ClientData clientData)) {
                PropertyInfo propertyInfo = typeof(ClientData).GetProperty(propertyName);
                if (propertyInfo != null) {
                    value = VirtualRoot.ConvertValue(propertyInfo.PropertyType, value);
                    var oldValue = propertyInfo.GetValue(clientData, null);
                    if (oldValue != value) {
                        propertyInfo.SetValue(clientData, value, null);
                        DoUpdateSave(MinerData.Create(clientData));
                    }
                }
            }
        }

        public virtual void UpdateClients(string propertyName, Dictionary<string, object> values) {
            if (!IsReadied) {
                return;
            }
            if (values.Count == 0) {
                return;
            }
            PropertyInfo propertyInfo = typeof(ClientData).GetProperty(propertyName);
            if (propertyInfo != null) {
                values.ChangeValueType(propertyInfo.PropertyType);
                List<MinerData> minerDatas = new List<MinerData>();
                foreach (var kv in values) {
                    string objectId = kv.Key;
                    object value = kv.Value;
                    if (_dicByObjectId.TryGetValue(objectId, out ClientData clientData)) {
                        var oldValue = propertyInfo.GetValue(clientData, null);
                        if (oldValue != value) {
                            propertyInfo.SetValue(clientData, value, null);
                            minerDatas.Add(MinerData.Create(clientData));
                        }
                    }
                }
                DoUpdateSave(minerDatas);
            }
        }

        public void RemoveByObjectId(string objectId) {
            if (!IsReadied) {
                return;
            }
            if (objectId == null) {
                return;
            }
            if (_dicByObjectId.TryGetValue(objectId, out ClientData clientData)) {
                _dicByObjectId.Remove(objectId);
                _dicByClientId.Remove(clientData.ClientId);
                DoRemoveSave(MinerData.Create(clientData));
            }
        }

        public bool IsAnyClientInGroup(Guid groupId) {
            if (!IsReadied) {
                return false;
            }
            return _dicByObjectId.Values.Any(a => a.GroupId == groupId);
        }

        public bool IsAnyClientInWork(Guid workId) {
            if (!IsReadied) {
                return false;
            }
            return _dicByObjectId.Values.Any(a => a.WorkId == workId);
        }

        public IEnumerable<ClientData> AsEnumerable() {
            return _dicByObjectId.Values;
        }
    }
}

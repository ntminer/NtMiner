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
        public ClientDataSetBase(bool isPull, Action<Action<IEnumerable<MinerData>>> doInit) {
            _isPull = isPull;
            doInit((minerDatas) => {
                InitedOn = DateTime.Now;
                IsReadied = true;
                foreach (var item in minerDatas) {
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
            IQueryable<ClientData> data = _dicByObjectId.Values.AsQueryable();
            if (user != null && !user.IsAdmin()) {
                data = data.Where(a => a.CanReadBy(user));
            }
            if (query.GroupId.HasValue && query.GroupId.Value != Guid.Empty) {
                data = data.Where(a => a.GroupId == query.GroupId.Value);
            }
            switch (query.MineState) {
                case MineStatus.All:
                    break;
                case MineStatus.Mining:
                    data = data.Where(a => a.IsMining == true);
                    break;
                case MineStatus.UnMining:
                    data = data.Where(a => a.IsMining == false);
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(query.MinerIp)) {
                data = data.Where(a => a.MinerIp == query.MinerIp);
            }
            if (!string.IsNullOrEmpty(query.MinerName)) {
                data = data.Where(a =>
                (!string.IsNullOrEmpty(a.MinerName) && a.MinerName.IndexOf(query.MinerName, StringComparison.OrdinalIgnoreCase) != -1)
                || (!string.IsNullOrEmpty(a.WorkerName) && a.WorkerName.IndexOf(query.MinerName, StringComparison.OrdinalIgnoreCase) != -1));
            }
            if (!string.IsNullOrEmpty(query.Version)) {
                data = data.Where(a => a.Version != null && a.Version.StartsWith(query.Version, StringComparison.OrdinalIgnoreCase));
            }
            if (query.WorkId.HasValue && query.WorkId.Value != Guid.Empty) {
                data = data.Where(a => a.WorkId == query.WorkId.Value);
            }
            else {
                if (!string.IsNullOrEmpty(query.Coin)) {
                    data = data.Where(a => a.MainCoinCode == query.Coin || a.DualCoinCode == query.Coin);
                }
                if (!string.IsNullOrEmpty(query.Pool)) {
                    data = data.Where(a => a.MainCoinPool == query.Pool || a.DualCoinPool == query.Pool);
                }
                if (!string.IsNullOrEmpty(query.Wallet)) {
                    data = data.Where(a => a.MainCoinWallet == query.Wallet || a.DualCoinWallet == query.Wallet);
                }
                if (!string.IsNullOrEmpty(query.Kernel)) {
                    data = data.Where(a => a.Kernel != null && a.Kernel.StartsWith(query.Kernel, StringComparison.OrdinalIgnoreCase));
                }
            }
            total = data.Count();
            switch (query.SortDirection) {
                case SortDirection.Ascending:
                    data = data.OrderBy(a => a.MinerName);
                    break;
                case SortDirection.Descending:
                    data = data.OrderByDescending(a => a.MinerName);
                    break;
                default:
                    break;
            }
            coinSnapshots = VirtualRoot.CreateCoinSnapshots(_isPull, DateTime.Now, data, out onlineCount, out miningCount).ToList();
            var results = data.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToList();
            foreach (var item in results) {
                // 去除AESPassword避免在网络上传输
                item.AESPassword = string.Empty;
            }
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

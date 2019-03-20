using NTMiner.Profile;
using NTMiner.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTMiner.Core.Profiles {
    internal partial class MinerProfile {
        private class CoinProfileSet {
            private readonly Dictionary<Guid, CoinProfile> _dicById = new Dictionary<Guid, CoinProfile>();
            private readonly INTMinerRoot _root;
            private readonly object _locker = new object();
            private Guid _workId;
            public CoinProfileSet(INTMinerRoot root, Guid workId) {
                _root = root;
                _workId = workId;
            }

            public void Refresh(Guid workId) {
                _workId = workId;
                _dicById.Clear();
            }

            public ICoinProfile GetCoinProfile(Guid coinId) {
                if (_dicById.ContainsKey(coinId)) {
                    return _dicById[coinId];
                }
                lock (_locker) {
                    if (_dicById.ContainsKey(coinId)) {
                        return _dicById[coinId];
                    }
                    CoinProfile coinProfile = CoinProfile.Create(_root, _workId, coinId);
                    _dicById.Add(coinId, coinProfile);
                    return coinProfile;
                }
            }

            public IEnumerable<ICoinProfile> GetCoinProfiles() {
                return _dicById.Values;
            }

            public void SetCoinProfileProperty(Guid coinId, string propertyName, object value) {
                CoinProfile coinProfile = (CoinProfile)GetCoinProfile(coinId);
                coinProfile.SetValue(propertyName, value);
            }

            private class CoinProfile : ICoinProfile {
                private static readonly CoinProfile Empty = new CoinProfile(Guid.Empty, new CoinProfileData());

                public static CoinProfile Create(INTMinerRoot root, Guid workId, Guid coinId) {
                    if (root.CoinSet.TryGetCoin(coinId, out ICoin coin)) {
                        var data = GetCoinProfileData(workId, coin.GetId());
                        if (data == null) {
                            Guid poolId = Guid.Empty;
                            IPool pool = root.PoolSet.OrderBy(a => a.SortNumber).FirstOrDefault(a => a.CoinId == coinId);
                            if (pool != null) {
                                poolId = pool.GetId();
                            }
                            string wallet = coin.TestWallet;
                            Guid coinKernelId = Guid.Empty;
                            ICoinKernel coinKernel = root.CoinKernelSet.OrderBy(a => a.SortNumber).FirstOrDefault(a => a.CoinId == coinId);
                            if (coinKernel != null) {
                                coinKernelId = coinKernel.GetId();
                            }
                            data = CoinProfileData.CreateDefaultData(coinId, poolId, wallet, coinKernelId);
                        }
                        CoinProfile coinProfile = new CoinProfile(workId, data);

                        return coinProfile;
                    }
                    else {
                        return Empty;
                    }
                }

                private readonly Guid _workId;
                private CoinProfileData _data;

                private static CoinProfileData GetCoinProfileData(Guid workId, Guid coinId) {
                    bool isUseJson = workId != Guid.Empty;
                    IRepository<CoinProfileData> repository = NTMinerRoot.CreateLocalRepository<CoinProfileData>(isUseJson);
                    var result = repository.GetByKey(coinId);
                    return result;
                }

                private CoinProfile(Guid workId, CoinProfileData data) {
                    _workId = workId;
                    _data = data ?? throw new ArgumentNullException(nameof(data));
                }

                [IgnoreReflectionSet]
                public Guid CoinId {
                    get => _data.CoinId;
                    private set {
                        _data.CoinId = value;
                    }
                }

                public Guid PoolId {
                    get => _data.PoolId;
                    private set {
                        _data.PoolId = value;
                    }
                }

                public string Wallet {
                    get => _data.Wallet;
                    private set {
                        _data.Wallet = value;
                    }
                }

                public bool IsHideWallet {
                    get => _data.IsHideWallet;
                    private set {
                        _data.IsHideWallet = value;
                    }
                }

                public Guid CoinKernelId {
                    get => _data.CoinKernelId;
                    private set {
                        _data.CoinKernelId = value;
                    }
                }
                public Guid DualCoinPoolId {
                    get => _data.DualCoinPoolId;
                    private set {
                        _data.DualCoinPoolId = value;
                    }
                }

                public string DualCoinWallet {
                    get => _data.DualCoinWallet;
                    private set {
                        _data.DualCoinWallet = value;
                    }
                }

                public bool IsDualCoinHideWallet {
                    get => _data.IsDualCoinHideWallet;
                    private set {
                        _data.IsDualCoinHideWallet = value;
                    }
                }

                private static Dictionary<string, PropertyInfo> _sProperties;
                [IgnoreReflectionSet]
                private static Dictionary<string, PropertyInfo> Properties {
                    get {
                        if (_sProperties == null) {
                            _sProperties = GetPropertiesCanSet<CoinProfile>();
                        }
                        return _sProperties;
                    }
                }

                public void SetValue(string propertyName, object value) {
                    if (Properties.TryGetValue(propertyName, out PropertyInfo propertyInfo)) {
                        if (propertyInfo.CanWrite) {
                            if (propertyInfo.PropertyType == typeof(Guid)) {
                                value = DictionaryExtensions.ConvertToGuid(value);
                            }
                            var oldValue = propertyInfo.GetValue(this, null);
                            if (oldValue != value) {
                                propertyInfo.SetValue(this, value, null);
                                bool isUseJson = _workId != Guid.Empty;
                                IRepository<CoinProfileData> repository = NTMinerRoot.CreateLocalRepository<CoinProfileData>(isUseJson);
                                repository.Update(_data);
                                VirtualRoot.Happened(new CoinProfilePropertyChangedEvent(this.CoinId, propertyName));
                            }
                        }
                    }
                }

                public override string ToString() {
                    if (_data == null) {
                        return string.Empty;
                    }
                    return _data.ToString();
                }
            }
        }
    }
}

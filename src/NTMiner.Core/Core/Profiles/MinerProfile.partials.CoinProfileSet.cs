using NTMiner.Profile;
using NTMiner.Repositories;
using System;
using System.Collections.Generic;
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
                public static readonly CoinProfile Empty = new CoinProfile(NTMinerRoot.Current, Guid.Empty);

                public static CoinProfile Create(INTMinerRoot root, Guid workId, Guid coinId) {
                    if (root.CoinSet.TryGetCoin(coinId, out ICoin coin)) {
                        CoinProfile coinProfile = new CoinProfile(root, workId, coin);

                        return coinProfile;
                    }
                    else {
                        return Empty;
                    }
                }

                private readonly INTMinerRoot _root;
                private readonly Guid _workId;
                private CoinProfileData _data;
                private CoinProfile(INTMinerRoot root, Guid workId) {
                    _root = root;
                    _workId = workId;
                }

                private CoinProfileData GetCoinProfileData(Guid coinId) {
                    if (VirtualRoot.IsControlCenter) {
                        return Server.ControlCenterService.GetCoinProfile(_workId, coinId);
                    }
                    else {
                        bool isUseJson = _workId != Guid.Empty;
                        IRepository<CoinProfileData> repository = NTMinerRoot.CreateLocalRepository<CoinProfileData>(isUseJson);
                        var result = repository.GetByKey(coinId);
                        if (result == null) {
                            result = CoinProfileData.CreateDefaultData(coinId);
                        }
                        return result;
                    }
                }

                private CoinProfile(INTMinerRoot root, Guid workId, ICoin coin) {
                    _root = root;
                    _workId = workId;
                    _data = GetCoinProfileData(coin.GetId());
                    if (_data == null) {
                        throw new ValidationException("未获取到CoinProfileData数据，请重试");
                    }
                }

                [IgnoreReflectionSet]
                public Guid CoinId {
                    get => _data.CoinId;
                    private set {
                        if (_data.CoinId != value) {
                            _data.CoinId = value;
                        }
                    }
                }

                public Guid PoolId {
                    get => _data.PoolId;
                    private set {
                        if (_data.PoolId != value) {
                            _data.PoolId = value;
                        }
                    }
                }

                public string Wallet {
                    get => _data.Wallet;
                    private set {
                        if (_data.Wallet != value) {
                            _data.Wallet = value;
                        }
                    }
                }

                public bool IsHideWallet {
                    get => _data.IsHideWallet;
                    private set {
                        if (_data.IsHideWallet != value) {
                            _data.IsHideWallet = value;
                        }
                    }
                }

                public Guid CoinKernelId {
                    get => _data.CoinKernelId;
                    private set {
                        if (_data.CoinKernelId != value) {
                            _data.CoinKernelId = value;
                        }
                    }
                }
                public Guid DualCoinPoolId {
                    get => _data.DualCoinPoolId;
                    private set {
                        if (_data.DualCoinPoolId != value) {
                            _data.DualCoinPoolId = value;
                        }
                    }
                }

                public string DualCoinWallet {
                    get => _data.DualCoinWallet;
                    private set {
                        if (_data.DualCoinWallet != value) {
                            _data.DualCoinWallet = value;
                        }
                    }
                }

                public bool IsDualCoinHideWallet {
                    get => _data.IsDualCoinHideWallet;
                    private set {
                        if (_data.IsDualCoinHideWallet != value) {
                            _data.IsDualCoinHideWallet = value;
                        }
                    }
                }

                public bool IsOverClockEnabled {
                    get { return _data.IsOverClockEnabled; }
                    private set {
                        _data.IsOverClockEnabled = value;
                    }
                }

                public bool IsOverClockGpuAll {
                    get { return _data.IsOverClockGpuAll; }
                    private set {
                        _data.IsOverClockGpuAll = value;
                    }
                }

                private static Dictionary<string, PropertyInfo> s_properties;
                [IgnoreReflectionSet]
                private static Dictionary<string, PropertyInfo> Properties {
                    get {
                        if (s_properties == null) {
                            s_properties = GetPropertiesCanSet<CoinProfile>();
                        }
                        return s_properties;
                    }
                }

                public void SetValue(string propertyName, object value) {
                    if (Properties.TryGetValue(propertyName, out PropertyInfo propertyInfo)) {
                        if (propertyInfo.CanWrite) {
                            if (propertyInfo.PropertyType == typeof(Guid)) {
                                value = DictionaryExtensions.ConvertToGuid(value);
                            }
                            propertyInfo.SetValue(this, value, null);
                            if (VirtualRoot.IsControlCenter) {
                                Server.ControlCenterService.SetCoinProfilePropertyAsync(_workId, CoinId, propertyName, value, (response, exception) => {
                                    VirtualRoot.Happened(new CoinProfilePropertyChangedEvent(this.CoinId, propertyName));
                                });
                            }
                            else {
                                bool isUseJson = _workId != Guid.Empty;
                                IRepository<CoinProfileData> repository = NTMinerRoot.CreateLocalRepository<CoinProfileData>(isUseJson);
                                repository.Update(_data);
                                VirtualRoot.Happened(new CoinProfilePropertyChangedEvent(this.CoinId, propertyName));
                            }
                        }
                    }
                }
            }
        }
    }
}

using NTMiner.MinerServer;
using NTMiner.Profile;
using NTMiner.Repositories;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NTMiner.Core.Profiles {
    internal partial class MinerProfile {
        private class CoinKernelProfileSet {
            private readonly Dictionary<Guid, CoinKernelProfile> _dicById = new Dictionary<Guid, CoinKernelProfile>();

            private readonly INTMinerRoot _root;
            private readonly object _locker = new object();

            private MineWorkData _mineWorkData;
            public CoinKernelProfileSet(INTMinerRoot root, MineWorkData mineWorkData) {
                _root = root;
                _mineWorkData = mineWorkData;
            }

            public void Refresh(MineWorkData mineWorkData) {
                _mineWorkData = mineWorkData;
                _dicById.Clear();
            }

            public ICoinKernelProfile GetCoinKernelProfile(Guid coinKernelId) {
                if (_dicById.ContainsKey(coinKernelId)) {
                    return _dicById[coinKernelId];
                }
                lock (_locker) {
                    if (_dicById.ContainsKey(coinKernelId)) {
                        return _dicById[coinKernelId];
                    }
                    CoinKernelProfile coinKernelProfile = CoinKernelProfile.Create(_root, _mineWorkData, coinKernelId);
                    _dicById.Add(coinKernelId, coinKernelProfile);

                    return coinKernelProfile;
                }
            }

            public IEnumerable<ICoinKernelProfile> GetCoinKernelProfiles() {
                return _dicById.Values;
            }

            public void SetCoinKernelProfileProperty(Guid coinKernelId, string propertyName, object value) {
                CoinKernelProfile coinKernelProfile = (CoinKernelProfile)GetCoinKernelProfile(coinKernelId);
                coinKernelProfile.SetValue(propertyName, value);
            }

            private class CoinKernelProfile : ICoinKernelProfile {
                private static readonly CoinKernelProfile Empty = new CoinKernelProfile(null, new CoinKernelProfileData());
                public static CoinKernelProfile Create(INTMinerRoot root, MineWorkData mineWorkData, Guid coinKernelId) {
                    if (root.CoinKernelSet.TryGetCoinKernel(coinKernelId, out ICoinKernel coinKernel)) {
                        var data = GetCoinKernelProfileData(mineWorkData, coinKernel.GetId());
                        if (data == null) {
                            data = CoinKernelProfileData.CreateDefaultData(coinKernel.GetId());
                        }
                        CoinKernelProfile coinProfile = new CoinKernelProfile(mineWorkData, data);

                        return coinProfile;
                    }
                    else {
                        return Empty;
                    }
                }

                private readonly MineWorkData _mineWorkData;

                private static CoinKernelProfileData GetCoinKernelProfileData(MineWorkData mineWorkData, Guid coinKernelId) {
                    bool isUseJson = mineWorkData != null;
                    IRepository<CoinKernelProfileData> repository = NTMinerRoot.CreateLocalRepository<CoinKernelProfileData>(isUseJson);
                    var result = repository.GetByKey(coinKernelId);
                    if (result == null) {
                        result = CoinKernelProfileData.CreateDefaultData(coinKernelId);
                    }
                    return result;
                }

                private CoinKernelProfileData _data;
                private CoinKernelProfile(MineWorkData mineWorkData, CoinKernelProfileData data) {
                    _mineWorkData = mineWorkData;
                    _data = data ?? throw new ArgumentNullException(nameof(data));
                }

                [IgnoreReflectionSet]
                public Guid CoinKernelId {
                    get => _data.CoinKernelId;
                    private set {
                        _data.CoinKernelId = value;
                    }
                }

                public bool IsDualCoinEnabled {
                    get => _data.IsDualCoinEnabled;
                    private set {
                        _data.IsDualCoinEnabled = value;
                    }
                }
                public Guid DualCoinId {
                    get => _data.DualCoinId;
                    private set {
                        _data.DualCoinId = value;
                    }
                }

                public double DualCoinWeight {
                    get => _data.DualCoinWeight;
                    private set {
                        _data.DualCoinWeight = value;
                    }
                }

                public bool IsAutoDualWeight {
                    get => _data.IsAutoDualWeight;
                    private set {
                        _data.IsAutoDualWeight = value;
                    }
                }

                public string CustomArgs {
                    get => _data.CustomArgs;
                    private set {
                        _data.CustomArgs = value;
                    }
                }

                private static Dictionary<string, PropertyInfo> _sProperties;
                [IgnoreReflectionSet]
                private static Dictionary<string, PropertyInfo> Properties {
                    get {
                        if (_sProperties == null) {
                            _sProperties = GetPropertiesCanSet<CoinKernelProfile>();
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
                                bool isUseJson = _mineWorkData != null;
                                IRepository<CoinKernelProfileData> repository = NTMinerRoot.CreateLocalRepository<CoinKernelProfileData>(isUseJson);
                                repository.Update(_data);
                                VirtualRoot.Happened(new CoinKernelProfilePropertyChangedEvent(this.CoinKernelId, propertyName));
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

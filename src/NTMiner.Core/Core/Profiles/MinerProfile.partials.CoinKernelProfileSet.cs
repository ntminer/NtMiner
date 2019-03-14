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

            private Guid _workId;
            public CoinKernelProfileSet(INTMinerRoot root, Guid workId) {
                _root = root;
                _workId = workId;
            }

            public void Refresh(Guid workId) {
                _workId = workId;
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
                    CoinKernelProfile coinKernelProfile = CoinKernelProfile.Create(_root, _workId, coinKernelId);
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
                private static readonly CoinKernelProfile Empty = new CoinKernelProfile(Guid.Empty, new CoinKernelProfileData());
                public static CoinKernelProfile Create(INTMinerRoot root, Guid workId, Guid coinKernelId) {
                    if (root.CoinKernelSet.TryGetCoinKernel(coinKernelId, out ICoinKernel coinKernel)) {
                        var data = GetCoinKernelProfileData(workId, coinKernel.GetId());
                        if (data == null) {
                            data = CoinKernelProfileData.CreateDefaultData(coinKernel.GetId());
                            if (VirtualRoot.IsControlCenter) {
                                Server.ControlCenterService.SetCoinKernelProfileAsync(workId, data, callback: null);
                            }
                        }
                        CoinKernelProfile coinProfile = new CoinKernelProfile(workId, data);

                        return coinProfile;
                    }
                    else {
                        return Empty;
                    }
                }

                private readonly Guid _workId;

                private static CoinKernelProfileData GetCoinKernelProfileData(Guid workId, Guid coinKernelId) {
                    if (VirtualRoot.IsControlCenter) {
                        return Server.ControlCenterService.GetCoinKernelProfile(workId, coinKernelId);
                    }
                    else {
                        bool isUseJson = workId != Guid.Empty;
                        IRepository<CoinKernelProfileData> repository = NTMinerRoot.CreateLocalRepository<CoinKernelProfileData>(isUseJson);
                        var result = repository.GetByKey(coinKernelId);
                        if (result == null) {
                            result = CoinKernelProfileData.CreateDefaultData(coinKernelId);
                        }
                        return result;
                    }
                }

                private CoinKernelProfileData _data;
                private CoinKernelProfile(Guid workId, CoinKernelProfileData data) {
                    _workId = workId;
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
                                if (VirtualRoot.IsControlCenter) {
                                    Server.ControlCenterService.SetCoinKernelProfilePropertyAsync(_workId, CoinKernelId, propertyName, value, (response, exception) => {
                                        VirtualRoot.Happened(new CoinKernelProfilePropertyChangedEvent(this.CoinKernelId, propertyName));
                                    });
                                }
                                else {
                                    bool isUseJson = _workId != Guid.Empty;
                                    IRepository<CoinKernelProfileData> repository = NTMinerRoot.CreateLocalRepository<CoinKernelProfileData>(isUseJson);
                                    repository.Update(_data);
                                    VirtualRoot.Happened(new CoinKernelProfilePropertyChangedEvent(this.CoinKernelId, propertyName));
                                }
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

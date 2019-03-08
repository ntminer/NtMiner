using NTMiner.Core.Kernels;
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
                public static readonly CoinKernelProfile Empty = new CoinKernelProfile(NTMinerRoot.Current, Guid.Empty);
                public static CoinKernelProfile Create(INTMinerRoot root, Guid workId, Guid coinKernelId) {
                    if (root.CoinKernelSet.TryGetCoinKernel(coinKernelId, out ICoinKernel coinKernel)) {
                        CoinKernelProfile coinProfile = new CoinKernelProfile(root, workId, coinKernel);

                        return coinProfile;
                    }
                    else {
                        return Empty;
                    }
                }

                private readonly Guid _workId;
                private CoinKernelProfile(INTMinerRoot root, Guid workId) {
                    _root = root;
                    _workId = workId;
                }

                private CoinKernelProfileData GetCoinKernelProfileData(Guid coinKernelId) {
                    if (VirtualRoot.IsControlCenter) {
                        return Server.ProfileService.GetCoinKernelProfile(_workId, coinKernelId);
                    }
                    else {
                        bool isUseJson = _workId != Guid.Empty;
                        IRepository<CoinKernelProfileData> repository = NTMinerRoot.CreateLocalRepository<CoinKernelProfileData>(isUseJson);
                        var result = repository.GetByKey(coinKernelId);
                        if (result == null) {
                            result = CoinKernelProfileData.CreateDefaultData(coinKernelId);
                        }
                        return result;
                    }
                }

                private readonly INTMinerRoot _root;
                private CoinKernelProfileData _data;
                private CoinKernelProfile(INTMinerRoot root, Guid workId, ICoinKernel coinKernel) {
                    _root = root;
                    _workId = workId;
                    _data = GetCoinKernelProfileData(coinKernel.GetId());
                    if (_data == null) {
                        throw new ValidationException("未获取到CoinKernelProfileData数据，请重试");
                    }
                }

                [IgnoreReflectionSet]
                public Guid CoinKernelId {
                    get => _data.CoinKernelId;
                    private set {
                        if (_data.CoinKernelId != value) {
                            _data.CoinKernelId = value;
                        }
                    }
                }

                public bool IsDualCoinEnabled {
                    get => _data.IsDualCoinEnabled;
                    private set {
                        if (_data.IsDualCoinEnabled != value) {
                            _data.IsDualCoinEnabled = value;
                        }
                    }
                }
                public Guid DualCoinId {
                    get => _data.DualCoinId;
                    private set {
                        if (_data.DualCoinId != value) {
                            _data.DualCoinId = value;
                        }
                    }
                }

                public double DualCoinWeight {
                    get => _data.DualCoinWeight;
                    private set {
                        if (_data.DualCoinWeight != value) {
                            _data.DualCoinWeight = value;
                        }
                    }
                }

                public bool IsAutoDualWeight {
                    get => _data.IsAutoDualWeight;
                    private set {
                        if (_data.IsAutoDualWeight != value) {
                            _data.IsAutoDualWeight = value;
                        }
                    }
                }

                public string CustomArgs {
                    get => _data.CustomArgs;
                    private set {
                        if (_data.CustomArgs != value) {
                            _data.CustomArgs = value;
                        }
                    }
                }

                private static Dictionary<string, PropertyInfo> s_properties;
                [IgnoreReflectionSet]
                private static Dictionary<string, PropertyInfo> Properties {
                    get {
                        if (s_properties == null) {
                            s_properties = GetPropertiesCanSet<CoinKernelProfile>();
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
        }
    }
}

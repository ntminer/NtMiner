using NTMiner.Core.Kernels;
using NTMiner.Profile;
using NTMiner.Repositories;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NTMiner.Core.Profiles.Impl {
    public class CoinKernelProfileSet : ICoinKernelProfileSet {
        private readonly Dictionary<Guid, CoinKernelProfile> _dicById = new Dictionary<Guid, CoinKernelProfile>();

        private readonly INTMinerRoot _root;
        private readonly object _locker = new object();

        private readonly Guid _workId;
        public CoinKernelProfileSet(INTMinerRoot root, Guid workId) {
            _root = root;
            _workId = workId;
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
                if (_workId != Guid.Empty) {
                    return Server.ProfileService.GetCoinKernelProfile(_workId, coinKernelId);
                }
                else {
                    IRepository<CoinKernelProfileData> repository = NTMinerRoot.CreateLocalRepository<CoinKernelProfileData>();
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

            private static Dictionary<string, PropertyInfo> _properties;
            private static Dictionary<string, PropertyInfo> Properties {
                get {
                    if (_properties == null) {
                        _properties = new Dictionary<string, PropertyInfo>();
                        foreach (var item in typeof(CoinKernelProfile).GetProperties()) {
                            _properties.Add(item.Name, item);
                        }
                    }
                    return _properties;
                }
            }

            public void SetValue(string propertyName, object value) {
                if (Properties.TryGetValue(propertyName, out PropertyInfo propertyInfo)) {
                    if (propertyInfo.CanWrite) {
                        if (propertyInfo.PropertyType == typeof(Guid)) {
                            value = DictionaryExtensions.ConvertToGuid(value);
                        }
                        propertyInfo.SetValue(this, value, null);
                        if (_workId != Guid.Empty) {
                            if (CommandLineArgs.IsControlCenter) {
                                Server.ControlCenterService.SetCoinKernelProfilePropertyAsync(_workId, CoinKernelId, propertyName, value, isSuccess => {
                                    VirtualRoot.Happened(new CoinKernelProfilePropertyChangedEvent(this.CoinKernelId, propertyName));
                                });
                            }
                        }
                        else {
                            IRepository<CoinKernelProfileData> repository = NTMinerRoot.CreateLocalRepository<CoinKernelProfileData>();
                            repository.Update(_data);
                            VirtualRoot.Happened(new CoinKernelProfilePropertyChangedEvent(this.CoinKernelId, propertyName));
                        }
                    }
                }
            }
        }
    }
}

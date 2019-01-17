using NTMiner.Repositories;
using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace NTMiner.Core.Profiles.Impl {
    public class CoinProfileSet : ICoinProfileSet {
        private readonly Dictionary<Guid, CoinProfile> _dicById = new Dictionary<Guid, CoinProfile>();
        private readonly INTMinerRoot _root;
        private readonly object _locker = new object();

        public CoinProfileSet(INTMinerRoot root) {
            _root = root;
            Global.Logger.InfoDebugLine(this.GetType().FullName + "接入总线");
        }

        public ICoinProfile GetCoinProfile(Guid coinId) {
            if (_dicById.ContainsKey(coinId)) {
                return _dicById[coinId];
            }
            lock (_locker) {
                if (_dicById.ContainsKey(coinId)) {
                    return _dicById[coinId];
                }
                CoinProfile coinProfile = CoinProfile.Create(_root, coinId);
                _dicById.Add(coinId, coinProfile);
                return coinProfile;
            }
        }

        public void SetCoinProfileProperty(Guid coinId, string propertyName, object value) {
            CoinProfile coinProfile = (CoinProfile)GetCoinProfile(coinId);
            coinProfile.SetValue(propertyName, value);
        }

        private class CoinProfile : ICoinProfile {
            public static readonly CoinProfile Empty = new CoinProfile(NTMinerRoot.Current);

            public static CoinProfile Create(INTMinerRoot root, Guid coinId) {
                if (root.CoinSet.TryGetCoin(coinId, out ICoin coin)) {
                    CoinProfile coinProfile = new CoinProfile(root, coin);

                    return coinProfile;
                }
                else {
                    return Empty;
                }
            }

            private readonly INTMinerRoot _root;
            private CoinProfileData _data;
            private CoinProfile(INTMinerRoot root) {
                _root = root;
            }

            private CoinProfileData GetCoinProfileData(Guid coinId) {
                if (CommandLineArgs.IsWorker) {
                    return Server.ProfileService.GetCoinProfile(CommandLineArgs.WorkId, coinId);
                }
                else {
                    IRepository<CoinProfileData> repository = NTMinerRoot.CreateLocalRepository<CoinProfileData>();
                    var result = repository.GetByKey(coinId);
                    if (result == null) {
                        result = CoinProfileData.CreateDefaultData(coinId);
                    }
                    return result;
                }
            }

            private CoinProfile(INTMinerRoot root, ICoin coin) {
                _root = root;
                _data = GetCoinProfileData(coin.GetId());
                if (_data == null) {
                    throw new ValidationException("未获取到CoinProfileData数据，请重试");
                }
            }

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
                set {
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
                set {
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

            private static Dictionary<string, PropertyInfo> _properties;

            private static Dictionary<string, PropertyInfo> Properties {
                get {
                    if (_properties == null) {
                        _properties = new Dictionary<string, PropertyInfo>();
                        foreach (var item in typeof(CoinProfile).GetProperties()) {
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
                        if (CommandLineArgs.IsWorker) {
                            if (CommandLineArgs.IsControlCenter) {
                                Server.ControlCenterService.SetCoinProfileProperty(CommandLineArgs.WorkId, CoinId, propertyName, value, isSuccess => {
                                    Global.Happened(new CoinProfilePropertyChangedEvent(this.CoinId, propertyName));
                                });
                            }
                        }
                        else {
                            IRepository<CoinProfileData> repository = NTMinerRoot.CreateLocalRepository<CoinProfileData>();
                            repository.Update(_data);
                            Global.Happened(new CoinProfilePropertyChangedEvent(this.CoinId, propertyName));
                        }
                    }
                }
            }
        }
    }
}

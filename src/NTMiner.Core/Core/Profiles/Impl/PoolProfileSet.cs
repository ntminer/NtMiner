using NTMiner.MinerServer;
using NTMiner.Repositories;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NTMiner.Core.Profiles.Impl {
    public class PoolProfileSet : IPoolProfileSet {
        private readonly Dictionary<Guid, PoolProfile> _dicById = new Dictionary<Guid, PoolProfile>();
        private readonly INTMinerRoot _root;
        private readonly object _locker = new object();

        public PoolProfileSet(INTMinerRoot root) {
            _root = root;
        }

        public IPoolProfile GetPoolProfile(Guid poolId) {
            if (_dicById.ContainsKey(poolId)) {
                return _dicById[poolId];
            }
            lock (_locker) {
                if (_dicById.ContainsKey(poolId)) {
                    return _dicById[poolId];
                }
                PoolProfile coinProfile = PoolProfile.Create(_root, poolId);
                _dicById.Add(poolId, coinProfile);
                return coinProfile;
            }
        }

        public void SetPoolProfileProperty(Guid poolId, string propertyName, object value) {
            PoolProfile coinProfile = (PoolProfile)GetPoolProfile(poolId);
            coinProfile.SetValue(propertyName, value);
        }

        public class PoolProfile : IPoolProfile {
            public static readonly PoolProfile Empty = new PoolProfile(NTMinerRoot.Current);

            public static PoolProfile Create(INTMinerRoot root, Guid poolIdId) {
                if (root.PoolSet.TryGetPool(poolIdId, out IPool pool)) {
                    PoolProfile coinProfile = new PoolProfile(root, pool);

                    return coinProfile;
                }
                else {
                    return Empty;
                }
            }

            private readonly INTMinerRoot _root;
            private PoolProfileData _data;
            private PoolProfile(INTMinerRoot root) {
                _root = root;
            }

            private PoolProfileData GetPoolProfileData(Guid poolId) {
                if (CommandLineArgs.IsWorker) {
                    return Server.ProfileService.GetPoolProfile(CommandLineArgs.WorkId, poolId);
                }
                else {
                    IRepository<PoolProfileData> repository = NTMinerRoot.CreateLocalRepository<PoolProfileData>();
                    var result = repository.GetByKey(poolId);
                    if (result == null) {
                        // 如果本地未设置用户名密码则使用默认的测试用户名密码
                        result = PoolProfileData.CreateDefaultData(poolId);
                        if (_root.PoolSet.TryGetPool(poolId, out IPool pool)) {
                            result.UserName = pool.UserName;
                            result.Password = pool.Password;
                        }
                    }
                    return result;
                }
            }

            private PoolProfile(INTMinerRoot root, IPool pool) {
                _root = root;
                _data = GetPoolProfileData(pool.GetId());
                if (_data == null) {
                    throw new ValidationException("未获取到PoolProfileData数据，请重试");
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

            public string UserName {
                get => _data.UserName;
                private set {
                    if (_data.UserName != value) {
                        _data.UserName = value;
                    }
                }
            }

            public string Password {
                get => _data.Password;
                private set {
                    if (_data.Password != value) {
                        _data.Password = value;
                    }
                }
            }

            private static Dictionary<string, PropertyInfo> _properties;

            private static Dictionary<string, PropertyInfo> Properties {
                get {
                    if (_properties == null) {
                        _properties = new Dictionary<string, PropertyInfo>();
                        foreach (var item in typeof(PoolProfile).GetProperties()) {
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
                                Server.ControlCenterService.SetPoolProfilePropertyAsync(CommandLineArgs.WorkId, PoolId, propertyName, value, isSuccess => {
                                    VirtualRoot.Happened(new PoolProfilePropertyChangedEvent(this.PoolId, propertyName));
                                });
                            }
                        }
                        else {
                            IRepository<PoolProfileData> repository = NTMinerRoot.CreateLocalRepository<PoolProfileData>();
                            repository.Update(_data);
                            VirtualRoot.Happened(new PoolProfilePropertyChangedEvent(this.PoolId, propertyName));
                        }
                    }
                }
            }
        }
    }
}

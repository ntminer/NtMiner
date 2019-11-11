using NTMiner.Profile;
using NTMiner.Repositories;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NTMiner.Core.Profiles {
    internal partial class MinerProfile {
        private class PoolProfileSet {
            private readonly Dictionary<Guid, PoolProfile> _dicById = new Dictionary<Guid, PoolProfile>();
            private readonly INTMinerRoot _root;
            private readonly object _locker = new object();

            public PoolProfileSet(INTMinerRoot root) {
                _root = root;
            }

            public void Refresh() {
                _dicById.Clear();
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

            public IEnumerable<IPoolProfile> GetPoolProfiles() {
                return _dicById.Values;
            }

            public void SetPoolProfileProperty(Guid poolId, string propertyName, object value) {
                PoolProfile coinProfile = (PoolProfile)GetPoolProfile(poolId);
                coinProfile.SetValue(propertyName, value);
            }

            public class PoolProfile : IPoolProfile {
                private static readonly PoolProfile Empty = new PoolProfile(new PoolProfileData());

                public static PoolProfile Create(INTMinerRoot root, Guid poolIdId) {
                    if (root.ServerContext.PoolSet.TryGetPool(poolIdId, out IPool pool)) {
                        var data = GetPoolProfileData(root, pool.GetId());
                        if (data == null) {
                            data = PoolProfileData.CreateDefaultData(pool);
                        }
                        PoolProfile coinProfile = new PoolProfile(data);

                        return coinProfile;
                    }
                    else {
                        return Empty;
                    }
                }

                private readonly PoolProfileData _data;

                private static PoolProfileData GetPoolProfileData(INTMinerRoot root, Guid poolId) {
                    var repository = NTMinerRoot.CreateLocalRepository<PoolProfileData>();
                    var result = repository.GetByKey(poolId);
                    if (result == null) {
                        if (root.ServerContext.PoolSet.TryGetPool(poolId, out IPool pool)) {
                            // 如果本地未设置用户名密码则使用默认的测试用户名密码
                            result = PoolProfileData.CreateDefaultData(pool);
                        }
                    }
                    return result;
                }

                private PoolProfile(PoolProfileData data) {
                    _data = data ?? throw new ArgumentNullException(nameof(data));
                }

                [IgnoreReflectionSet]
                public Guid PoolId {
                    get => _data.PoolId;
                    private set {
                        _data.PoolId = value;
                    }
                }

                public string UserName {
                    get => _data.UserName;
                    private set {
                        _data.UserName = value;
                    }
                }

                public string Password {
                    get => _data.Password;
                    private set {
                        _data.Password = value;
                    }
                }

                private static Dictionary<string, PropertyInfo> _sProperties;
                [IgnoreReflectionSet]
                private static Dictionary<string, PropertyInfo> Properties {
                    get {
                        if (_sProperties == null) {
                            _sProperties = GetPropertiesCanSet<PoolProfile>();
                        }
                        return _sProperties;
                    }
                }

                public void SetValue(string propertyName, object value) {
                    if (Properties.TryGetValue(propertyName, out PropertyInfo propertyInfo)) {
                        if (propertyInfo.CanWrite) {
                            if (propertyInfo.PropertyType == typeof(Guid)) {
                                value = VirtualRoot.ConvertToGuid(value);
                            }
                            var oldValue = propertyInfo.GetValue(this, null);
                            if (oldValue != value) {
                                propertyInfo.SetValue(this, value, null);
                                var repository = NTMinerRoot.CreateLocalRepository<PoolProfileData>();
                                repository.Update(_data);
                                VirtualRoot.RaiseEvent(new PoolProfilePropertyChangedEvent(this.PoolId, propertyName));
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

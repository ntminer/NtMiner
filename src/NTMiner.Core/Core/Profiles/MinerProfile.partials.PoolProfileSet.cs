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

            private Guid _workId;
            public PoolProfileSet(INTMinerRoot root, Guid workId) {
                _root = root;
                _workId = workId;
            }

            public void Refresh(Guid workId) {
                _workId = workId;
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
                    PoolProfile coinProfile = PoolProfile.Create(_root, _workId, poolId);
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
                public static readonly PoolProfile Empty = new PoolProfile(NTMinerRoot.Current, Guid.Empty);

                private readonly Guid _workId;
                public static PoolProfile Create(INTMinerRoot root, Guid workId, Guid poolIdId) {
                    if (root.PoolSet.TryGetPool(poolIdId, out IPool pool)) {
                        PoolProfile coinProfile = new PoolProfile(root, workId, pool);

                        return coinProfile;
                    }
                    else {
                        return Empty;
                    }
                }

                private readonly INTMinerRoot _root;
                private PoolProfileData _data;
                private PoolProfile(INTMinerRoot root, Guid workId) {
                    _root = root;
                    _workId = workId;
                }

                private PoolProfileData GetPoolProfileData(Guid poolId) {
                    if (VirtualRoot.IsControlCenter) {
                        return Server.ControlCenterService.GetPoolProfile(_workId, poolId);
                    }
                    else {
                        bool isUseJson = _workId != Guid.Empty;
                        IRepository<PoolProfileData> repository = NTMinerRoot.CreateLocalRepository<PoolProfileData>(isUseJson);
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

                private PoolProfile(INTMinerRoot root, Guid workId, IPool pool) {
                    _root = root;
                    _workId = workId;
                    _data = GetPoolProfileData(pool.GetId());
                    if (_data == null) {
                        throw new ValidationException("未获取到PoolProfileData数据，请重试");
                    }
                }

                [IgnoreReflectionSet]
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

                private static Dictionary<string, PropertyInfo> s_properties;
                [IgnoreReflectionSet]
                private static Dictionary<string, PropertyInfo> Properties {
                    get {
                        if (s_properties == null) {
                            s_properties = GetPropertiesCanSet<PoolProfile>();
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
                                Server.ControlCenterService.SetPoolProfilePropertyAsync(_workId, PoolId, propertyName, value, (response, exception) => {
                                    VirtualRoot.Happened(new PoolProfilePropertyChangedEvent(this.PoolId, propertyName));
                                });
                            }
                            else {
                                bool isUseJson = _workId != Guid.Empty;
                                IRepository<PoolProfileData> repository = NTMinerRoot.CreateLocalRepository<PoolProfileData>(isUseJson);
                                repository.Update(_data);
                                VirtualRoot.Happened(new PoolProfilePropertyChangedEvent(this.PoolId, propertyName));
                            }
                        }
                    }
                }
            }
        }
    }
}

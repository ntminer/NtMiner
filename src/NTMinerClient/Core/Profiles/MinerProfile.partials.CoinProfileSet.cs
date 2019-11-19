using NTMiner.Profile;
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
            public CoinProfileSet(INTMinerRoot root) {
                _root = root;
            }

            public void Refresh() {
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
                    CoinProfile coinProfile = CoinProfile.Create(_root, coinId);
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
                private static readonly CoinProfile Empty = new CoinProfile(new CoinProfileData());

                public static CoinProfile Create(INTMinerRoot root, Guid coinId) {
                    if (root.ServerContext.CoinSet.TryGetCoin(coinId, out ICoin coin)) {
                        var data = GetCoinProfileData(coin.GetId());
                        if (data == null) {
                            Guid poolId = Guid.Empty;
                            IPool pool = root.ServerContext.PoolSet.AsEnumerable().OrderBy(a => a.SortNumber).FirstOrDefault(a => a.CoinId == coinId);
                            if (pool != null) {
                                poolId = pool.GetId();
                            }
                            string wallet = coin.TestWallet;
                            Guid coinKernelId = GetDefaultCoinKernelId(coin);
                            data = CoinProfileData.CreateDefaultData(coinId, poolId, wallet, coinKernelId);
                        }
                        else {
                            if (!root.ServerContext.CoinKernelSet.TryGetCoinKernel(data.CoinKernelId, out ICoinKernel coinKernel)) {
                                data.CoinKernelId = GetDefaultCoinKernelId(coin);
                            }
                        }
                        CoinProfile coinProfile = new CoinProfile(data);

                        return coinProfile;
                    }
                    else {
                        return Empty;
                    }
                }

                /// <summary>
                /// 选择默认内核
                /// </summary>
                /// <param name="coin"></param>
                /// <returns></returns>
                private static Guid GetDefaultCoinKernelId(ICoin coin) {
                    var root = NTMinerRoot.Instance;
                    Guid coinKernelId = Guid.Empty;
                    bool noneGpu = false;
                    if (root.GpuSet.GpuType == GpuType.Empty) {
                        noneGpu = true;
                    }
                    List<ICoinKernel> coinKernels;
                    if (noneGpu) {
                        coinKernels = root.ServerContext.CoinKernelSet.Where(a => a.CoinId == coin.GetId()).ToList();
                    }
                    else {
                        coinKernels = root.ServerContext.CoinKernelSet.Where(a => a.CoinId == coin.GetId() && a.SupportedGpu.IsSupportedGpu(root.GpuSet.GpuType)).ToList();
                    }
                    var items = new List<Tuple<Guid, IKernel>>(coinKernels.Count);
                    foreach (var item in coinKernels) {
                        if (root.ServerContext.KernelSet.TryGetKernel(item.KernelId, out IKernel kernel)) {
                            items.Add(new Tuple<Guid, IKernel>(item.GetId(), kernel));
                        }
                    }
                    items = items.OrderBy(a => a.Item2.Code).ThenByDescending(a => a.Item2.Version).ToList();
                    Guid kernelBrandId = coin.GetKernelBrandId(root.GpuSet.GpuType);
                    if (kernelBrandId == Guid.Empty) {
                        coinKernelId = items.Select(a => a.Item1).FirstOrDefault();
                    }
                    else {
                        coinKernelId = items.Where(a => a.Item2.BrandId == kernelBrandId).Select(a => a.Item1).FirstOrDefault();
                    }
                    return coinKernelId;
                }

                private CoinProfileData _data;

                private static CoinProfileData GetCoinProfileData(Guid coinId) {
                    var repository = NTMinerRoot.CreateLocalRepository<CoinProfileData>();
                    var result = repository.GetByKey(coinId);
                    return result;
                }

                private CoinProfile(CoinProfileData data) {
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

                public Guid PoolId1 {
                    get => _data.PoolId1;
                    private set {
                        _data.PoolId1 = value;
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

                public double CalcInput {
                    get => _data.CalcInput;
                    private set {
                        _data.CalcInput = value;
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
                                value = VirtualRoot.ConvertToGuid(value);
                            }
                            var oldValue = propertyInfo.GetValue(this, null);
                            if (oldValue != value) {
                                propertyInfo.SetValue(this, value, null);
                                var repository = NTMinerRoot.CreateLocalRepository<CoinProfileData>();
                                repository.Update(_data);
                                VirtualRoot.RaiseEvent(new CoinProfilePropertyChangedEvent(this.CoinId, propertyName));
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

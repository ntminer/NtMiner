using NTMiner.Core.Profiles.Impl;
using NTMiner.MinerServer;
using NTMiner.Profile;
using NTMiner.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NTMiner.Core.Profiles {
    internal partial class MinerProfile : IWorkProfile {
        private readonly INTMinerRoot _root;

        private MinerProfileData _data = null;
        private CoinKernelProfileSet _coinKernelProfileSet;
        private CoinProfileSet _coinProfileSet;
        private PoolProfileSet _poolProfileSet;
        private WalletSet _walletSet;

        private bool isUseJson;
        public MinerProfile(INTMinerRoot root, MineWorkData mineWorkData) {
            _root = root;
            Init(root, mineWorkData);
        }

        public void ReInit(INTMinerRoot root, MineWorkData mineWorkData) {
            Init(root, mineWorkData);
        }

        #region Init
        private void Init(INTMinerRoot root, MineWorkData mineWorkData) {
            MineWork = mineWorkData;
            isUseJson = mineWorkData != null;
            if (isUseJson) {
                _data = NTMinerRoot.LocalJson.MinerProfile;
            }
            else {
                IRepository<MinerProfileData> repository = NTMinerRoot.CreateLocalRepository<MinerProfileData>(false);
                _data = repository.GetAll().FirstOrDefault();
            }
            if (_data == null) {
                Guid coinId = Guid.Empty;
                ICoin coin = root.CoinSet.OrderBy(a => a.SortNumber).FirstOrDefault();
                if (coin != null) {
                    coinId = coin.GetId();
                }
                _data = MinerProfileData.CreateDefaultData(coinId);
            }
            if (_coinProfileSet == null) {
                _coinProfileSet = new CoinProfileSet(root, mineWorkData);
            }
            else {
                _coinProfileSet.Refresh(mineWorkData);
            }
            if (_coinKernelProfileSet == null) {
                _coinKernelProfileSet = new CoinKernelProfileSet(root, mineWorkData);
            }
            else {
                _coinKernelProfileSet.Refresh(mineWorkData);
            }
            if (_poolProfileSet == null) {
                _poolProfileSet = new PoolProfileSet(root, mineWorkData);
            }
            else {
                _poolProfileSet.Refresh(mineWorkData);
            }
            if (_walletSet == null) {
                _walletSet = new WalletSet(root);
            }
            else {
                _walletSet.Refresh();
            }
        }
        #endregion

        #region methods
        public ICoinKernelProfile GetCoinKernelProfile(Guid coinKernelId) {
            return _coinKernelProfileSet.GetCoinKernelProfile(coinKernelId);
        }

        public void SetCoinKernelProfileProperty(Guid coinKernelId, string propertyName, object value) {
            string coinCode = "意外的币种";
            string kernelName = "意外的内核";
            if (_root.CoinKernelSet.TryGetCoinKernel(coinKernelId, out ICoinKernel coinKernel)) {
                if (_root.CoinSet.TryGetCoin(coinKernel.CoinId, out ICoin coin)) {
                    coinCode = coin.Code;
                }
                if (_root.KernelSet.TryGetKernel(coinKernel.KernelId, out IKernel kernel)) {
                    kernelName = kernel.GetFullName();
                }
                _coinKernelProfileSet.SetCoinKernelProfileProperty(coinKernelId, propertyName, value);
            }
            Write.DevLine($"SetCoinKernelProfileProperty({coinCode}, {kernelName}, {propertyName}, {value})");
        }

        public ICoinProfile GetCoinProfile(Guid coinId) {
            return _coinProfileSet.GetCoinProfile(coinId);
        }

        public void SetCoinProfileProperty(Guid coinId, string propertyName, object value) {
            string coinCode = "意外的币种";
            if (_root.CoinSet.TryGetCoin(coinId, out ICoin coin)) {
                _coinProfileSet.SetCoinProfileProperty(coinId, propertyName, value);
                coinCode = coin.Code;
            }
            Write.DevLine($"SetCoinProfileProperty({coinCode}, {propertyName}, {value})");
        }

        public IPoolProfile GetPoolProfile(Guid poolId) {
            return _poolProfileSet.GetPoolProfile(poolId);
        }

        public void SetPoolProfileProperty(Guid poolId, string propertyName, object value) {
            string poolName = "意外的矿池";
            if (_root.PoolSet.TryGetPool(poolId, out IPool pool)) {
                poolName = pool.Name;
                if (!pool.IsUserMode) {
                    Write.DevLine("不是用户名密码模式矿池", ConsoleColor.Green);
                    return;
                }
                _poolProfileSet.SetPoolProfileProperty(poolId, propertyName, value);
            }
            Write.DevLine($"SetPoolProfileProperty({poolName}, {propertyName}, {value})");
        }

        public bool TryGetWallet(Guid walletId, out IWallet wallet) {
            return _walletSet.TryGetWallet(walletId, out wallet);
        }

        public List<IWallet> GetWallets() {
            return _walletSet.GetWallets().ToList();
        }

        public List<ICoinKernelProfile> GetCoinKernelProfiles() {
            return _coinKernelProfileSet.GetCoinKernelProfiles().ToList();
        }

        public List<ICoinProfile> GetCoinProfiles() {
            return _coinProfileSet.GetCoinProfiles().ToList();
        }

        public List<IPool> GetPools() {
            return _root.PoolSet.ToList();
        }

        public List<IPoolProfile> GetPoolProfiles() {
            return _poolProfileSet.GetPoolProfiles().ToList();
        }
        #endregion

        #region properties
        [IgnoreReflectionSet]
        public IMineWork MineWork { get; private set; }

        public Guid GetId() {
            return this.Id;
        }

        [IgnoreReflectionSet]
        public Guid Id {
            get { return _data.Id; }
            private set {
                _data.Id = value;
            }
        }

        public string MinerName {
            get {
                if (string.IsNullOrEmpty(_data.MinerName)) {
                    _data.MinerName = NTMinerRoot.GetThisPcName();
                }
                return _data.MinerName;
            }
            set {
                if (string.IsNullOrEmpty(value)) {
                    value = NTMinerRoot.GetThisPcName();
                }
                _data.MinerName = value;
            }
        }

        public bool IsNoShareRestartKernel {
            get => _data.IsNoShareRestartKernel;
            private set {
                _data.IsNoShareRestartKernel = value;
            }
        }
        public int NoShareRestartKernelMinutes {
            get => _data.NoShareRestartKernelMinutes;
            private set {
                _data.NoShareRestartKernelMinutes = value;
            }
        }
        public bool IsPeriodicRestartKernel {
            get => _data.IsPeriodicRestartKernel;
            private set {
                _data.IsPeriodicRestartKernel = value;
            }
        }
        public int PeriodicRestartKernelHours {
            get => _data.PeriodicRestartKernelHours;
            private set {
                _data.PeriodicRestartKernelHours = value;
            }
        }
        public bool IsPeriodicRestartComputer {
            get => _data.IsPeriodicRestartComputer;
            private set {
                _data.IsPeriodicRestartComputer = value;
            }
        }
        public int PeriodicRestartComputerHours {
            get => _data.PeriodicRestartComputerHours;
            private set {
                _data.PeriodicRestartComputerHours = value;
            }
        }

        public bool IsAutoRestartKernel {
            get {
                return _data.IsAutoRestartKernel;
            }
            private set {
                _data.IsAutoRestartKernel = value;
            }
        }

        public Guid CoinId {
            get => _data.CoinId;
            private set {
                _data.CoinId = value;
            }
        }
        #endregion

        private static Dictionary<string, PropertyInfo> s_properties;
        [IgnoreReflectionSet]
        private static Dictionary<string, PropertyInfo> Properties {
            get {
                if (s_properties == null) {
                    s_properties = GetPropertiesCanSet<MinerProfile>();
                }
                return s_properties;
            }
        }

        private static Dictionary<string, PropertyInfo> GetPropertiesCanSet<T>() {
            var properties = new Dictionary<string, PropertyInfo>();
            Type attrubuteType = typeof(IgnoreReflectionSetAttribute);
            foreach (var propertyInfo in typeof(T).GetProperties().Where(a => a.CanWrite)) {
                if (propertyInfo.GetCustomAttributes(attrubuteType, inherit: false).Length > 0) {
                    continue;
                }
                properties.Add(propertyInfo.Name, propertyInfo);
            }
            return properties;
        }

        public void SetMinerProfileProperty(string propertyName, object value) {
            if (Properties.TryGetValue(propertyName, out PropertyInfo propertyInfo)) {
                if (propertyInfo.CanWrite) {
                    if (propertyInfo.PropertyType == typeof(Guid)) {
                        value = DictionaryExtensions.ConvertToGuid(value);
                    }
                    object oldValue = propertyInfo.GetValue(this, null);
                    if (oldValue != value) {
                        propertyInfo.SetValue(this, value, null);
                        IRepository<MinerProfileData> repository = NTMinerRoot.CreateLocalRepository<MinerProfileData>(isUseJson);
                        repository.Update(_data);
                        VirtualRoot.Happened(new MinerProfilePropertyChangedEvent(propertyName));
                        Write.DevLine($"SetMinerProfileProperty({propertyName}, {value})");
                    }
                }
            }
        }

        public object GetValue(string propertyName) {
            if (Properties.TryGetValue(propertyName, out PropertyInfo propertyInfo)) {
                if (propertyInfo.CanRead) {
                    return propertyInfo.GetValue(this, null);
                }
            }
            return null;
        }

        public string GetSha1() {
            StringBuilder sb = new StringBuilder();
            if (_data != null) {
                sb.Append(_data.ToString());
            }

            if (this.CoinId != Guid.Empty) {
                ICoinProfile coinProfile = GetCoinProfile(this.CoinId);
                if (coinProfile != null) {
                    sb.Append(coinProfile.ToString());
                    ICoinKernelProfile coinKernelProfile = GetCoinKernelProfile(coinProfile.CoinKernelId);
                    if (coinKernelProfile != null) {
                        sb.Append(coinKernelProfile.ToString());
                        if (coinKernelProfile.IsDualCoinEnabled) {
                            ICoinProfile dualCoinProfile = GetCoinProfile(coinKernelProfile.DualCoinId);
                            if (dualCoinProfile != null) {
                                sb.Append(dualCoinProfile.ToString());
                                IPoolProfile dualCoinPoolProfile = GetPoolProfile(dualCoinProfile.PoolId);
                                if (dualCoinPoolProfile != null) {
                                    sb.Append(dualCoinPoolProfile.ToString());
                                }
                            }
                            ICoinKernelProfile dualCoinKernelProfile = GetCoinKernelProfile(coinKernelProfile.DualCoinId);
                            if (dualCoinKernelProfile != null) {
                                sb.Append(dualCoinKernelProfile.ToString());
                            }
                        }
                    }
                    IPoolProfile poolProfile = GetPoolProfile(coinProfile.PoolId);
                    if (poolProfile != null) {
                        sb.Append(poolProfile.ToString());
                    }
                }
            }

            return HashUtil.Sha1(sb.ToString());
        }
    }
}

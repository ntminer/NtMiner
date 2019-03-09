using NTMiner.Core.Impl;
using NTMiner.MinerServer;
using NTMiner.Profile;
using NTMiner.Repositories;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTMiner.Core.Profiles {
    internal partial class MinerProfile : IWorkProfile {
        public class IgnoreReflectionSetAttribute : Attribute { }

        private readonly INTMinerRoot _root;

        private MinerProfileData _data;
        private Guid _workId;
        private CoinKernelProfileSet _coinKernelProfileSet;
        private CoinProfileSet _coinProfileSet;
        private GpuOverClockDataSet _gpuOverClockDatakSet;
        private PoolProfileSet _poolProfileSet;
        private WalletSet _walletSet;
        private IUserSet _userSet;

        public MinerProfile(INTMinerRoot root, Guid workId) {
            _root = root;
            Init(root, workId);
            VirtualRoot.Accept<SwichMinerProfileCommand>(
                "处理切换MinerProfile命令",
                LogEnum.Console,
                action: message => {
                    Init(root, message.WorkId);
                    VirtualRoot.Happened(new MinerProfileSwichedEvent());
                });
        }

        private void Init(INTMinerRoot root, Guid workId) {
            _workId = workId;
            if (_coinKernelProfileSet == null) {
                _coinKernelProfileSet = new CoinKernelProfileSet(root, workId);
            }
            else {
                _coinKernelProfileSet.Refresh(workId);
            }
            if (_coinProfileSet == null) {
                _coinProfileSet = new CoinProfileSet(root, workId);
            }
            else {
                _coinProfileSet.Refresh(workId);
            }
            if (_gpuOverClockDatakSet == null) {
                _gpuOverClockDatakSet = new GpuOverClockDataSet(root, workId);
            }
            else {
                _gpuOverClockDatakSet.Refresh(workId);
            }
            if (_poolProfileSet == null) {
                _poolProfileSet = new PoolProfileSet(root, workId);
            }
            else {
                _poolProfileSet.Refresh(workId);
            }
            if (_walletSet == null) {
                _walletSet = new WalletSet(root, workId);
            }
            else {
                _walletSet.Refresh(workId);
            }
            if (_userSet == null) {
                if (VirtualRoot.IsControlCenter) {
                    _userSet = new MinerServer.Impl.UserSet();
                }
                else {
                    _userSet = new UserSet(isUseJson: _workId != Guid.Empty);
                }
            }
            else {
                _userSet.Refresh();
            }
            if (VirtualRoot.IsControlCenter) {
                MineWork = new MineWorkData();
            }
            else {
                if (workId != Guid.Empty) {
                    MineWork = LocalJson.Instance.MineWork;
                }
                else {
                    MineWork = new MineWorkData();
                }
            }
            _data = null;
            if (VirtualRoot.IsControlCenter) {
                _data = Server.ControlCenterService.GetMinerProfile(_workId);
            }
            else {
                bool isUseJson = _workId != Guid.Empty;
                if (isUseJson) {
                    _data = LocalJson.Instance.MinerProfile;
                }
                else {
                    IRepository<MinerProfileData> repository = NTMinerRoot.CreateLocalRepository<MinerProfileData>(isUseJson);
                    _data = repository.GetAll().FirstOrDefault();
                }
            }
            if (_data == null) {
                _data = MinerProfileData.CreateDefaultData();
            }
        }

        public ICoinKernelProfile GetCoinKernelProfile(Guid coinKernelId) {
            return _coinKernelProfileSet.GetCoinKernelProfile(coinKernelId);
        }

        public void SetCoinKernelProfileProperty(Guid coinKernelId, string propertyName, object value) {
            _coinKernelProfileSet.SetCoinKernelProfileProperty(coinKernelId, propertyName, value);
        }

        public ICoinProfile GetCoinProfile(Guid coinId) {
            return _coinProfileSet.GetCoinProfile(coinId);
        }

        public void SetCoinProfileProperty(Guid coinId, string propertyName, object value) {
            _coinProfileSet.SetCoinProfileProperty(coinId, propertyName, value);
        }

        public IPoolProfile GetPoolProfile(Guid poolId) {
            return _poolProfileSet.GetPoolProfile(poolId);
        }

        public void SetPoolProfileProperty(Guid poolId, string propertyName, object value) {
            _poolProfileSet.SetPoolProfileProperty(poolId, propertyName, value);
        }

        public bool TryGetWallet(Guid walletId, out IWallet wallet) {
            return _walletSet.TryGetWallet(walletId, out wallet);
        }

        public IGpuProfile GetGpuOverClockData(Guid coinId, int gpuIndex) {
            return _gpuOverClockDatakSet.GetGpuOverClockData(coinId, gpuIndex);
        }

        public IUser GetUser(string loginName) {
            return _userSet.GetUser(loginName);
        }

        public List<IWallet> GetWallets() {
            return _walletSet.GetAllWallets().ToList();
        }

        public List<ICoinKernelProfile> GetCoinKernelProfiles() {
            return _coinKernelProfileSet.GetCoinKernelProfiles().ToList();
        }

        public List<ICoinProfile> GetCoinProfiles() {
            return _coinProfileSet.GetCoinProfiles().ToList();
        }

        public List<IGpuProfile> GetGpuOverClocks() {
            return _gpuOverClockDatakSet.GetGpuOverClocks().ToList();
        }

        public List<IPool> GetPools() {
            return _root.PoolSet.ToList();
        }

        public List<IPoolProfile> GetPoolProfiles() {
            return _poolProfileSet.GetPoolProfiles().ToList();
        }

        public List<IUser> GetUsers() {
            return _userSet.ToList();
        }

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

        public bool IsAutoBoot {
            get => _data.IsAutoBoot;
            private set {
                if (_data.IsAutoBoot != value) {
                    _data.IsAutoBoot = value;
                    NTMinerRegistry.SetIsAutoBoot(value);
                }
            }
        }

        public bool IsNoShareRestartKernel {
            get => _data.IsNoShareRestartKernel;
            private set {
                if (_data.IsNoShareRestartKernel != value) {
                    _data.IsNoShareRestartKernel = value;
                }
            }
        }
        public int NoShareRestartKernelMinutes {
            get => _data.NoShareRestartKernelMinutes;
            private set {
                if (_data.NoShareRestartKernelMinutes != value) {
                    _data.NoShareRestartKernelMinutes = value;
                }
            }
        }
        public bool IsPeriodicRestartKernel {
            get => _data.IsPeriodicRestartKernel;
            private set {
                if (_data.IsPeriodicRestartKernel != value) {
                    _data.IsPeriodicRestartKernel = value;
                }
            }
        }
        public int PeriodicRestartKernelHours {
            get => _data.PeriodicRestartKernelHours;
            private set {
                if (_data.PeriodicRestartKernelHours != value) {
                    _data.PeriodicRestartKernelHours = value;
                }
            }
        }
        public bool IsPeriodicRestartComputer {
            get => _data.IsPeriodicRestartComputer;
            private set {
                if (_data.IsPeriodicRestartComputer != value) {
                    _data.IsPeriodicRestartComputer = value;
                }
            }
        }
        public int PeriodicRestartComputerHours {
            get => _data.PeriodicRestartComputerHours;
            private set {
                if (_data.PeriodicRestartComputerHours != value) {
                    _data.PeriodicRestartComputerHours = value;
                }
            }
        }

        public bool IsAutoStart {
            get => _data.IsAutoStart;
            private set {
                if (_data.IsAutoStart != value) {
                    _data.IsAutoStart = value;
                }
            }
        }

        public bool IsAutoRestartKernel {
            get {
                return _data.IsAutoRestartKernel;
            }
            private set {
                if (_data.IsAutoRestartKernel != value) {
                    _data.IsAutoRestartKernel = value;
                }
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

        public void SetValue(string propertyName, object value) {
            if (Properties.TryGetValue(propertyName, out PropertyInfo propertyInfo)) {
                if (propertyInfo.CanWrite) {
                    if (propertyInfo.PropertyType == typeof(Guid)) {
                        value = DictionaryExtensions.ConvertToGuid(value);
                    }
                    propertyInfo.SetValue(this, value, null);
                    if (VirtualRoot.IsControlCenter) {
                        Server.ControlCenterService.SetMinerProfilePropertyAsync(_workId, propertyName, value, (response, exception) => {
                            VirtualRoot.Happened(new MinerProfilePropertyChangedEvent(propertyName));
                        });
                    }
                    else {
                        bool isUseJson = _workId != Guid.Empty;
                        IRepository<MinerProfileData> repository = NTMinerRoot.CreateLocalRepository<MinerProfileData>(isUseJson);
                        repository.Update(_data);
                        VirtualRoot.Happened(new MinerProfilePropertyChangedEvent(propertyName));
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
    }
}

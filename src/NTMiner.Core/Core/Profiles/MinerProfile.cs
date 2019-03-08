using LiteDB;
using NTMiner.Core.Gpus;
using NTMiner.Core.Impl;
using NTMiner.Core.Kernels;
using NTMiner.Core.Profiles.Impl;
using NTMiner.MinerServer;
using NTMiner.Profile;
using NTMiner.Repositories;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTMiner.Core.Profiles {
    internal class MinerProfile : IWorkProfile {
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
                    _userSet = new UserSet(_workId != Guid.Empty);
                }
            }
            else {
                _userSet.Refresh();
            }
            if (workId != Guid.Empty) {
                MineWork = Server.ProfileService.GetMineWork(workId);
            }
            _data = null;
            _data = GetMinerProfileData();
            if (_data == null) {
                throw new ValidationException("未获取到MinerProfileData数据，请重试");
            }
        }

        private MinerProfileData GetMinerProfileData() {
            MinerProfileData result = null;
            if (_workId != Guid.Empty) {
                result = Server.ProfileService.GetMinerProfile(_workId);
            }
            else {
                bool isUseJson = _workId != Guid.Empty && VirtualRoot.IsControlCenter;
                IRepository<MinerProfileData> repository = NTMinerRoot.CreateLocalRepository<MinerProfileData>(isUseJson);
                result = repository.GetAll().FirstOrDefault();
            }
            if (result == null) {
                result = MinerProfileData.CreateDefaultData();
            }
            return result;
        }

        [IgnoreReflectionSet]
        public IMineWork MineWork { get; private set; }

        #region IMinerProfile
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
                    if (_workId != Guid.Empty) {
                        if (VirtualRoot.IsControlCenter) {
                            Server.ControlCenterService.SetMinerProfilePropertyAsync(_workId, propertyName, value, (response, exception) => {
                                VirtualRoot.Happened(new MinerProfilePropertyChangedEvent(propertyName));
                            });
                        }
                    }
                    else {
                        bool isUseJson = _workId != Guid.Empty && VirtualRoot.IsControlCenter;
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
        #endregion

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

        #region CoinKernelProfileSet
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
        #endregion

        #region CoinProfileSet
        private class CoinProfileSet {
            private readonly Dictionary<Guid, CoinProfile> _dicById = new Dictionary<Guid, CoinProfile>();
            private readonly INTMinerRoot _root;
            private readonly object _locker = new object();
            private Guid _workId;
            public CoinProfileSet(INTMinerRoot root, Guid workId) {
                _root = root;
                _workId = workId;
            }

            public void Refresh(Guid workId) {
                _workId = workId;
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
                    CoinProfile coinProfile = CoinProfile.Create(_root, _workId, coinId);
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
                public static readonly CoinProfile Empty = new CoinProfile(NTMinerRoot.Current, Guid.Empty);

                public static CoinProfile Create(INTMinerRoot root, Guid workId, Guid coinId) {
                    if (root.CoinSet.TryGetCoin(coinId, out ICoin coin)) {
                        CoinProfile coinProfile = new CoinProfile(root, workId, coin);

                        return coinProfile;
                    }
                    else {
                        return Empty;
                    }
                }

                private readonly INTMinerRoot _root;
                private readonly Guid _workId;
                private CoinProfileData _data;
                private CoinProfile(INTMinerRoot root, Guid workId) {
                    _root = root;
                    _workId = workId;
                }

                private CoinProfileData GetCoinProfileData(Guid coinId) {
                    if (VirtualRoot.IsControlCenter) {
                        return Server.ProfileService.GetCoinProfile(_workId, coinId);
                    }
                    else {
                        bool isUseJson = _workId != Guid.Empty;
                        IRepository<CoinProfileData> repository = NTMinerRoot.CreateLocalRepository<CoinProfileData>(isUseJson);
                        var result = repository.GetByKey(coinId);
                        if (result == null) {
                            result = CoinProfileData.CreateDefaultData(coinId);
                        }
                        return result;
                    }
                }

                private CoinProfile(INTMinerRoot root, Guid workId, ICoin coin) {
                    _root = root;
                    _workId = workId;
                    _data = GetCoinProfileData(coin.GetId());
                    if (_data == null) {
                        throw new ValidationException("未获取到CoinProfileData数据，请重试");
                    }
                }

                [IgnoreReflectionSet]
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
                    private set {
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
                    private set {
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

                public bool IsOverClockEnabled {
                    get { return _data.IsOverClockEnabled; }
                    private set {
                        _data.IsOverClockEnabled = value;
                    }
                }

                public bool IsOverClockGpuAll {
                    get { return _data.IsOverClockGpuAll; }
                    private set {
                        _data.IsOverClockGpuAll = value;
                    }
                }

                private static Dictionary<string, PropertyInfo> s_properties;
                [IgnoreReflectionSet]
                private static Dictionary<string, PropertyInfo> Properties {
                    get {
                        if (s_properties == null) {
                            s_properties = GetPropertiesCanSet<CoinProfile>();
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
                                Server.ControlCenterService.SetCoinProfilePropertyAsync(_workId, CoinId, propertyName, value, (response, exception) => {
                                    VirtualRoot.Happened(new CoinProfilePropertyChangedEvent(this.CoinId, propertyName));
                                });
                            }
                            else {
                                bool isUseJson = _workId != Guid.Empty;
                                IRepository<CoinProfileData> repository = NTMinerRoot.CreateLocalRepository<CoinProfileData>(isUseJson);
                                repository.Update(_data);
                                VirtualRoot.Happened(new CoinProfilePropertyChangedEvent(this.CoinId, propertyName));
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region GpuOverClockDataSet
        public class GpuOverClockDataSet {
            private readonly Dictionary<Guid, GpuProfileData> _dicById = new Dictionary<Guid, GpuProfileData>();

            private readonly INTMinerRoot _root;
            private Guid _workId;

            public GpuOverClockDataSet(INTMinerRoot root, Guid workId) {
                _root = root;
                _workId = workId;
                VirtualRoot.Accept<AddOrUpdateGpuOverClockDataCommand>(
                    "处理添加或更新Gpu超频数据命令",
                    LogEnum.Console,
                    action: message => {
                        IGpu gpu;
                        if (root.GpuSet.TryGetGpu(message.Input.Index, out gpu)) {
                            GpuProfileData data;
                            if (_dicById.ContainsKey(message.Input.GetId())) {
                                data = _dicById[message.Input.GetId()];
                                data.Update(message.Input);
                                using (LiteDatabase db = new LiteDatabase(SpecialPath.LocalDbFileFullName)) {
                                    var col = db.GetCollection<GpuProfileData>();
                                    col.Update(data);
                                }
                            }
                            else {
                                data = new GpuProfileData(message.Input);
                                _dicById.Add(data.Id, data);
                                using (LiteDatabase db = new LiteDatabase(SpecialPath.LocalDbFileFullName)) {
                                    var col = db.GetCollection<GpuProfileData>();
                                    col.Insert(data);
                                }
                            }
                            VirtualRoot.Happened(new GpuOverClockDataAddedOrUpdatedEvent(data));
                        }
                    });
                VirtualRoot.Accept<OverClockCommand>(
                    "处理超频命令",
                    LogEnum.Console,
                    action: message => {
                        IGpu gpu;
                        if (root.GpuSet.TryGetGpu(message.Input.Index, out gpu)) {
                            message.Input.OverClock(gpu.OverClock);
                            gpu.OverClock.RefreshGpuState(message.Input.Index);
                        }
                    });
                VirtualRoot.Accept<CoinOverClockCommand>(
                    "处理币种超频命令",
                    LogEnum.Console,
                    action: message => {
                        ICoinProfile coinProfile = root.MinerProfile.GetCoinProfile(message.CoinId);
                        if (coinProfile.IsOverClockGpuAll) {
                            GpuProfileData overClockData = _dicById.Values.FirstOrDefault(a => a.CoinId == message.CoinId && a.Index == NTMinerRoot.GpuAllId);
                            if (overClockData != null) {
                                VirtualRoot.Execute(new OverClockCommand(overClockData));
                            }
                        }
                        else {
                            foreach (var overClockData in _dicById.Values.Where(a => a.CoinId == message.CoinId)) {
                                if (overClockData.IsEnabled && overClockData.Index != NTMinerRoot.GpuAllId) {
                                    VirtualRoot.Execute(new OverClockCommand(overClockData));
                                }
                            }
                        }
                    });
            }

            private bool _isInited = false;
            private object _locker = new object();

            private void InitOnece() {
                if (_isInited) {
                    return;
                }
                Init();
            }

            private void Init() {
                lock (_locker) {
                    if (!_isInited) {
                        bool isUseJson = _workId != Guid.Empty;
                        GpuProfileData[] datas;
                        if (isUseJson) {
                            datas = LocalJson.Instance.GpuProfiles;
                        }
                        else {
                            using (LiteDatabase db = new LiteDatabase(SpecialPath.LocalDbFileFullName)) {
                                var col = db.GetCollection<GpuProfileData>();
                                datas = col.FindAll().ToArray();
                            }
                        }
                        foreach (var item in datas) {
                            _dicById.Add(item.Id, item);
                        }
                        _isInited = true;
                    }
                }
            }

            public void Refresh(Guid workId) {
                _workId = workId;
                _dicById.Clear();
                _isInited = false;
            }

            public IGpuProfile GetGpuOverClockData(Guid coinId, int index) {
                InitOnece();
                GpuProfileData data = _dicById.Values.FirstOrDefault(a => a.CoinId == coinId && a.Index == index);
                if (data == null) {
                    return new GpuProfileData(Guid.NewGuid(), coinId, index);
                }
                return data;
            }

            public IEnumerable<IGpuProfile> GetGpuOverClocks() {
                InitOnece();
                return _dicById.Values;
            }
        }
        #endregion

        #region PoolProfileSet
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
                        return Server.ProfileService.GetPoolProfile(_workId, poolId);
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
        #endregion

        #region WalletSet
        private class WalletSet {
            private readonly INTMinerRoot _root;
            private readonly Dictionary<Guid, WalletData> _dicById = new Dictionary<Guid, WalletData>();

            private Guid _workId;
            public WalletSet(INTMinerRoot root, Guid workId) {
                _root = root;
                _workId = workId;
                VirtualRoot.Accept<AddWalletCommand>(
                    "添加钱包",
                    LogEnum.Console,
                    action: message => {
                        InitOnece();
                        if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                            throw new ArgumentNullException();
                        }
                        if (!_root.CoinSet.Contains(message.Input.CoinId)) {
                            throw new ValidationException("there is not coin with id " + message.Input.CoinId);
                        }
                        if (string.IsNullOrEmpty(message.Input.Address)) {
                            throw new ValidationException("wallet code and Address can't be null or empty");
                        }
                        if (_dicById.ContainsKey(message.Input.GetId())) {
                            return;
                        }
                        WalletData entity = new WalletData().Update(message.Input);
                        _dicById.Add(entity.Id, entity);
                        AddWallet(entity);

                        VirtualRoot.Happened(new WalletAddedEvent(entity));
                    });
                VirtualRoot.Accept<UpdateWalletCommand>(
                    "更新钱包",
                    LogEnum.Console,
                    action: message => {
                        InitOnece();
                        if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                            throw new ArgumentNullException();
                        }
                        if (!_root.CoinSet.Contains(message.Input.CoinId)) {
                            throw new ValidationException("there is not coin with id " + message.Input.CoinId);
                        }
                        if (string.IsNullOrEmpty(message.Input.Address)) {
                            throw new ValidationException("wallet Address can't be null or empty");
                        }
                        if (string.IsNullOrEmpty(message.Input.Name)) {
                            throw new ValidationException("wallet name can't be null or empty");
                        }
                        if (!_dicById.ContainsKey(message.Input.GetId())) {
                            return;
                        }
                        WalletData entity = _dicById[message.Input.GetId()];
                        entity.Update(message.Input);
                        UpdateWallet(entity);

                        VirtualRoot.Happened(new WalletUpdatedEvent(entity));
                    });
                VirtualRoot.Accept<RemoveWalletCommand>(
                    "移除钱包",
                    LogEnum.Console,
                    action: (message) => {
                        InitOnece();
                        if (message == null || message.EntityId == Guid.Empty) {
                            throw new ArgumentNullException();
                        }
                        if (!_dicById.ContainsKey(message.EntityId)) {
                            return;
                        }
                        WalletData entity = _dicById[message.EntityId];
                        _dicById.Remove(entity.GetId());
                        RemoveWallet(entity.Id);

                        VirtualRoot.Happened(new WalletRemovedEvent(entity));
                    });
            }

            private void AddWallet(WalletData entity) {
                if (VirtualRoot.IsControlCenter) {
                    Server.ControlCenterService.AddOrUpdateWalletAsync(entity, null);
                }
                else {
                    bool isUseJson = _workId != Guid.Empty;
                    var repository = NTMinerRoot.CreateLocalRepository<WalletData>(isUseJson);
                    repository.Add(entity);
                }
            }

            private void UpdateWallet(WalletData entity) {
                if (VirtualRoot.IsControlCenter) {
                    Server.ControlCenterService.AddOrUpdateWalletAsync(entity, null);
                }
                else {
                    bool isUseJson = _workId != Guid.Empty;
                    var repository = NTMinerRoot.CreateLocalRepository<WalletData>(isUseJson);
                    repository.Update(entity);
                }
            }

            private void RemoveWallet(Guid id) {
                if (VirtualRoot.IsControlCenter) {
                    Server.ControlCenterService.RemoveWalletAsync(id, null);
                }
                else {
                    bool isUseJson = _workId != Guid.Empty;
                    var repository = NTMinerRoot.CreateLocalRepository<WalletData>(isUseJson);
                    repository.Remove(id);
                }
            }

            public void Refresh(Guid workId) {
                _workId = workId;
                _dicById.Clear();
                _isInited = false;
            }

            private bool _isInited = false;
            private object _locker = new object();

            private void InitOnece() {
                if (_isInited) {
                    return;
                }
                Init();
            }

            private void Init() {
                if (!_isInited) {
                    if (VirtualRoot.IsControlCenter) {
                        lock (_locker) {
                            if (!_isInited) {
                                var response = Server.ControlCenterService.GetWallets();
                                if (response != null) {
                                    foreach (var item in response.Data) {
                                        if (!_dicById.ContainsKey(item.Id)) {
                                            _dicById.Add(item.Id, item);
                                        }
                                    }
                                }
                                _isInited = true;
                            }
                        }
                    }
                    else {
                        bool isUseJson = _workId != Guid.Empty;
                        var repository = NTMinerRoot.CreateLocalRepository<WalletData>(isUseJson);
                        lock (_locker) {
                            if (!_isInited) {
                                foreach (var item in repository.GetAll()) {
                                    if (!_dicById.ContainsKey(item.Id)) {
                                        _dicById.Add(item.Id, item);
                                    }
                                }
                                _isInited = true;
                            }
                        }
                    }
                }
            }

            public bool TryGetWallet(Guid walletId, out IWallet wallet) {
                InitOnece();
                WalletData wlt;
                bool r = _dicById.TryGetValue(walletId, out wlt);
                wallet = wlt;
                return r;
            }

            public IEnumerable<IWallet> GetAllWallets() {
                InitOnece();
                return _dicById.Values;
            }
        }
        #endregion
    }
}

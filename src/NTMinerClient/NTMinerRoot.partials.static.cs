using NTMiner.Core;
using NTMiner.JsonDb;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using NTMiner.Profile;
using NTMiner.Repositories;
using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NTMiner {
    public partial class NTMinerRoot {
        static NTMinerRoot() {
            ServerVersion = EntryAssemblyInfo.CurrentVersion;
        }

        public const int SpeedHistoryLengthByMinute = 10;
        public const int GpuAllId = -1;
        private static readonly NTMinerRoot S_Instance = new NTMinerRoot();
        public static readonly INTMinerRoot Instance = S_Instance;

        public static Version ServerVersion;
        private static bool _isJsonServer;
        public static bool IsJsonServer {
            get { return _isJsonServer; }
            private set { _isJsonServer = value; }
        }

        private static bool _isJsonLocal;
        public static bool IsJsonLocal {
            get { return _isJsonLocal; }
            private set { _isJsonLocal = value; }
        }

        #region 电脑硬件
        private static Computer _computer = null;
        private static bool _isComputerFirst = true;
        private static readonly object _computerLocker = new object();
        public static Computer Computer {
            get {
                if (_isComputerFirst) {
                    lock (_computerLocker) {
                        if (_isComputerFirst) {
                            _isComputerFirst = false;
                            _computer = new Computer();
                            _computer.Open();
                            _computer.CPUEnabled = true;
                        }
                    }
                }
                return _computer;
            }
        }
        #endregion

        public static string ThisPcName {
            get {
                string value = Environment.MachineName;
                value = new string(value.ToCharArray().Where(a => !NTKeyword.InvalidMinerNameChars.Contains(a)).ToArray());
                return value;
            }
        }

        public static Action RefreshArgsAssembly { get; private set; } = () => { };
        public static void SetRefreshArgsAssembly(Action action) {
            RefreshArgsAssembly = action;
        }
        private static bool _isUiVisible = false;
        public static bool IsUiVisible {
            get { return _isUiVisible; }
            set {
                _isUiVisible = value;
                if (value) {
                    MainWindowRendedOn = DateTime.Now;
                }
            }
        }
        private static DateTime _mainWindowRendedOn = DateTime.MinValue;
        public static DateTime MainWindowRendedOn {
            get { return _mainWindowRendedOn; }
            private set {
                _mainWindowRendedOn = value;
            }
        }

        public static bool IsUseDevConsole = false;
        public static int OSVirtualMemoryMb;

        public static bool IsAutoStartCanceled = false;

        public static bool IsKernelBrand {
            get {
                return KernelBrandId != Guid.Empty;
            }
        }

        private static Guid? kernelBrandId = null;
        public static Guid KernelBrandId {
            get {
                if (!kernelBrandId.HasValue) {
                    kernelBrandId = VirtualRoot.GetBrandId(VirtualRoot.AppFileFullName, NTKeyword.KernelBrandId);
                }
                return kernelBrandId.Value;
            }
        }

        public static bool IsPoolBrand {
            get {
                return PoolBrandId != Guid.Empty;
            }
        }

        private static Guid? poolBrandId = null;
        public static Guid PoolBrandId {
            get {
                if (!poolBrandId.HasValue) {
                    poolBrandId = VirtualRoot.GetBrandId(VirtualRoot.AppFileFullName, NTKeyword.PoolBrandId);
                }
                return poolBrandId.Value;
            }
        }

        public static bool IsBrandSpecified {
            get {
                return KernelBrandId != Guid.Empty || PoolBrandId != Guid.Empty;
            }
        }

        #region 枚举数据集
        public static IEnumerable<EnumItem<LocalMessageChannel>> LocalMessageChannelEnumItems {
            get { 
                return EnumItem<LocalMessageChannel>.GetEnumItems(); 
            }
        }

        public static IEnumerable<EnumItem<ConsoleColor>> ConsoleColorEnumItems {
            get {
                return EnumItem<ConsoleColor>.GetEnumItems();
            }
        }

        public static IEnumerable<EnumItem<SupportedGpu>> SupportedGpuEnumItems {
            get {
                return EnumItem<SupportedGpu>.GetEnumItems();
            }
        }

        public static IEnumerable<EnumItem<GpuType>> GpuTypeEnumItems {
            get {
                return EnumItem<GpuType>.GetEnumItems();
            }
        }

        public static IEnumerable<EnumItem<PublishStatus>> PublishStatusEnumItems {
            get {
                return EnumItem<PublishStatus>.GetEnumItems();
            }
        }

        public static IEnumerable<EnumItem<MineStatus>> MineStatusEnumItems {
            get {
                return EnumItem<MineStatus>.GetEnumItems();
            }
        }

        public static IEnumerable<EnumItem<ServerMessageType>> ServerMessageTypeEnumItems {
            get {
                return EnumItem<ServerMessageType>.GetEnumItems();
            }
        }

        public static IEnumerable<EnumItem<LocalMessageType>> LocalMessageTypeEnumItems {
            get {
                return EnumItem<LocalMessageType>.GetEnumItems();
            }
        }
        #endregion

        private static LocalJsonDb _localJson;
        public static ILocalJsonDb LocalJson {
            get {
                LocalJsonInit();
                return _localJson;
            }
        }

        public static void ReInitLocalJson() {
            _localJsonInited = false;
            LocalJsonInit();
        }

        #region LocalJsonInit
        private static readonly object _localJsonlocker = new object();
        private static bool _localJsonInited = false;
        // 从磁盘读取local.json反序列化为LocalJson对象
        private static void LocalJsonInit() {
            if (!_localJsonInited) {
                lock (_localJsonlocker) {
                    if (!_localJsonInited) {
                        string localJson = SpecialPath.ReadLocalJsonFile();
                        if (!string.IsNullOrEmpty(localJson)) {
                            try {
                                LocalJsonDb data = VirtualRoot.JsonSerializer.Deserialize<LocalJsonDb>(localJson);
                                _localJson = data ?? new LocalJsonDb();
                            }
                            catch (Exception e) {
                                Logger.ErrorDebugLine(e);
                            }
                        }
                        else {
                            _localJson = new LocalJsonDb();
                        }
                        // 因为是群控作业，将开机启动和自动挖矿设置为true
                        var repository = new LiteDbReadWriteRepository<MinerProfileData>(VirtualRoot.LocalDbFileFullName);
                        MinerProfileData localProfile = repository.GetByKey(MinerProfileData.DefaultId);
                        if (localProfile != null) {
                            if (localProfile.IsAutoStart == false || localProfile.IsAutoBoot == false) {
                                localProfile.IsAutoBoot = true;
                                localProfile.IsAutoStart = true;
                                repository.Update(localProfile);
                            }
                        }
                        _localJson.MinerProfile.IsAutoBoot = true;
                        _localJson.MinerProfile.IsAutoStart = true;
                        // 这里的逻辑是，当用户在主界面填写矿工名时，矿工名会被交换到注册表从而当用户使用群控但没有填写群控矿工名时作为缺省矿工名
                        // 但是旧版本的挖矿端并没有把矿工名交换到注册表去所以当注册表中没有矿工名时需读取local.litedb中的矿工名
                        if (string.IsNullOrEmpty(_localJson.MinerProfile.MinerName)) {
                            _localJson.MinerProfile.MinerName = NTMinerRegistry.GetMinerName();
                            if (string.IsNullOrEmpty(_localJson.MinerProfile.MinerName)) {
                                if (localProfile != null) {
                                    _localJson.MinerProfile.MinerName = localProfile.MinerName;
                                }
                            }
                        }
                        _localJsonInited = true;
                    }
                }
            }
        }
        #endregion

        private static ServerJsonDb _serverJson;
        public static IServerJsonDb ServerJson {
            get {
                ServerJsonInit();
                return _serverJson;
            }
        }

        public static void ReInitServerJson() {
            _serverJsonInited = false;
        }

        #region ServerJsonInit
        private static readonly object _serverJsonlocker = new object();
        private static bool _serverJsonInited = false;
        // 从磁盘读取server.json反序列化为ServerJson对象
        private static void ServerJsonInit() {
            if (!_serverJsonInited) {
                lock (_serverJsonlocker) {
                    if (!_serverJsonInited) {
                        string serverJson = SpecialPath.ReadServerJsonFile();
                        if (!string.IsNullOrEmpty(serverJson)) {
                            try {
                                ServerJsonDb data = VirtualRoot.JsonSerializer.Deserialize<ServerJsonDb>(serverJson);
                                _serverJson = data;
                                if (KernelBrandId != Guid.Empty) {
                                    var kernelToRemoves = data.Kernels.Where(a => a.BrandId != KernelBrandId).ToArray();
                                    foreach (var item in kernelToRemoves) {
                                        data.Kernels.Remove(item);
                                    }
                                    var coinKernelToRemoves = data.CoinKernels.Where(a => kernelToRemoves.Any(b => b.Id == a.KernelId)).ToArray();
                                    foreach (var item in coinKernelToRemoves) {
                                        data.CoinKernels.Remove(item);
                                    }
                                    var poolKernelToRemoves = data.PoolKernels.Where(a => kernelToRemoves.Any(b => b.Id == a.KernelId)).ToArray();
                                    foreach (var item in poolKernelToRemoves) {
                                        data.PoolKernels.Remove(item);
                                    }
                                }
                                if (PoolBrandId != Guid.Empty) {
                                    var poolToRemoves = data.Pools.Where(a => a.BrandId != PoolBrandId && data.Pools.Any(b => b.CoinId == a.CoinId && b.BrandId == poolBrandId)).ToArray();
                                    foreach (var item in poolToRemoves) {
                                        data.Pools.Remove(item);
                                    }
                                    var poolKernelToRemoves = data.PoolKernels.Where(a => poolToRemoves.Any(b => b.Id == a.PoolId)).ToArray();
                                    foreach (var item in poolKernelToRemoves) {
                                        data.PoolKernels.Remove(item);
                                    }
                                }
                            }
                            catch (Exception e) {
                                Logger.ErrorDebugLine(e);
                            }
                        }
                        else {
                            _serverJson = new ServerJsonDb();
                        }
                        _serverJsonInited = true;
                    }
                }
            }
        }
        #endregion

        #region ExportJson
        /// <summary>
        /// 将当前的系统状态导出到给定的json文件
        /// </summary>
        /// <returns></returns>
        public static void ExportServerVersionJson(string jsonFileFullName) {
            ServerJsonDb serverJsonObj = new ServerJsonDb(Instance);
            serverJsonObj.CutJsonSize();
            string json = VirtualRoot.JsonSerializer.Serialize(serverJsonObj);
            serverJsonObj.UnCut();
            File.WriteAllText(jsonFileFullName, json);
        }

        public static void ExportWorkJson(MineWorkData mineWorkData, out string localJson, out string serverJson) {
            localJson = string.Empty;
            serverJson = string.Empty;
            try {
                LocalJsonDb localJsonObj = new LocalJsonDb(Instance, mineWorkData);
                ServerJsonDb serverJsonObj = new ServerJsonDb(Instance, localJsonObj);
                localJson = VirtualRoot.JsonSerializer.Serialize(localJsonObj);
                serverJson = VirtualRoot.JsonSerializer.Serialize(serverJsonObj);
                mineWorkData.ServerJsonSha1 = HashUtil.Sha1(serverJson);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
        #endregion

        #region CreateRepository
        /// <summary>
        /// 创建组合仓储，组合仓储由ServerDb和ProfileDb层序组成。
        /// 如果是开发者则访问ServerDb且只访问GlobalDb，否则将ServerDb和ProfileDb并起来访问且不能修改删除GlobalDb。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRepository<T> CreateCompositeRepository<T>() where T : class, ILevelEntity<Guid> {
            return new HierarchicalRepository<T>(CreateServerRepository<T>(), CreateLocalRepository<T>());
        }

        public static IRepository<T> CreateLocalRepository<T>() where T : class, IDbEntity<Guid> {
            if (!IsJsonLocal) {
                return new LiteDbReadWriteRepository<T>(VirtualRoot.LocalDbFileFullName);
            }
            else {
                return new JsonReadOnlyRepository<T>(LocalJson);
            }
        }

        public static IRepository<T> CreateServerRepository<T>() where T : class, IDbEntity<Guid> {
            if (!IsJsonServer) {
                return new LiteDbReadWriteRepository<T>(VirtualRoot.ServerDbFileFullName);
            }
            else {
                return new JsonReadOnlyRepository<T>(ServerJson);
            }
        }
        #endregion

        #region DiskSpace
        private static DateTime _diskSpaceOn = DateTime.MinValue;
        private static string _diskSpace = string.Empty;
        public static string DiskSpace {
            get {
                if (_diskSpaceOn.AddMinutes(20) < DateTime.Now) {
                    _diskSpaceOn = DateTime.Now;
                    StringBuilder sb = new StringBuilder();
                    int len = sb.Length;
                    foreach (var item in DriveInfo.GetDrives().Where(a => a.DriveType == DriveType.Fixed)) {
                        if (len != sb.Length) {
                            sb.Append(";");
                        }
                        // item.Name like C:\
                        sb.Append(item.Name).Append((item.AvailableFreeSpace / (double)(1024 * 1024 * 1024)).ToString("f1")).Append(" Gb");
                    }
                    _diskSpace = sb.ToString();
                }

                return _diskSpace;
            }
        }
        #endregion
    }
}

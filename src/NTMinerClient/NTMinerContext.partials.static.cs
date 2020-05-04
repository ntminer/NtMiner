using NTMiner.Core;
using NTMiner.Core.MinerServer;
using NTMiner.Core.MinerStudio;
using NTMiner.Core.MinerStudio.Impl;
using NTMiner.Core.Profile;
using NTMiner.JsonDb;
using NTMiner.Repositories;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NTMiner {
    public partial class NTMinerContext {
        static NTMinerContext() {
            ServerVersion = EntryAssemblyInfo.CurrentVersion;
        }

        public const int SpeedHistoryLengthByMinute = 10;
        public const int GpuAllId = -1;
        private static readonly NTMinerContext S_Instance = new NTMinerContext();
        public static readonly INTMinerContext Instance = S_Instance;

        private static WorkType _workType;
        private static string _workerName;

        public static WorkType WorkType {
            get { return _workType; }
        }

        public static IMinerStudioContext MinerStudioContext { get; private set; } = new MinerStudioContext();

        private static Guid _id = NTMinerRegistry.GetClientId(ClientAppType.AppType);
        public static Guid Id {
            get {
                return _id;
            }
        }

        public static Version ServerVersion;
        /// <summary>
        /// 表示是否是使用server.json只读数据库文件。
        /// 只有DevMode模式的挖矿端才会返回False，否则都是True。
        /// </summary>
        public static bool IsJsonServer {
            get {
                if (ClientAppType.IsMinerClient && DevMode.IsDevMode) {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 是否是使用local.json作为数据库而不是local.litedb。
        /// 有两种情况该属性会被赋值为true：1. 群控客户端；2. 作业模式的挖矿端。
        /// </summary>
        public static bool IsJsonLocal {
            get { return ClientAppType.IsMinerStudio || _workType != WorkType.None; }
        }

        public static string ThisPcName {
            get {
                return Environment.MachineName;
            }
        }

        public static Action<string> RefreshArgsAssembly { get; private set; } = (reason) => { };
        public static void SetRefreshArgsAssembly(Action<string> action) {
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
                    lock (_locker) {
                        if (!kernelBrandId.HasValue) {
                            kernelBrandId = VirtualRoot.GetBrandId(VirtualRoot.AppFileFullName, NTKeyword.KernelBrandId);
                        }
                    }
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
                    lock (_locker) {
                        if (!poolBrandId.HasValue) {
                            poolBrandId = VirtualRoot.GetBrandId(VirtualRoot.AppFileFullName, NTKeyword.PoolBrandId);
                        }
                    }
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

        public static IEnumerable<EnumItem<UserStatus>> UserStatusEnumItems {
            get {
                return EnumItem<UserStatus>.GetEnumItems();
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

        private static LocalJsonDb _localJsonDb;
        public static ILocalJsonDb LocalJsonDb {
            get {
                LocalJsonInit();
                return _localJsonDb;
            }
        }

        public static void ReInitLocalJson() {
            _localJsonInited = false;
        }

        #region LocalJsonInit
        private static bool _localJsonInited = false;
        // 从磁盘读取local.json反序列化为LocalJson对象
        private static void LocalJsonInit() {
            if (!_localJsonInited) {
                lock (_locker) {
                    if (!_localJsonInited) {
                        string localJson = HomePath.ReadLocalJsonFile(_workType);
                        LocalJsonDb localJsonDb = null;
                        if (!string.IsNullOrEmpty(localJson)) {
                            localJsonDb = VirtualRoot.JsonSerializer.Deserialize<LocalJsonDb>(localJson);
                        }
                        if (localJsonDb == null) {
                            if (ClientAppType.IsMinerClient) {
                                localJsonDb = JsonDb.LocalJsonDb.ConvertFromNTMinerContext();
                                VirtualRoot.ThisLocalWarn(nameof(NTMinerContext), "当前作业由本机数据自动生成，因为本机没有作业记录，请先在群控端创建或编辑作业。", OutEnum.Warn, toConsole: true);
                            }
                            else {
                                localJsonDb = new LocalJsonDb();
                            }
                        }
                        _localJsonDb = localJsonDb;

                        #region 因为是群控作业，将开机启动和自动挖矿设置为true
                        var repository = new LiteDbReadWriteRepository<MinerProfileData>(HomePath.LocalDbFileFullName);
                        MinerProfileData localProfile = repository.GetByKey(MinerProfileData.DefaultId);
                        if (localProfile != null) {
                            MinerProfileData.CopyWorkIgnoreValues(localProfile, _localJsonDb.MinerProfile);
                            // 如果是作业模式则必须设置为开机自动重启
                            if (localProfile.IsAutoStart == false || localProfile.IsAutoBoot == false) {
                                localProfile.IsAutoBoot = true;
                                localProfile.IsAutoStart = true;
                                repository.Update(localProfile);
                            }
                        }
                        _localJsonDb.MinerProfile.IsAutoBoot = true;
                        _localJsonDb.MinerProfile.IsAutoStart = true;
                        #endregion

                        #region 矿机名
                        if (!string.IsNullOrEmpty(_workerName)) {
                            _localJsonDb.MinerProfile.MinerName = _workerName;
                        }
                        else {
                            // 当用户使用群控作业但没有指定群控矿机名时使用从local.litedb中读取的矿工名
                            if (string.IsNullOrEmpty(_localJsonDb.MinerProfile.MinerName)) {
                                if (localProfile != null) {
                                    _localJsonDb.MinerProfile.MinerName = localProfile.MinerName;
                                }
                                // 如果local.litedb中也没有矿机名则使用去除了特殊符号的本机机器名作为矿机名
                                if (string.IsNullOrEmpty(_localJsonDb.MinerProfile.MinerName)) {
                                    string minerName = Environment.MachineName;
                                    minerName = new string(minerName.ToCharArray().Where(a => !NTKeyword.InvalidMinerNameChars.Contains(a)).ToArray());
                                    _localJsonDb.MinerProfile.MinerName = minerName;
                                }
                            }
                        }
                        #endregion
                        _localJsonInited = true;
                    }
                }
            }
        }
        #endregion

        private static ServerJsonDb _serverJsonDb;
        public static IServerJsonDb ServerJsonDb {
            get {
                ServerJsonInit();
                return _serverJsonDb;
            }
        }

        public static void ReInitServerJson() {
            _serverJsonInited = false;
        }

        #region ServerJsonInit
        private static bool _serverJsonInited = false;
        // 从磁盘读取server.json反序列化为ServerJson对象
        private static void ServerJsonInit() {
            if (!_serverJsonInited) {
                lock (_locker) {
                    if (!_serverJsonInited) {
                        string serverJson;
                        switch (_workType) {
                            case WorkType.SelfWork:
                                serverJson = HomePath.ReadSelfWorkServerJsonFile();
                                break;
                            case WorkType.MineWork:
                                serverJson = HomePath.ReadMineWorkServerJsonFile();
                                break;
                            default:
                                serverJson = HomePath.ReadServerJsonFile();
                                break;
                        }
                        if (!string.IsNullOrEmpty(serverJson)) {
                            ServerJsonDb data = VirtualRoot.JsonSerializer.Deserialize<ServerJsonDb>(serverJson) ?? new ServerJsonDb();
                            try {
                                _serverJsonDb = data;
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
                            _serverJsonDb = new ServerJsonDb();
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
    }
}

using Microsoft.Win32;
using NTMiner.Core;
using NTMiner.Core.Impl;
using NTMiner.Core.Kernels;
using NTMiner.JsonDb;
using NTMiner.MinerServer;
using NTMiner.Profile;
using NTMiner.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NTMiner {
    public partial class NTMinerRoot {
        public static IKernelDownloader KernelDownloader;
        public static Action RefreshArgsAssembly = () => { };
        public static Guid KernelBrandId;
        public static byte[] KernelBrandRaw = new byte[0];

        public static Func<System.Windows.Forms.Keys, bool> RegHotKey;
        public static string AppName;
        public static bool IsUseDevConsole = false;
        // ReSharper disable once InconsistentNaming
        public static int OSVirtualMemoryMb;
        public static string UserKernelCommandLine;

        public static readonly int GpuAllId = -1;

        static NTMinerRoot() {
            Assembly mainAssembly = Assembly.GetEntryAssembly();
            CurrentVersion = mainAssembly.GetName().Version;
            CurrentVersionTag = ((AssemblyDescriptionAttribute)mainAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), inherit: false).First()).Description;
        }

        private static readonly NTMinerRoot SCurrent = new NTMinerRoot();
        public static readonly INTMinerRoot Current = SCurrent;
        public static readonly Version CurrentVersion;
        public static readonly string CurrentVersionTag;
        private static string _sJsonFileVersion;
        public static string JsonFileVersion {
            get { return _sJsonFileVersion; }
            set {
                if (_sJsonFileVersion != value) {
                    string oldVersion = _sJsonFileVersion;
                    _sJsonFileVersion = value;
                    VirtualRoot.Happened(new ServerJsonVersionChangedEvent(oldVersion, value));
                }
            }
        }

        private static LocalJsonDb _localJson;
        public static ILocalJsonDb LocalJson {
            get {
                LocalJsonInit();
                return _localJson;
            }
        }

        public static void ReInitLocalJson(MineWorkData mineWorkData = null) {
            _localJsonInited = false;
            if (mineWorkData != null) {
                LocalJsonInit();
                _localJson.MineWork = mineWorkData;
            }
        }

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
                                _localJson = data;
                            }
                            catch (Exception e) {
                                Logger.ErrorDebugLine(e.Message, e);
                            }
                        }
                        else {
                            _localJson = new LocalJsonDb();
                        }
                        _localJsonInited = true;
                    }
                }
            }
        }

        private static ServerJsonDb _serverJson;
        private static IServerJsonDb ServerJson {
            get {
                ServerJsonInit();
                return _serverJson;
            }
        }

        public static void ReInitServerJson() {
            _serverJsonInited = false;
        }

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
                                    var kernelToRemoves = data.Kernels.Where(a => a.BrandId != NTMinerRoot.KernelBrandId).ToArray();
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
                            }
                            catch (Exception e) {
                                Logger.ErrorDebugLine(e.Message, e);
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

        // 将当前的系统状态导出为serverVersion.json
        public static string ExportServerVersionJson() {
            var root = Current;
            ServerJsonDb serverJsonObj = new ServerJsonDb {
                Coins = root.CoinSet.Cast<CoinData>().ToArray(),
                Groups = root.GroupSet.Cast<GroupData>().ToArray(),
                CoinGroups = root.CoinGroupSet.Cast<CoinGroupData>().ToArray(),
                KernelInputs = root.KernelInputSet.Cast<KernelInputData>().ToArray(),
                KernelOutputs = root.KernelOutputSet.Cast<KernelOutputData>().ToArray(),
                KernelOutputFilters = root.KernelOutputFilterSet.Cast<KernelOutputFilterData>().ToArray(),
                KernelOutputTranslaters = root.KernelOutputTranslaterSet.Cast<KernelOutputTranslaterData>().ToArray(),
                Kernels = root.KernelSet.Cast<KernelData>().ToList(),
                CoinKernels = root.CoinKernelSet.Cast<CoinKernelData>().ToList(),
                PoolKernels = root.PoolKernelSet.Cast<PoolKernelData>().Where(a => !string.IsNullOrEmpty(a.Args)).ToList(),
                Pools = root.PoolSet.Cast<PoolData>().ToArray(),
                SysDicItems = root.SysDicItemSet.Cast<SysDicItemData>().ToArray(),
                SysDics = root.SysDicSet.Cast<SysDicData>().ToArray()
            };
            string json = VirtualRoot.JsonSerializer.Serialize(serverJsonObj);
            File.WriteAllText(AssemblyInfo.ServerVersionJsonFileFullName, json);
            return Path.GetFileName(AssemblyInfo.ServerVersionJsonFileFullName);
        }

        public static void ExportWorkJson(MineWorkData mineWorkData, out string localJson, out string serverJson) {
            localJson = string.Empty;
            serverJson = string.Empty;
            try {
                var root = Current;
                var minerProfile = root.MinerProfile;
                CoinProfileData mainCoinProfile = new CoinProfileData(minerProfile.GetCoinProfile(minerProfile.CoinId));
                List<CoinProfileData> coinProfiles = new List<CoinProfileData> { mainCoinProfile };
                List<PoolProfileData> poolProfiles = new List<PoolProfileData>();
                CoinKernelProfileData coinKernelProfile = new CoinKernelProfileData(minerProfile.GetCoinKernelProfile(mainCoinProfile.CoinKernelId));
                PoolProfileData mainCoinPoolProfile = new PoolProfileData(minerProfile.GetPoolProfile(mainCoinProfile.PoolId));
                poolProfiles.Add(mainCoinPoolProfile);
                if (coinKernelProfile.IsDualCoinEnabled) {
                    CoinProfileData dualCoinProfile = new CoinProfileData(minerProfile.GetCoinProfile(coinKernelProfile.DualCoinId));
                    coinProfiles.Add(dualCoinProfile);
                    PoolProfileData dualCoinPoolProfile = new PoolProfileData(minerProfile.GetPoolProfile(dualCoinProfile.DualCoinPoolId));
                    poolProfiles.Add(dualCoinPoolProfile);
                }
                LocalJsonDb localJsonObj = new LocalJsonDb {
                    MinerProfile = new MinerProfileData(minerProfile) {
                        MinerName = "{{MinerName}}"
                    },
                    MineWork = mineWorkData,
                    CoinProfiles = coinProfiles.ToArray(),
                    CoinKernelProfiles = new CoinKernelProfileData[] { coinKernelProfile },
                    PoolProfiles = poolProfiles.ToArray(),
                    TimeStamp = Timestamp.GetTimestamp(),
                    Pools = root.PoolSet.Where(a => poolProfiles.Any(b => b.PoolId == a.GetId())).Select(a => new PoolData(a)).ToArray(),
                    Wallets = minerProfile.GetWallets().Select(a => new WalletData(a)).ToArray()
                };
                localJson = VirtualRoot.JsonSerializer.Serialize(localJsonObj);
                root.CoinKernelSet.TryGetCoinKernel(coinKernelProfile.CoinKernelId, out ICoinKernel coinKernel);
                root.KernelSet.TryGetKernel(coinKernel.KernelId, out IKernel kernel);
                var coins = root.CoinSet.Cast<CoinData>().Where(a => localJsonObj.CoinProfiles.Any(b => b.CoinId == a.Id)).ToArray();
                var coinGroups = root.CoinGroupSet.Cast<CoinGroupData>().Where(a => coins.Any(b => b.Id == a.CoinId)).ToArray();
                var pools = root.PoolSet.Cast<PoolData>().Where(a => localJsonObj.PoolProfiles.Any(b => b.PoolId == a.Id)).ToArray();
                ServerJsonDb serverJsonObj = new ServerJsonDb {
                    Coins = coins,
                    CoinGroups = coinGroups,
                    Pools = pools,
                    TimeStamp = Timestamp.GetTimestamp(),
                    Groups = root.GroupSet.Cast<GroupData>().Where(a => coinGroups.Any(b => b.GroupId == a.Id)).ToArray(),
                    KernelInputs = root.KernelInputSet.Cast<KernelInputData>().Where(a => a.Id == kernel.KernelInputId).ToArray(),
                    KernelOutputs = root.KernelOutputSet.Cast<KernelOutputData>().Where(a => a.Id == kernel.KernelOutputId).ToArray(),
                    KernelOutputFilters = root.KernelOutputFilterSet.Cast<KernelOutputFilterData>().Where(a => a.KernelOutputId == kernel.KernelOutputId).ToArray(),
                    KernelOutputTranslaters = root.KernelOutputTranslaterSet.Cast<KernelOutputTranslaterData>().Where(a => a.KernelOutputId == kernel.KernelOutputId).ToArray(),
                    Kernels = new List<KernelData> { (KernelData)kernel },
                    CoinKernels = root.CoinKernelSet.Cast<CoinKernelData>().Where(a => localJsonObj.CoinKernelProfiles.Any(b => b.CoinKernelId == a.Id)).ToList(),
                    PoolKernels = root.PoolKernelSet.Cast<PoolKernelData>().Where(a => !string.IsNullOrEmpty(a.Args) && pools.Any(b => b.Id == a.PoolId)).ToList(),
                    SysDicItems = root.SysDicItemSet.Cast<SysDicItemData>().ToArray(),
                    SysDics = root.SysDicSet.Cast<SysDicData>().ToArray()
                };
                serverJson = VirtualRoot.JsonSerializer.Serialize(serverJsonObj);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        public static IRepository<T> CreateLocalRepository<T>(bool isUseJson) where T : class, IDbEntity<Guid> {
            if (!isUseJson) {
                return new CommonRepository<T>(SpecialPath.LocalDbFileFullName);
            }
            else {
                return new ReadOnlyRepository<T>(LocalJson);
            }
        }

        public static IRepository<T> CreateServerRepository<T>(bool isUseJson) where T : class, IDbEntity<Guid> {
            if (!isUseJson) {
                return new CommonRepository<T>(SpecialPath.ServerDbFileFullName);
            }
            else {
                return new ReadOnlyRepository<T>(ServerJson);
            }
        }

        public static string GetThisPcName() {
            string value = Environment.MachineName.ToLower();
            value = new string(value.ToCharArray().Where(a => !MinerNameConst.InvalidChars.Contains(a)).ToArray());
            return value;
        }

        private static DateTime _diskSpaceOn = DateTime.MinValue;
        private static string _diskSpace = string.Empty;
        public static string DiskSpace {
            get {
                if (_diskSpaceOn.AddMinutes(10) < DateTime.Now) {
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

        #region HotKey
        public static string GetHotKey() {
            object value = Windows.Registry.GetValue(Registry.Users, NTMinerRegistry.NTMinerRegistrySubKey, "HotKey");
            if (value == null) {
                return "X";
            }
            return value.ToString();
        }

        public static bool SetHotKey(string value) {
            if (RegHotKey == null) {
                return false;
            }
            if (string.IsNullOrEmpty(value)) {
                return false;
            }
            if (Enum.TryParse(value, out System.Windows.Forms.Keys key) && key >= System.Windows.Forms.Keys.A && key <= System.Windows.Forms.Keys.Z) {
                if (RegHotKey.Invoke(key)) {
                    Windows.Registry.SetValue(Registry.Users, NTMinerRegistry.NTMinerRegistrySubKey, "HotKey", value);
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region IsShowCommandLine
        public static bool GetIsShowCommandLine() {
            object isAutoBootValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistry.NTMinerRegistrySubKey, "IsShowCommandLine");
            return isAutoBootValue != null && isAutoBootValue.ToString() == "True";
        }

        public static void SetIsShowCommandLine(bool value) {
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistry.NTMinerRegistrySubKey, "IsShowCommandLine", value);
        }
        #endregion
    }
}

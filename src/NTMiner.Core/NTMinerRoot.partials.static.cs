using Microsoft.Win32;
using NTMiner.Core.Impl;
using NTMiner.JsonDb;
using NTMiner.MinerServer;
using NTMiner.Repositories;
using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;

namespace NTMiner {
    public partial class NTMinerRoot {
        public const int SpeedHistoryLengthByMinute = 10;
        public const int GpuAllId = -1;
        public static readonly bool IsAutoStart = (NTMinerRegistry.GetIsAutoStart() || CommandLineArgs.IsAutoStart);
        private static readonly NTMinerRoot S_Instance = new NTMinerRoot();
        public static readonly INTMinerRoot Instance = S_Instance;
        public static readonly Version CurrentVersion;
        public static readonly string CurrentVersionTag;

        public static string ServerVersion;
        public static Action RefreshArgsAssembly = () => { };
        public static bool IsUiVisible;
        public static DateTime MainWindowRendedOn = DateTime.MinValue;

        public static bool IsUseDevConsole = false;
        // ReSharper disable once InconsistentNaming
        public static int OSVirtualMemoryMb;
        public static string UserKernelCommandLine;

        public static bool IsAutoStartCanceled = false;

        private static bool _isMinerClient;
        public static bool IsMinerClient {
            get => _isMinerClient;
        }

        public static void SetIsMinerClient(bool value) {
            _isMinerClient = value;
        }

        public static bool IsKernelBrand {
            get {
                return KernelBrandId != Guid.Empty;
            }
        }

        private static Guid? kernelBrandId = null;
        public static Guid KernelBrandId {
            get {
                if (!kernelBrandId.HasValue) {
                    kernelBrandId = VirtualRoot.GetBrandId(VirtualRoot.AppFileFullName, Consts.KernelBrandId);
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
                    poolBrandId = VirtualRoot.GetBrandId(VirtualRoot.AppFileFullName, Consts.PoolBrandId);
                }
                return poolBrandId.Value;
            }
        }

        public static bool IsBrandSpecified {
            get {
                return KernelBrandId != Guid.Empty || PoolBrandId != Guid.Empty;
            }
        }

        static NTMinerRoot() {
            Assembly mainAssembly = Assembly.GetEntryAssembly();
            CurrentVersion = mainAssembly.GetName().Version;
            ServerVersion = CurrentVersion.ToString();
            CurrentVersionTag = ((AssemblyDescriptionAttribute)mainAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), inherit: false).First()).Description;
        }

        public static bool IsNCard {
            get {
                try {
                    foreach (ManagementBaseObject item in new ManagementObjectSearcher("SELECT Caption FROM Win32_VideoController").Get()) {
                        foreach (var property in item.Properties) {
                            if ((property.Value ?? string.Empty).ToString().Contains("NVIDIA")) {
                                return true;
                            }
                        }
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
                return false;
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
                                Logger.ErrorDebugLine(e);
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
                                    var poolToRemoves = data.Pools.Where(a => a.BrandId != PoolBrandId).ToArray();
                                    foreach (var item in poolToRemoves) {
                                        data.Pools.Remove(item);
                                    }
                                    var poolKernelToRemoves = data.PoolKernels.Where(a => a.PoolId != PoolBrandId).ToArray();
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

        /// <summary>
        /// 将当前的系统状态导出到给定的json文件
        /// </summary>
        /// <returns></returns>
        public static void ExportServerVersionJson(string jsonFileFullName) {
            ServerJsonDb serverJsonObj = new ServerJsonDb(Instance);
            string json = VirtualRoot.JsonSerializer.Serialize(serverJsonObj);
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
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
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

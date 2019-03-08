using Microsoft.Win32;
using NTMiner.Core;
using NTMiner.Core.Impl;
using NTMiner.Core.Kernels;
using NTMiner.Repositories;
using System;
using System.Linq;
using System.Reflection;

namespace NTMiner {
    public partial class NTMinerRoot {
        public static IKernelDownloader KernelDownloader;
        public static Func<System.Windows.Forms.Keys, bool> RegHotKey;
        public static string AppName;
        public static bool IsUseDevConsole = false;
        public static int OSVirtualMemoryMb;

        public static readonly int GpuAllId = -1;

        static NTMinerRoot() {
            Assembly mainAssembly = Assembly.GetEntryAssembly();
            CurrentVersion = mainAssembly.GetName().Version;
            CurrentVersionTag = ((AssemblyDescriptionAttribute)mainAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), inherit: false).First()).Description;
        }

        private static readonly NTMinerRoot s_current = new NTMinerRoot();
        public static readonly INTMinerRoot Current = s_current;
        public static readonly Version CurrentVersion;
        public static readonly string CurrentVersionTag;
        private static string s_jsonFileVersion;
        public static string JsonFileVersion {
            get { return s_jsonFileVersion; }
            set {
                if (s_jsonFileVersion != value && !string.IsNullOrEmpty(s_jsonFileVersion)) {
                    s_jsonFileVersion = value;
                    VirtualRoot.Happened(new ServerJsonVersionChangedEvent());
                }
            }
        }

        public static IRepository<T> CreateLocalRepository<T>(bool isUseJson) where T : class, IDbEntity<Guid> {
            return new CommonRepository<T>(SpecialPath.LocalDbFileFullName);
        }

        public static IRepository<T> CreateServerRepository<T>(bool isUseJson) where T : class, IDbEntity<Guid> {
            if (!isUseJson) {
                return new CommonRepository<T>(SpecialPath.ServerDbFileFullName);
            }
            else {
                return new ReadOnlyServerRepository<T>();
            }
        }

        /// <summary>
        /// 创建组合仓储，组合仓储由ServerDb和ProfileDb层序组成。
        /// 如果是开发者则访问ServerDb且只访问GlobalDb，否则将ServerDb和ProfileDb并起来访问且不能修改删除GlobalDb。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRepository<T> CreateCompositeRepository<T>(bool isUseJson) where T : class, ILevelEntity<Guid> {
            return new CompositeRepository<T>(CreateServerRepository<T>(isUseJson), CreateLocalRepository<T>(isUseJson));
        }

        public static string GetThisPcName() {
            string value = Environment.MachineName.ToLower();
            value = new string(value.ToCharArray().Where(a => !MinerNameConst.InvalidChars.Contains(a)).ToArray());
            return value;
        }

        #region MinerName
        public static string GetMinerName() {
            object locationValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistry.NTMinerRegistrySubKey, "MinerName");
            string result = string.Empty;
            if (locationValue != null) {
                result =(string)locationValue;
            }
            if (string.IsNullOrEmpty(result)) {
                result = GetThisPcName();
            }
            return result;
        }

        private static void SetMinerName(string minerName) {
            if (!string.IsNullOrEmpty(minerName)) {
                minerName = new string(minerName.ToCharArray().Where(a => !MinerNameConst.InvalidChars.Contains(a)).ToArray());
            }
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistry.NTMinerRegistrySubKey, "MinerName", minerName ?? string.Empty);
            VirtualRoot.Execute(new RefreshArgsAssemblyCommand());
        }
        #endregion

        #region IsShowInTaskbar
        public static bool GetIsShowInTaskbar() {
            object isAutoBootValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistry.NTMinerRegistrySubKey, "IsShowInTaskbar");
            return isAutoBootValue == null || isAutoBootValue.ToString() == "True";
        }

        public static void SetIsShowInTaskbar(bool value) {
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistry.NTMinerRegistrySubKey, "IsShowInTaskbar", value);
        }
        #endregion

        #region IsShowNotifyIcon
        public static bool GetIsShowNotifyIcon() {
            object isAutoBootValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistry.NTMinerRegistrySubKey, "IsShowNotifyIcon");
            return isAutoBootValue == null || isAutoBootValue.ToString() == "True";
        }

        public static void SetIsShowNotifyIcon(bool value) {
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistry.NTMinerRegistrySubKey, "IsShowNotifyIcon", value);
        }
        #endregion

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
            System.Windows.Forms.Keys key;
            if (Enum.TryParse(value, out key) && key >= System.Windows.Forms.Keys.A && key <= System.Windows.Forms.Keys.Z) {
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

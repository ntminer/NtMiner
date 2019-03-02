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
        public static string AppName;
        public static bool IsUseDevConsole = false;

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

        public static bool IsAutoStartCanceled = false;

        public static IRepository<T> CreateLocalRepository<T>() where T : class, IDbEntity<Guid> {
            return new CommonRepository<T>(SpecialPath.LocalDbFileFullName);
        }

        public static IRepository<T> CreateServerRepository<T>() where T : class, IDbEntity<Guid> {
            if (DevMode.IsDebugMode) {
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
        public static IRepository<T> CreateCompositeRepository<T>() where T : class, ILevelEntity<Guid> {
            return new CompositeRepository<T>(CreateServerRepository<T>(), CreateLocalRepository<T>());
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

        public static void SetMinerName(string minerName) {
            if (string.IsNullOrEmpty(minerName)) {
                minerName = GetThisPcName();
            }
            else {
                minerName = new string(minerName.ToCharArray().Where(a => !MinerNameConst.InvalidChars.Contains(a)).ToArray());
            }
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistry.NTMinerRegistrySubKey, "MinerName", minerName);
            VirtualRoot.Execute(new RefreshArgsAssemblyCommand());
        }
        #endregion

        #region IsAutoThisPCName
        public static bool GetIsAutoThisPCName() {
            object isAutoBootValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistry.NTMinerRegistrySubKey, "IsAutoThisPCName");
            return isAutoBootValue != null && isAutoBootValue.ToString() == "True";
        }

        public static void SetIsAutoThisPCName(bool value) {
            SetMinerName(GetThisPcName());
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistry.NTMinerRegistrySubKey, "IsAutoThisPCName", value);
        }
        #endregion

        #region IsShowInTaskbar
        public static bool GetIsShowInTaskbar() {
            object isAutoBootValue = Windows.Registry.GetValue(Registry.Users, NTMinerRegistry.NTMinerRegistrySubKey, "IsShowInTaskbar");
            return isAutoBootValue != null && isAutoBootValue.ToString() == "True";
        }

        public static void SetIsShowInTaskbar(bool value) {
            Windows.Registry.SetValue(Registry.Users, NTMinerRegistry.NTMinerRegistrySubKey, "IsShowInTaskbar", value);
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

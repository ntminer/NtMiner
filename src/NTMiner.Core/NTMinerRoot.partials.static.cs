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

        public static readonly int GpuAllId = -1;

        static NTMinerRoot() {
            Assembly mainAssembly = Assembly.GetEntryAssembly();
            CurrentVersion = mainAssembly.GetName().Version;
            CurrentVersionTag = ((AssemblyDescriptionAttribute)mainAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), inherit: false).First()).Description;
        }

        private static readonly NTMinerRoot _current = new NTMinerRoot();
        public static readonly INTMinerRoot Current = _current;
        public static readonly Version CurrentVersion;
        public static readonly string CurrentVersionTag;
        private static ulong _jsonFileVersion;
        public static ulong JsonFileVersion {
            get { return _jsonFileVersion; }
            set {
                _jsonFileVersion = value;
                Global.Happened(new ServerJsonVersionChangedEvent());
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
        /// 创建组合仓储，组合仓储由GlobalDb和ProfileDb层序组成。
        /// 如果是开发者则访问GlobalDb且只访问GlobalDb，否则将GlobalDb和ProfileDb并起来访问且不能修改删除GlobalDb。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRepository<T> CreateCompositeRepository<T>() where T : class, ILevelEntity<Guid> {
            return new CompositeRepository<T>(CreateServerRepository<T>(), CreateLocalRepository<T>());
        }
    }
}

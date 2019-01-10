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
            var atrr = mainAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), inherit: false).Cast<AssemblyDescriptionAttribute>().FirstOrDefault();
            if (atrr != null) {
                VersionTag = atrr.Description;
            }
            else {
                VersionTag = "谜麟";
            }
        }

        private static readonly NTMinerRoot _current = new NTMinerRoot();
        public static readonly INTMinerRoot Current = _current;
        public static readonly Version CurrentVersion;
        public static readonly string CurrentVersionTag;
        public static readonly string VersionTag;

        public static bool IsAutoStartCanceled = false;

        public static string Title {
            get {
                if (CommandLineArgs.IsControlCenter && !CommandLineArgs.IsWorker) {
                    return "开源矿工中控";
                }
                return "开源矿工";
            }
        }

        public static IRepository<T> CreateLocalRepository<T>() where T : class, IDbEntity<Guid> {
            return new CommonRepository<T>(SpecialPath.LocalDbFileFullName);
        }

        public static IRepository<T> CreateServerRepository<T>() where T : class, IDbEntity<Guid> {
            if (DevMode.IsDevMode) {
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

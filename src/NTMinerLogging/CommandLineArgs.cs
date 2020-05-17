using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static partial class CommandLineArgs {
        public static List<string> Args { get; private set; } = Environment.GetCommandLineArgs().Skip(1).ToList();

        // 注意：这些来自命令行的参数不一定对每种应用程序都有用，但因为它们都是简单的基本类型不依赖别处只依赖.NET框架统一都放这里吧。
        // 用属性而不用字段因为visual studio ide对字段不显式引用计数。
        // 最新的C#语法模式对只读属性无需声明private set访问器，但那样的话就没法在set上右键查找引用位置了。

        public static bool IsAutoStart { get; private set; }
        public static string Upgrade { get; private set; }
        public static string Action { get; private set; }
        public static string NTMinerFileName { get; private set; }

        static CommandLineArgs() {
            IsAutoStart = Args.Contains(NTKeyword.AutoStartCmdParameterName, StringComparer.OrdinalIgnoreCase);
            Upgrade = PickArgument(NTKeyword.UpgradeCmdParameterName);
            Action = PickArgument(NTKeyword.ActionCmdParameterName);
            NTMinerFileName = PickArgument("ntminerFileName=");
        }

        /// <summary>
        /// 返回的日志文件名和应用程序的类型和版本以及启动参数有关系。
        /// 主要是因为挖矿端应用程序不是单例的，也就是说已经有一个挖矿端程序进程时当挖矿端程序启动时追加了比如upgrade=ntminer2.8.exe参数的话依旧可以启动进程。
        /// </summary>
        /// <returns></returns>
        public static string GetLogFileName() {
            // 避免不同进程使用相同的日志文件，虽然并不会异常但会看不到日志
            if (!string.IsNullOrEmpty(Upgrade)) {
                return $"root{NTKeyword.VersionBuild}_upgrade.log";
            }
            else if (!string.IsNullOrEmpty(Action)) {
                return $"root{NTKeyword.VersionBuild}_{Action}.log";
            }
            return $"root{NTKeyword.VersionBuild}.log";
        }

        /// <summary>
        /// 提取格式形如argumentName=argumentValue格式的命令行参数。
        /// 注意：参数名是忽略大小写的，且如果命令行上有重名参数后面的值覆盖前面的值
        /// </summary>
        /// <param name="argumentName"></param>
        /// <returns></returns>
        private static string PickArgument(string argumentName) {
            string result = string.Empty;
            int index = -1;
            for (int i = 0; i < Args.Count; i++) {
                string item = Args[i];
                if (item.StartsWith(argumentName, StringComparison.OrdinalIgnoreCase)) {
                    string[] parts = item.Split('=');
                    if (parts.Length == 2) {
                        result = parts[1];
                        index = i;
                    }
                }
            }
            // 移除形如upgrade=格式的没有值的参数
            if (string.IsNullOrEmpty(result) && index != -1) {
                Args.RemoveAt(index);
            }
            return result;
        }
    }
}

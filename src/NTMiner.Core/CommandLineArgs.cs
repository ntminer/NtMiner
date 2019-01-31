using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static class CommandLineArgs {
        /// <summary>
        /// 是否是工人。工人被雇佣，受中控管理，非工人则是自由的。
        /// </summary>
        public static readonly bool IsWorker;
        public static readonly Guid WorkId = Guid.Empty;
        public static readonly bool IsControlCenter;
        public static readonly bool JustClientWorker;
        public static readonly bool IsFreeClient;
        public static readonly bool IsWorkEdit;
        public static readonly bool IsAutoStart;
        public static readonly bool IsSkipDownloadJson;
        public static readonly string Upgrade;

        private static readonly List<string> _commandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToList();
        public static List<string> Args {
            get {
                return _commandLineArgs;
            }
        }

        static CommandLineArgs() {
            IsSkipDownloadJson = _commandLineArgs.Contains("--skipDownloadJson", StringComparer.OrdinalIgnoreCase);
            IsControlCenter = _commandLineArgs.Contains("--ControlCenter", StringComparer.OrdinalIgnoreCase);
            IsAutoStart = _commandLineArgs.Contains("--AutoStart", StringComparer.OrdinalIgnoreCase);
            Upgrade = PickArgument("upgrade=");
            string workId = PickArgument("workid=");
            Guid.TryParse(workId, out WorkId);
            IsWorkEdit = IsControlCenter && IsWorker;
            JustClientWorker = !IsControlCenter && IsWorker;
            IsFreeClient = !IsControlCenter && !IsWorker;
        }

        private static string PickArgument(string argumentName) {
            string result = string.Empty;
            int index = -1;
            for (int i = 0; i < _commandLineArgs.Count; i++) {
                string item = _commandLineArgs[i];
                if (item.StartsWith(argumentName)) {
                    string[] parts = item.Split('=');
                    if (parts.Length == 2) {
                        result = parts[0];
                        index = i;
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(result) && index != -1) {
                _commandLineArgs.RemoveAt(index);
            }
            return result;
        }
    }
}

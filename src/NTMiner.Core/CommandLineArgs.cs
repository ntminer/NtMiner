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

        private static readonly List<string> _commandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToList();
        public static List<string> Args {
            get {
                return _commandLineArgs;
            }
        }

        static CommandLineArgs() {
            IsControlCenter = _commandLineArgs.Contains("--ControlCenter", StringComparer.OrdinalIgnoreCase);
            IsAutoStart = _commandLineArgs.Contains("--AutoStart", StringComparer.OrdinalIgnoreCase);
            int index = 0;
            for (; index < _commandLineArgs.Count; index++) {
                string item = _commandLineArgs[index];
                if (item.StartsWith("--workid=", StringComparison.OrdinalIgnoreCase)) {
                    string[] parts = item.Split('=');
                    if (parts.Length != 2) {
                        throw new InvalidProgramException("--workid参数格式错误");
                    }
                    if (!Guid.TryParse(parts[1], out WorkId)) {
                        throw new InvalidProgramException();
                    }
                    IsWorker = WorkId != Guid.Empty;
                    if (!IsWorker) {
                        _commandLineArgs.RemoveAt(index);
                    }
                    break;
                }
            }
            IsWorkEdit = IsControlCenter && IsWorker;
            JustClientWorker = !IsControlCenter && IsWorker;
            IsFreeClient = !IsControlCenter && !IsWorker;
        }
    }
}

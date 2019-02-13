using System;
using System.Linq;

namespace NTMiner {
    public static partial class CommandLineArgs {
        /// <summary>
        /// 是否是工人。工人被雇佣，受中控管理，非工人则是自由的。
        /// </summary>
        public static readonly Guid WorkId = Guid.Empty;
        public static readonly bool IsControlCenter;
        public static readonly bool JustClientWorker;
        public static readonly bool IsFreeClient;
        public static readonly bool IsWorkEdit;
        public static readonly bool IsAutoStart;
        public static readonly bool IsSkipDownloadJson;
        public static readonly string Upgrade;

        static CommandLineArgs() {
            IsSkipDownloadJson = _commandLineArgs.Contains("--skipDownloadJson", StringComparer.OrdinalIgnoreCase);
            IsControlCenter = _commandLineArgs.Contains("--ControlCenter", StringComparer.OrdinalIgnoreCase);
            IsAutoStart = _commandLineArgs.Contains("--AutoStart", StringComparer.OrdinalIgnoreCase);
            Upgrade = PickArgument("upgrade=");
            string workId = PickArgument("workid=");
            Guid.TryParse(workId, out WorkId);
            bool isWorker = WorkId != Guid.Empty;
            IsWorkEdit = IsControlCenter && isWorker;
            JustClientWorker = !IsControlCenter && isWorker;
            IsFreeClient = !IsControlCenter && !isWorker;
        }
    }
}

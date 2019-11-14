using System;
using System.Linq;

namespace NTMiner {
    public static partial class CommandLineArgs {
        public static readonly bool IsAutoStart;
        public static readonly string Upgrade;

        static CommandLineArgs() {
            IsAutoStart = s_commandLineArgs.Contains(NTKeyword.AutoStartCmdParameterName, StringComparer.OrdinalIgnoreCase);
            Upgrade = PickArgument(NTKeyword.UpgradeCmdParameterName);
        }
    }
}

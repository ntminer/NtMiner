using System;
using System.Linq;

namespace NTMiner {
    public static partial class CommandLineArgs {
        public static readonly bool IsAutoStart;
        public static readonly string Upgrade;
        public static readonly string KernelBrand;

        static CommandLineArgs() {
            IsAutoStart = s_commandLineArgs.Contains("--AutoStart", StringComparer.OrdinalIgnoreCase);
            Upgrade = PickArgument("upgrade=");
            KernelBrand = PickArgument("KernelBrand=");
        }
    }
}

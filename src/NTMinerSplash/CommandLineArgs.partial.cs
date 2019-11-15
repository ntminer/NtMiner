namespace NTMiner {
    public static partial class CommandLineArgs {
        public static readonly string VersionFullName;

        static CommandLineArgs() {
            VersionFullName = PickArgument(nameof(VersionFullName));
        }
    }
}

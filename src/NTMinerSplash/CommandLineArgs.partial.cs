namespace NTMiner {
    public static partial class CommandLineArgs {
        public static readonly string VersionFullName;
        public static readonly int ParentProcessId;

        static CommandLineArgs() {
            VersionFullName = PickArgument(nameof(VersionFullName));
            string parentProcessId = PickArgument(nameof(ParentProcessId));
            int.TryParse(parentProcessId, out ParentProcessId);
        }
    }
}

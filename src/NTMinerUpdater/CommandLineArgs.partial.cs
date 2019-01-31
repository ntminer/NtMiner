namespace NTMiner {
    public static partial class CommandLineArgs {
        public static readonly string NTMinerFileName;

        static CommandLineArgs() {
            NTMinerFileName = PickArgument("ntminerFileName=");
        }
    }
}

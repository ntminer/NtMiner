namespace NTMiner {
    public enum StopMineReason {
        Unknown,
        LocalUserAction,
        InStartMine,
        HighCpuTemperature,
        RPCUserAction,
        KernelProcessLost,
        RestartMine,
        ApplicationExit
    }
}

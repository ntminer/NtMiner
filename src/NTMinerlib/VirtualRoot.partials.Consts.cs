namespace NTMiner {
    public static partial class VirtualRoot {
        public const string HomeDirParameterName = "{家目录}";
        public const string TempDirParameterName = "{临时目录}";
        public const int MinerClientPort = 3336;
        public const int NTMinerDaemonPort = 3337;
        public const int MinerStudioPort = 3338;
        public const int ControlCenterPort = 3339;

        public const int WorkerMessageSetCapacity = 1000;

        #region 系统字典编码
        public const string KernelBrandSysDicCode = "KernelBrand";
        public const string PoolBrandSysDicCode = "PoolBrand";
        public const string AlgoSysDicCode = "Algo";
        public const string LogColorSysDicCode = "LogColor";
        public const string CudaVersionSysDicCode = "CudaVersion";
        public const string ThisSystemSysDicCode = "ThisSystem";
        #endregion

        #region 打在程序集中的定位关键字
        public const string KernelBrandId = "KernelBrandId";
        public const string PoolBrandId = "PoolBrandId";
        #endregion

        #region 正则表达式组名
        public const string TotalSpeedGroupName = "totalSpeed";
        public const string TotalSpeedUnitGroupName = "totalSpeedUnit";
        public const string TotalShareGroupName = "totalShare";
        public const string AcceptShareGroupName = "acceptShare";
        public const string RejectShareGroupName = "rejectShare";
        public const string RejectPercentGroupName = "rejectPercent";
        public const string GpuIndexGroupName = "gpu";
        public const string GpuSpeedGroupName = "gpuSpeed";
        public const string GpuSpeedUnitGroupName = "gpuSpeedUnit";
        public const string PoolDelayGroupName = "poolDelay";
        #endregion

        #region 上下文变量名
        public const string MainCoinParameterName = "mainCoin";
        public const string UserNameParameterName = "userName";
        public const string PasswordParameterName = "password";
        public const string PasswordDefaultValue = "x";
        public const string WalletParameterName = "wallet";
        public const string HostParameterName = "host";
        public const string PortParameterName = "port";
        public const string PoolParameterName = "pool";
        public const string WorkerParameterName = "worker";
        public const string Worker1ParameterName = "worker1";
        public const string Host1ParameterName = "host1";
        public const string Port1ParameterName = "port1";
        public const string Pool1ParameterName = "pool1";
        public const string UserName1ParameterName = "userName1";
        public const string Password1ParameterName = "password1";
        public const string Wallet1ParameterName = "wallet1";
        public const string DualCoinParameterName = "dualCoin";
        public const string DualWalletParameterName = "dualWallet";
        public const string DualUserNameParameterName = "dualUserName";
        public const string DualPasswordParameterName = "dualPassword";
        public const string DualHostParameterName = "dualHost";
        public const string DualPortParameterName = "dualPort";
        public const string DualPoolParameterName = "dualPool";
        #endregion

        // 因为界面上输入框不好体现输入的空格，所以这里对空格进行转义
        public const string SpaceKeyword = "space";
    }
}

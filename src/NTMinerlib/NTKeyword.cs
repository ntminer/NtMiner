namespace NTMiner {
    public static class NTKeyword {
        public const string WpfDesignOnly = "这是供WPF设计时使用的构造，不应在业务代码中被调用";

        #region AppMutex
        public const string MinerClientAppMutex = "ntminerclient";
        public const string MinerStudioAppMutex = "ntminercontrol";
        public const string MinerUpdaterAppMutex = "NTMinerUpdaterAppMutex";
        public const string MinerClientFinderAppMutex = "MinerClientFinderAppMutex";
        #endregion

        public const string HomeDirParameterName = "{家目录}";
        public const string TempDirParameterName = "{临时目录}";
        public const int MinerClientPort = 3336;
        public const int NTMinerDaemonPort = 3337;
        public const int MinerStudioPort = 3338;
        public const int ControlCenterPort = 3339;
        public const string ServerHost = "server.ntminer.com";
        public const string DNSServer0 = "119.29.29.29";
        public const string DNSServer1 = "223.5.5.5";
        public static string OfficialServerHost { get; private set; } = ServerHost;
        public static void SetOfficialServerHost(string host) {
            OfficialServerHost = host;
        }

        public const string NTMinerUpdaterFileName = "NTMinerUpdater.exe";
        public const string MinerClientFinderFileName = "MinerClientFinder.exe";
        public const string NTMinerServicesFileName = "NTMinerServices.exe";
        public const string GpuProfilesFileName = "gpuProfiles.json";
        public const string LocalJsonFileName = "local.json";
        public const string LocalDbFileName = "local.litedb";
        public const string ServerJsonFileName = "server.json";
        public const string ServerDbFileName = "server.litedb";
        public const string DevConsoleFileName = "DevConsole.exe";
        public const string NTMinerDaemonFileName = "NTMinerDaemon.exe";

        public const string NTMinerDaemonKey = "NTMiner.Daemon.NTMinerDaemon.exe";
        public const string NTMinerServicesKey = "NTMiner.NTMinerServices.NTMinerServices.exe";
        public const string MinerStudioCmdParameterName = "--minerstudio";
        public const string EnableInnerIpCmdParameterName = "--enableInnerIp";
        public const string NotOfficialCmdParameterName = "--notofficial";
        public const string AutoStartCmdParameterName = "--AutoStart";
        public const string UpgradeCmdParameterName = "upgrade=";

        public const int LocalMessageSetCapacity = 1000;
        public const int ServerMessageSetCapacity = 1000;
        // 矿工名中不可以包含的字符
        public static readonly char[] InvalidMinerNameChars = { '.', ' ', '-', '_' };
        // 因为界面上输入框不好体现输入的空格，所以这里对空格进行转义
        public const string SpaceKeyword = "space";

        // 如果没有使用分隔符分割序号的话无法表达两位数的序号，此时这种情况基本都是用ABCDEFGH……表达的后续的两位数
        private static readonly string[] IndexChars = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n" };
        public static string GetIndexChar(int index, string separator) {
            if (index <= 9 || !string.IsNullOrEmpty(separator)) {
                return index.ToString();
            }
            return IndexChars[index - 10];
        }

        #region 目录名
        public const string DaemonDirName = "Daemon";
        public const string PackagesDirName = "Packages";
        public const string CoinIconsDirName = "CoinIcons";
        public const string DownloadDirName = "Download";
        public const string KernelsDirName = "Kernels";
        public const string LogsDirName = "Logs";
        public const string ToolsDirName = "Tools";
        public const string UpdaterDirName = "Updater";
        public const string ServicesDirName = "Services";
        #endregion

        #region 注册表
        public const string MinerNameRegistryKey = "MinerName";
        public const string ClientIdRegistryKey = "ClientId";
        public const string DaemonActiveOnRegistryKey = "DaemonActiveOn";
        public const string DaemonVersionRegistryKey = "DaemonVersion";
        public const string ControlCenterHostsRegistryKey = "ControlCenterHosts";
        public const string ControlCenterHostRegistryKey = "ControlCenterHost";
        public const string CurrentVersionTagRegistryKey = "CurrentVersionTag";
        public const string CurrentVersionRegistryKey = "CurrentVersion";
        public const string ArgumentsRegistryKey = "Arguments";
        public const string IsLastIsWorkRegistryKey = "IsLastIsWork";
        public const string LocationRegistryKey = "Location";
        #endregion

        #region LocalAppSettingKey
        public const string UseDevicesAppSettingKey = "UseDevices";
        public const string UpdaterVersionAppSettingKey = "UpdaterVersion";
        public const string MinerClientFinderVersionAppSettingKey = "MinerClientFinderVersion";
        public const string ServerJsonVersionAppSettingKey = "ServerJsonVersion";
        #endregion

        #region ServerAppSettingKey
        public const string ColumnsShowIdAppSettingKey = "ColumnsShowId";
        public const string FrozenColumnCountAppSettingKey = "FrozenColumnCount";
        public const string MaxTempAppSettingKey = "MaxTemp";
        public const string MinTempAppSettingKey = "MinTemp";
        public const string RejectPercentAppSettingKey = "RejectPercent";
        public const string NTMinerUpdaterFileNameAppSettingKey = "NTMinerUpdaterFileName";
        public const string MinerClientFinderFileNameAppSettingKey = " MinerClientFinderFileName";
        #endregion

        #region 系统字典编码
        public const string KernelBrandSysDicCode = "KernelBrand";
        public const string PoolBrandSysDicCode = "PoolBrand";
        public const string AlgoSysDicCode = "Algo";
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

        public const string LogFileParameterName = "{logfile}";

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
    }
}

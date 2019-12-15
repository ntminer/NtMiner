using Aliyun.OSS;
using LiteDB;
using NTMiner.Core;
using NTMiner.Data;
using NTMiner.Data.Impl;
using NTMiner.KernelOutputKeyword;
using NTMiner.MinerServer;
using NTMiner.ServerMessage;
using NTMiner.User;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace NTMiner {
    public class HostRoot : IHostRoot {
        private static readonly EventWaitHandle WaitHandle = new AutoResetEvent(false);
        public static readonly bool IsNotOfficial = Environment.CommandLine.IndexOf(NTKeyword.NotOfficialCmdParameterName, StringComparison.OrdinalIgnoreCase) != -1;
        public static readonly bool EnableInnerIp = Environment.CommandLine.IndexOf(NTKeyword.EnableInnerIpCmdParameterName, StringComparison.OrdinalIgnoreCase) != -1;

        private static Mutex _sMutexApp;
        // 该程序编译为控制台程序，如果不启用内网支持则默认设置为开机自动启动
        [STAThread]
        static void Main() {
            try {
                Console.Title = "NTMinerServices";
                bool mutexCreated;
                try {
                    _sMutexApp = new Mutex(true, "NTMinerServicesMutex", out mutexCreated);
                }
                catch {
                    mutexCreated = false;
                }
                if (mutexCreated) {
                    VirtualRoot.StartTimer();
                    if (!EnableInnerIp) {
                        NTMinerRegistry.SetAutoBoot("NTMinerServices", true);
                    }
                    Type thisType = typeof(HostRoot);
                    Run();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static bool _isClosed = false;
        private static void Close() {
            if (!_isClosed) {
                _isClosed = true;
                HttpServer.Stop();
                _sMutexApp?.Dispose();
            }
        }

        public static void Exit() {
            Close();
            Environment.Exit(0);
        }

        private static void Run() {
            try {
                string baseAddress = $"http://localhost:{NTKeyword.ControlCenterPort.ToString()}";
                HttpServer.Start(baseAddress);
                Windows.ConsoleHandler.Register(Close);
                WaitHandle.WaitOne();
                Close();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            finally {
                Close();
            }
        }

        public DateTime StartedOn { get; private set; } = DateTime.Now;

        public static readonly IHostRoot Instance = new HostRoot();
        public static readonly ClientCount ClientCount = new ClientCount();

        private OssClient _ossClient = null;

        public OssClient OssClient {
            get {
                OssClientInit();
                return _ossClient;
            }
        }

        #region OSSClientInit
        private DateTime _ossClientOn = DateTime.MinValue;
        private readonly object _ossClientLocker = new object();
        private void OssClientInit() {
            if (IsNotOfficial) {
                this.HostConfig = HostConfigData.LocalHostConfig;
            }
            else {
                DateTime now = DateTime.Now;
                if (_ossClientOn.AddMinutes(10) < now) {
                    lock (_ossClientLocker) {
                        if (_ossClientOn.AddMinutes(10) < now) {
                            HostConfigData hostConfigData;
                            using (LiteDatabase db = CreateLocalDb()) {
                                var col = db.GetCollection<HostConfigData>();
                                hostConfigData = col.FindOne(Query.All());
                            }
                            if (hostConfigData == null) {
                                Console.WriteLine("HostConfigData未配置");
                            }
                            else {
                                this.HostConfig = hostConfigData;
                            }
                            string accessKeyId = hostConfigData.OssAccessKeyId;
                            string accessKeySecret = hostConfigData.OssAccessKeySecret;
                            string endpoint = hostConfigData.OssEndpoint;
                            _ossClientOn = DateTime.Now;
                        }
                    }
                }
            }
            _ossClient = new OssClient(this.HostConfig.OssEndpoint, this.HostConfig.OssAccessKeyId, this.HostConfig.OssAccessKeySecret);
        }
        #endregion

        public static LiteDatabase CreateLocalDb() {
            return new LiteDatabase($"filename={SpecialPath.LocalDbFileFullName};journal=false");
        }

        public static LiteDatabase CreateReportDb() {
            string dbFileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"report{DateTime.Now.ToString("yyyy-MM-dd")}.litedb");
            return new LiteDatabase($"filename={dbFileFullName};journal=false");
        }

        public static ServerState GetServerState(string jsonVersionKey) {
            string jsonVersion = string.Empty;
            string minerClientVersion = string.Empty;
            try {
                var fileData = Instance.NTMinerFileSet.LatestMinerClientFile;
                minerClientVersion = fileData != null ? fileData.Version : string.Empty;
                if (!VirtualRoot.LocalAppSettingSet.TryGetAppSetting(jsonVersionKey, out IAppSetting data) || data.Value == null) {
                    jsonVersion = string.Empty;
                }
                else {
                    jsonVersion = data.Value.ToString();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            return new ServerState {
                JsonFileVersion = jsonVersion,
                MinerClientVersion = minerClientVersion,
                Time = Timestamp.GetTimestamp(),
                MessageTimestamp = Timestamp.GetTimestamp(Instance.ServerMessageTimestamp),
                OutputKeywordTimestamp = Timestamp.GetTimestamp(Instance.KernelOutputKeywordTimestamp)
            };
        }

        private HostRoot() {
            OssClientInit();
            this.UserSet = new UserSet(SpecialPath.LocalDbFileFullName);
            this.CalcConfigSet = new CalcConfigSet(this);
            this.ColumnsShowSet = new ColumnsShowSet(this);
            this.NTMinerWalletSet = new NTMinerWalletSet(this);
            this.ClientSet = new ClientSet();
            this.CoinSnapshotSet = new CoinSnapshotSet(this);
            this.MineWorkSet = new MineWorkSet(this);
            this.MinerGroupSet = new MinerGroupSet(this);
            this.WalletSet = new WalletSet(this);
            this.PoolSet = new PoolSet(this);
            this.NTMinerFileSet = new NTMinerFileSet(this);
            this.OverClockDataSet = new OverClockDataSet(this);
            this.KernelOutputKeywordSet = new KernelOutputKeywordSet(SpecialPath.LocalDbFileFullName, isServer: true);
            this.ServerMessageSet = new ServerMessageSet(SpecialPath.LocalDbFileFullName, isServer: true);
            this.UpdateServerMessageTimestamp();
            if (VirtualRoot.LocalAppSettingSet.TryGetAppSetting(nameof(KernelOutputKeywordTimestamp), out IAppSetting appSetting) && appSetting.Value is DateTime value) {
                this.KernelOutputKeywordTimestamp = value;
            }
            else {
                this.KernelOutputKeywordTimestamp = Timestamp.UnixBaseTime;
            }
        }

        public IUserSet UserSet { get; private set; }

        public ICalcConfigSet CalcConfigSet { get; private set; }

        public IColumnsShowSet ColumnsShowSet { get; private set; }

        public INTMinerWalletSet NTMinerWalletSet { get; private set; }

        public IClientSet ClientSet { get; private set; }

        public ICoinSnapshotSet CoinSnapshotSet { get; private set; }

        public IMineWorkSet MineWorkSet { get; private set; }

        public IMinerGroupSet MinerGroupSet { get; private set; }

        public IWalletSet WalletSet { get; private set; }

        public IPoolSet PoolSet { get; private set; }

        public INTMinerFileSet NTMinerFileSet { get; private set; }

        public IHostConfig HostConfig { get; private set; }

        public IOverClockDataSet OverClockDataSet { get; private set; }

        public IKernelOutputKeywordSet KernelOutputKeywordSet { get; private set; }

        public IServerMessageSet ServerMessageSet { get; private set; }

        public DateTime ServerMessageTimestamp { get; set; }

        public DateTime KernelOutputKeywordTimestamp { get; private set; }

        public void UpdateServerMessageTimestamp() {
            var first = this.ServerMessageSet.AsEnumerable().OrderByDescending(a => a.Timestamp).FirstOrDefault();
            if (first == null) {
                this.ServerMessageTimestamp = DateTime.MinValue;
            }
            else {
                this.ServerMessageTimestamp = first.Timestamp;
            }
        }

        public void UpdateKernelOutputKeywordTimestamp(DateTime timestamp) {
            this.KernelOutputKeywordTimestamp = timestamp;
            VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData {
                Key = nameof(KernelOutputKeywordTimestamp),
                Value = timestamp
            }));
        }
    }
}

using Aliyun.OSS;
using LiteDB;
using NTMiner.Data;
using NTMiner.Data.Impl;
using NTMiner.User;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace NTMiner {
    public class HostRoot : IHostRoot {
        public static EventWaitHandle WaitHandle = new AutoResetEvent(false);
        public static ExtendedNotifyIcon NotifyIcon;
        private static Mutex s_mutexApp;
        static void Main(string[] args) {
            try {
                Console.Title = "NTMinerServices";
                bool mutexCreated;
                try {
                    s_mutexApp = new Mutex(true, "NTMinerServicesMutex", out mutexCreated);
                }
                catch {
                    mutexCreated = false;
                }
                if (mutexCreated) {
                    NTMinerRegistry.SetAutoBoot("NTMinerServices", true);
                    Type thisType = typeof(HostRoot);
                    NotifyIcon = ExtendedNotifyIcon.Create(new System.Drawing.Icon(thisType.Assembly.GetManifestResourceStream(thisType, "logo.ico")), "群控服务", isShowNotifyIcon: true);
                    Run();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        private static bool _isClosed = false;
        private static void Close() {
            if (!_isClosed) {
                _isClosed = true;
                HttpServer.Stop();
                s_mutexApp?.Dispose();
                NotifyIcon?.Dispose();
            }
        }

        public static void Exit() {
            Close();
            Environment.Exit(0);
        }

        private static void Run() {
            try {
                string baseAddress = $"http://localhost:{WebApiConst.ControlCenterPort}";
                HttpServer.Start(baseAddress);
                Windows.ConsoleHandler.Register(() => {
                    Close();
                });
                WaitHandle.WaitOne();
                Close();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
            finally {
                Close();
            }
        }

        public DateTime StartedOn { get; private set; } = DateTime.Now;

        public static readonly IHostRoot Current = new HostRoot();
        public static readonly ClientCount ClientCount = new ClientCount();

        private OssClient _ossClient = null;

        public OssClient OssClient {
            get {
                OssClientInit();
                return _ossClient;
            }
        }

        #region OSSClientInit
        private readonly bool _isNotOfficial = Environment.CommandLine.IndexOf("--notofficial", StringComparison.OrdinalIgnoreCase) != -1;
        private DateTime _ossClientOn = DateTime.MinValue;
        private readonly object _ossClientLocker = new object();
        private void OssClientInit() {
            if (_isNotOfficial) {
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
                            _ossClient = new OssClient(endpoint, accessKeyId, accessKeySecret);
                        }
                    }
                }
            }
        }
        #endregion

        public static LiteDatabase CreateLocalDb() {
            return new LiteDatabase($"filename={SpecialPath.LocalDbFileFullName};journal=false");
        }

        public static LiteDatabase CreateReportDb() {
            string dbFileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"report{DateTime.Now.ToString("yyyy-MM-dd")}.litedb");
            return new LiteDatabase($"filename={dbFileFullName};journal=false");
        }

        private HostRoot() {
            OssClientInit();
            this.UserSet = new UserSet(SpecialPath.LocalDbFileFullName);
            this.AppSettingSet = new AppSettingSet(SpecialPath.LocalDbFileFullName);
            this.CalcConfigSet = new CalcConfigSet(this);
            this.ColumnsShowSet = new ColumnsShowSet(this);
            this.ClientSet = new ClientSet(this);
            this.CoinSnapshotSet = new CoinSnapshotSet(this);
            this.MineWorkSet = new MineWorkSet(this);
            this.MinerGroupSet = new MinerGroupSet(this);
            this.WalletSet = new WalletSet(this);
            this.PoolSet = new PoolSet(this);
            this.NTMinerFileSet = new NTMinerFileSet(this);
            this.OverClockDataSet = new OverClockDataSet(this);
        }

        public IUserSet UserSet { get; private set; }

        public IAppSettingSet AppSettingSet { get; private set; }

        public ICalcConfigSet CalcConfigSet { get; private set; }

        public IColumnsShowSet ColumnsShowSet { get; private set; }

        public IClientSet ClientSet { get; private set; }

        public ICoinSnapshotSet CoinSnapshotSet { get; private set; }

        public IMineWorkSet MineWorkSet { get; private set; }

        public IMinerGroupSet MinerGroupSet { get; private set; }

        public IWalletSet WalletSet { get; private set; }

        public IPoolSet PoolSet { get; private set; }

        public INTMinerFileSet NTMinerFileSet { get; private set; }

        public IHostConfig HostConfig { get; private set; }

        public IOverClockDataSet OverClockDataSet { get; private set; }
    }
}

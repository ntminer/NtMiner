using Aliyun.OSS;
using LiteDB;
using NTMiner.Data;
using NTMiner.Data.Impl;
using NTMiner.User;
using NTMiner.User.Impl;
using System;
using System.IO;

namespace NTMiner {
    public class HostRoot : IHostRoot {
        public static bool IsPull = false;

        static void Main(string[] args) {
            string baseAddress = "http://localhost:3339";
            Console.Title = baseAddress + " Enter exit or ctrl+c to quit.";
            HttpServer.Start(baseAddress);
            Logger.InfoDebugLine("启动成功");
            Windows.ConsoleHandler.Register(() => {
                HttpServer.Stop();
            });
            string line = Console.ReadLine();
            while (line != "exit") {
                line = Console.ReadLine();
            }
            HttpServer.Stop();
        }

        public DateTime StartedOn { get; private set; } = DateTime.Now;

        public static readonly IHostRoot Current = new HostRoot();

        private OssClient _ossClient = null;

        public OssClient OssClient {
            get {
                OSSClientInit();
                return _ossClient;
            }
        }

        #region OSSClientInit
        private DateTime _ossClientOn = DateTime.MinValue;
        private readonly object _ossClientLocker = new object();
        private void OSSClientInit() {
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
        #endregion

        public static LiteDatabase CreateLocalDb() {
            return new LiteDatabase($"filename={SpecialPath.LocalDbFileFullName};journal=false");
        }

        public static LiteDatabase CreateReportDb() {
            string dbFileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"report{DateTime.Now.ToString("yyyy-MM-dd")}.litedb");
            return new LiteDatabase($"filename={dbFileFullName};journal=false");
        }

        private HostRoot() {
            OSSClientInit();
            this.UserSet = new UserSet(SpecialPath.LocalDbFileFullName);
            this.AppSettingSet = new AppSettingSet(SpecialPath.LocalDbFileFullName);
            this.CalcConfigSet = new CalcConfigSet(this);
            this.ClientCoinSnapshotSet = new ClientCoinSnapshotSet(this);
            this.ColumnsShowSet = new ColumnsShowSet(this);
            this.ClientSet = new ClientSet(this);
            this.CoinSnapshotSet = new CoinSnapshotSet(this);
            this.MineWorkSet = new MineWorkSet(this);
            this.MinerGroupSet = new MinerGroupSet(this);
            this.WalletSet = new WalletSet(this);
            this.MineProfileManager = new MineProfileManager(this);
            this.NTMinerFileSet = new NTMinerFileSet(this);
            this.OverClockDataSet = new OverClockDataSet(this);
        }

        public IUserSet UserSet { get; private set; }

        public IAppSettingSet AppSettingSet { get; private set; }

        public ICalcConfigSet CalcConfigSet { get; private set; }

        public IClientCoinSnapshotSet ClientCoinSnapshotSet { get; private set; }

        public IColumnsShowSet ColumnsShowSet { get; private set; }

        public IClientSet ClientSet { get; private set; }

        public ICoinSnapshotSet CoinSnapshotSet { get; private set; }

        public IMineWorkSet MineWorkSet { get; private set; }

        public IMinerGroupSet MinerGroupSet { get; private set; }

        public IWalletSet WalletSet { get; private set; }

        public IMineProfileManager MineProfileManager { get; private set; }

        public INTMinerFileSet NTMinerFileSet { get; private set; }

        public IHostConfig HostConfig { get; private set; }

        public IOverClockDataSet OverClockDataSet { get; private set; }
    }
}

using Aliyun.OSS;
using LiteDB;
using NTMiner.Data;
using NTMiner.Data.Impl;
using NTMiner.User;
using NTMiner.User.Impl;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NTMiner {
    public class HostRoot : IHostRoot {
        static void Main(string[] args) {
            string baseAddress = "http://localhost:3339";
            Console.Title = baseAddress + " Enter exit to quit.";
            HttpServer.Start(baseAddress);
            new Windows.ConsoleHandler(() => {
                HttpServer.Stop();
            }).Handle();
            string line = Console.ReadLine();
            while (line != "exit") {
                line = Console.ReadLine();
            }
            HttpServer.Stop();
        }

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
        private readonly object ossClientLocker = new object();
        private void OSSClientInit() {
            DateTime now = DateTime.Now;
            if (_ossClientOn.AddMinutes(10) < now) {
                lock (ossClientLocker) {
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
            return new LiteDatabase($"filename={SpecialPath.ReportDbFileFullName};journal=false");
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
            VirtualRoot.On<UserLoginedEvent>(
                "用户登录成功后广播",
                LogEnum.Console,
                action: message => {
                    Task.Factory.StartNew(() => {
                        Parallel.ForEach(VirtualRoot.IpSet, (ip, state, i) => {
                            try {
                                Client.MinerClientService.AddUserAsync(ip.ToString(), message.User.LoginName, HashUtil.Sha1(message.User.Password), message.User.Description, response=> {
                                    if (response.IsSuccess()) {
                                        Write.DevLine($"向{ip.ToString()}广播登录成功成功", ConsoleColor.Green);
                                    }
                                    else {
                                        Write.DevLine($"向{ip.ToString()}广播登录成功失败：{response?.Description}", ConsoleColor.Red);
                                    }
                                });
                            }
                            catch (Exception e) {
                                Write.DevLine(e.Message);
                            }
                        });
                    });
                });
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

        public DateTime StartedOn { get; private set; } = DateTime.Now;

        public IHostConfig HostConfig { get; private set; }
    }
}

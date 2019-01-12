using Aliyun.OSS;
using LiteDB;
using NTMiner.Data;
using NTMiner.Data.Impl;
using NTMiner.ServiceContracts;
using NTMiner.Services;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace NTMiner {
    public class HostRoot : IHostRoot {
        public static readonly IHostRoot Current = new HostRoot();

        private static OssClient _ossClient = null;

        public static OssClient OssClient {
            get {
                OSSClientInit();
                return _ossClient;
            }
        }

        static HostRoot() {
            OSSClientInit();
        }

        #region OSSClientInit
        private static DateTime _ossClientOn = DateTime.MinValue;
        private static readonly object ossClientLocker = new object();
        private static void OSSClientInit() {
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

        private List<ServiceHost> _serviceHosts = null;

        private HostRoot() {
            this.UserSet = new UserSet(this);
            this.CalcConfigSet = new CalcConfigSet(this);
            this.ClientCoinSnapshotSet = new ClientCoinSnapshotSet(this);
            this.ClientSet = new ClientSet(this);
            this.CoinSnapshotSet = new CoinSnapshotSet(this);
            this.MineWorkSet = new MineWorkSet(this);
            this.MinerGroupSet = new MinerGroupSet(this);
            this.WalletSet = new WalletSet(this);
            this.MineProfileManager = new MineProfileManager(this);
            this.NTMinerFileSet = new NTMinerFileSet(this);

            Windows.App.SetAutoBoot("NTMiner.WcfServer", true);
        }

        public IUserSet UserSet { get; private set; }

        public ICalcConfigSet CalcConfigSet { get; private set; }

        public IClientCoinSnapshotSet ClientCoinSnapshotSet { get; private set; }

        public IClientSet ClientSet { get; private set; }

        public ICoinSnapshotSet CoinSnapshotSet { get; private set; }

        public IMineWorkSet MineWorkSet { get; private set; }

        public IMinerGroupSet MinerGroupSet { get; private set; }

        public IWalletSet WalletSet { get; private set; }

        public IMineProfileManager MineProfileManager { get; private set; }

        public INTMinerFileSet NTMinerFileSet { get; private set; }

        // 创建一个128位数随机数，base64编码后返回
        private static string CreateAesKey() {
            byte[] data = new byte[16];// 128位
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(data);
            }
            return Convert.ToBase64String(data);
        }

        public static string EncryptSerialize<T>(T obj, string pubKey, out string desKey) {
            string json = Global.JsonSerializer.Serialize(obj);
            string key = CreateAesKey();
            desKey = Security.RSAHelper.EncryptString(key, pubKey);
            string result = Security.AESHelper.Encrypt(json, key);
            return result;
        }

        public void Stop() {
            if (_serviceHosts == null) {
                return;
            }
            foreach (var serviceHost in _serviceHosts) {
                serviceHost.Close();
            }
            _serviceHosts = null;
        }

        public DateTime StartedOn { get; private set; } = DateTime.Now;

        public void Start() {
            string baseUrl = $"http://{Global.Localhost}:{Global.ClientPort}/";

            ServiceHost timeServiceHost = new ServiceHost(typeof(TimeServiceImpl));
            timeServiceHost.AddServiceEndpoint(typeof(ITimeService), ChannelFactory.BasicHttpBinding, new Uri(new Uri(baseUrl), nameof(ITimeService)));

            ServiceHost minerServerServiceHost = new ServiceHost(typeof(ControlCenterServiceImpl));
            minerServerServiceHost.AddServiceEndpoint(typeof(IControlCenterService), ChannelFactory.BasicHttpBinding, new Uri(new Uri(baseUrl), nameof(IControlCenterService)));

            ServiceHost mineServerServiceHost = new ServiceHost(typeof(ProfileServiceImpl));
            mineServerServiceHost.AddServiceEndpoint(typeof(IProfileService), ChannelFactory.BasicHttpBinding, new Uri(new Uri(baseUrl), nameof(IProfileService)));

            ServiceHost reportServerServiceHost = new ServiceHost(typeof(ReportServiceImpl));
            reportServerServiceHost.AddServiceEndpoint(typeof(IReportService), ChannelFactory.BasicHttpBinding, new Uri(new Uri(baseUrl), nameof(IReportService)));

            ServiceHost versionServerServiceHost = new ServiceHost(typeof(FileUrlServiceImpl));
            versionServerServiceHost.AddServiceEndpoint(typeof(IFileUrlService), ChannelFactory.BasicHttpBinding, new Uri(new Uri(baseUrl), nameof(IFileUrlService)));

            _serviceHosts = new List<ServiceHost>
            {
                timeServiceHost,
                minerServerServiceHost,
                mineServerServiceHost,
                reportServerServiceHost,
                versionServerServiceHost
            };
            foreach (var serviceHost in _serviceHosts) {
                ServiceMetadataBehavior serviceMetadata = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (serviceMetadata == null) {
                    serviceMetadata = new ServiceMetadataBehavior();
                    serviceHost.Description.Behaviors.Add(serviceMetadata);
                }
                serviceMetadata.HttpGetEnabled = false;

                serviceHost.Open();
            }

            Global.Logger.InfoDebugLine($"服务启动成功: {DateTime.Now}.");
            Global.Logger.InfoDebugLine("服务列表：");
            foreach (var serviceHost in _serviceHosts) {
                foreach (var endpoint in serviceHost.Description.Endpoints) {
                    Global.Logger.InfoDebugLine(endpoint.Address.Uri.ToString());
                }
            }
            StartedOn = DateTime.Now;
        }
    }
}

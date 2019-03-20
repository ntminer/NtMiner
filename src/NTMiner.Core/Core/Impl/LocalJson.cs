using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class LocalJson : IJsonDb {
        public static readonly LocalJson Instance = new LocalJson();

        public static LocalJson NewInstance() {
            return new LocalJson();
        }

        // 私有构造函数不影响序列化反序列化
        private LocalJson() {
            Clear();
        }

        private void Clear() {
            this.CoinKernelProfiles = new CoinKernelProfileData[0];
            this.CoinProfiles = new CoinProfileData[0];
            this.MinerProfile = new MinerProfileData();
            this.MineWork = new MineWorkData();
            this.Pools = new PoolData[0];
            this.PoolProfiles = new PoolProfileData[0];
            this.Wallets = new WalletData[0];
            this.TimeStamp = Timestamp.GetTimestamp();
        }

        private readonly object _locker = new object();
        private bool _inited = false;
        public void Init() {
            if (!_inited) {
                lock (_locker) {
                    if (!_inited) {
                        string localJson = SpecialPath.ReadLocalJsonFile();
                        if (!string.IsNullOrEmpty(localJson)) {
                            try {
                                LocalJson data = VirtualRoot.JsonSerializer.Deserialize<LocalJson>(localJson);
                                this.CoinKernelProfiles = data.CoinKernelProfiles ?? new CoinKernelProfileData[0];
                                this.CoinProfiles = data.CoinProfiles ?? new CoinProfileData[0];
                                this.MinerProfile = data.MinerProfile ?? new MinerProfileData();
                                this.MineWork = data.MineWork ?? new MineWorkData();
                                this.Pools = data.Pools ?? new PoolData[0];
                                this.PoolProfiles = data.PoolProfiles ?? new PoolProfileData[0];
                                this.Wallets = data.Wallets ?? new WalletData[0];
                                this.TimeStamp = data.TimeStamp;
                            }
                            catch (Exception e) {
                                Logger.ErrorDebugLine(e.Message, e);
                            }
                        }
                        else {
                            Clear();
                        }
                        _inited = true;
                    }
                }
            }
        }

        public void ReInit() {
            _inited = false;
            Init();
        }

        public void ReInit(IMineWork work) {
            _inited = true;
            Clear();
            this.MineWork = new MineWorkData(work);
        }

        public bool Exists<T>(Guid key) where T : IDbEntity<Guid> {
            return GetAll<T>().Any(a => a.GetId() == key);
        }

        public T GetByKey<T>(Guid key) where T : IDbEntity<Guid> {
            return GetAll<T>().FirstOrDefault(a => a.GetId() == key);
        }

        public IEnumerable<T> GetAll<T>() where T : IDbEntity<Guid> {
            string typeName = typeof(T).Name;
            switch (typeName) {
                case nameof(CoinKernelProfileData):
                    return this.CoinKernelProfiles.Cast<T>();
                case nameof(CoinProfileData):
                    return this.CoinProfiles.Cast<T>();
                case nameof(PoolData):
                    return this.Pools.Cast<T>();
                case nameof(PoolProfileData):
                    return this.PoolProfiles.Cast<T>();
                case nameof(WalletData):
                    return this.Wallets.Cast<T>();
                default:
                    return new List<T>();
            }
        }

        public ulong TimeStamp { get; set; }

        public CoinKernelProfileData[] CoinKernelProfiles { get; set; }

        public CoinProfileData[] CoinProfiles { get; set; }

        public MinerProfileData MinerProfile { get; set; }

        public MineWorkData MineWork { get; set; }

        public PoolData[] Pools { get; set; }

        public PoolProfileData[] PoolProfiles { get; set; }

        public WalletData[] Wallets { get; set; }
    }
}

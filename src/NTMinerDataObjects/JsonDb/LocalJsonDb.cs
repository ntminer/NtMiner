using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.JsonDb {
    public class LocalJsonDb : ILocalJsonDb {
        public LocalJsonDb() {
            this.CoinKernelProfiles = new CoinKernelProfileData[0];
            this.CoinProfiles = new CoinProfileData[0];
            this.MinerProfile = MinerProfileData.CreateDefaultData(Guid.Empty);
            this.MineWork = new MineWorkData();
            this.Pools = new PoolData[0];
            this.PoolProfiles = new PoolProfileData[0];
            this.Wallets = new WalletData[0];
            this.TimeStamp = Timestamp.GetTimestamp();
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

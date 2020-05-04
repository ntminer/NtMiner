using NTMiner.Core;
using NTMiner.Core.Profile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.JsonDb {
    public class LocalJsonDb : ILocalJsonDb {
        public static LocalJsonDb ConvertFromNTMinerContext() {
            return new LocalJsonDb(NTMinerContext.Instance, new MineWorkData {
                CreatedOn = DateTime.Now,
                Description = "由本机挖矿设置转化而来",
                Id = MineWorkData.SelfMineWorkId,
                ModifiedOn = DateTime.MinValue,
                Name = "缺省作业",
                ServerJsonSha1 = string.Empty
            });
        }

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

        public LocalJsonDb(INTMinerContext root, MineWorkData mineWorkData) {
            var minerProfile = root.MinerProfile;
            CoinProfileData mainCoinProfile = new CoinProfileData().Update(minerProfile.GetCoinProfile(minerProfile.CoinId));
            List<CoinProfileData> coinProfiles = new List<CoinProfileData> { mainCoinProfile };
            List<PoolProfileData> poolProfiles = new List<PoolProfileData>();
            CoinKernelProfileData coinKernelProfile = new CoinKernelProfileData().Update(minerProfile.GetCoinKernelProfile(mainCoinProfile.CoinKernelId));
            PoolProfileData mainCoinPoolProfile = new PoolProfileData().Update(minerProfile.GetPoolProfile(mainCoinProfile.PoolId));
            poolProfiles.Add(mainCoinPoolProfile);
            if (mainCoinProfile.PoolId1 != Guid.Empty) {
                mainCoinPoolProfile = new PoolProfileData().Update(minerProfile.GetPoolProfile(mainCoinProfile.PoolId1));
                poolProfiles.Add(mainCoinPoolProfile);
            }
            if (coinKernelProfile.IsDualCoinEnabled) {
                CoinProfileData dualCoinProfile = new CoinProfileData().Update(minerProfile.GetCoinProfile(coinKernelProfile.DualCoinId));
                coinProfiles.Add(dualCoinProfile);
                PoolProfileData dualCoinPoolProfile = new PoolProfileData().Update(minerProfile.GetPoolProfile(dualCoinProfile.DualCoinPoolId));
                poolProfiles.Add(dualCoinPoolProfile);
            }

            MinerProfile = new MinerProfileData().Update(minerProfile);
            MinerProfile.MinerName = NTKeyword.MinerNameParameterName;
            MineWork = mineWorkData;
            CoinProfiles = coinProfiles.ToArray();
            CoinKernelProfiles = new CoinKernelProfileData[] { coinKernelProfile };
            PoolProfiles = poolProfiles.ToArray();
            Pools = root.ServerContext.PoolSet.AsEnumerable().Where(a => poolProfiles.Any(b => b.PoolId == a.GetId())).Select(a => new PoolData().Update(a)).ToArray();
            Wallets = minerProfile.GetWallets().Select(a => new WalletData().Update(a)).ToArray();
            TimeStamp = Timestamp.GetTimestamp();
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
                case nameof(MinerProfileData):
                    return GetMinerProfiles().Cast<T>();
                case nameof(MineWorkData):
                    return GetMineWorks().Cast<T>();
                default:
                    return new List<T>();
            }
        }

        public long TimeStamp { get; set; }

        public CoinKernelProfileData[] CoinKernelProfiles { get; set; }

        public CoinProfileData[] CoinProfiles { get; set; }

        public MinerProfileData MinerProfile { get; set; }

        private IEnumerable<MinerProfileData> GetMinerProfiles() {
            yield return MinerProfile;
        }

        public MineWorkData MineWork { get; set; }

        private IEnumerable<MineWorkData> GetMineWorks() {
            yield return MineWork;
        }

        public PoolData[] Pools { get; set; }

        public PoolProfileData[] PoolProfiles { get; set; }

        public WalletData[] Wallets { get; set; }
    }
}

using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.JsonDb {
    public class ServerJsonDb : IServerJsonDb {
        public ServerJsonDb() {
            this.Coins = new CoinData[0];
            this.Groups = new GroupData[0];
            this.CoinGroups = new CoinGroupData[0];
            this.CoinKernels = new List<CoinKernelData>();
            this.FileWriters = new List<FileWriterData>();
            this.FragmentWriters = new List<FragmentWriterData>();
            this.Kernels = new List<KernelData>();
            this.Packages = new List<PackageData>();
            this.KernelInputs = new KernelInputData[0];
            this.KernelOutputs = new KernelOutputData[0];
            this.KernelOutputTranslaters = new KernelOutputTranslaterData[0];
            this.Pools = new List<PoolData>();
            this.PoolKernels = new List<PoolKernelData>();
            this.SysDics = new SysDicData[0];
            this.SysDicItems = new SysDicItemData[0];
            this.TimeStamp = Timestamp.GetTimestamp();
        }

        public ServerJsonDb(INTMinerContext root) {
            Coins = root.ServerContext.CoinSet.AsEnumerable().Cast<CoinData>().ToArray();
            // json向后兼容
            foreach (var coin in Coins) {
                if (root.ServerContext.SysDicItemSet.TryGetDicItem(coin.AlgoId, out ISysDicItem dicItem)) {
                    coin.Algo = dicItem.Value;
                }
            }
            Groups = root.ServerContext.GroupSet.AsEnumerable().Cast<GroupData>().ToArray();
            CoinGroups = root.ServerContext.CoinGroupSet.AsEnumerable().Cast<CoinGroupData>().ToArray();
            KernelInputs = root.ServerContext.KernelInputSet.AsEnumerable().Cast<KernelInputData>().ToArray();
            KernelOutputs = root.ServerContext.KernelOutputSet.AsEnumerable().Cast<KernelOutputData>().ToArray();
            KernelOutputTranslaters = root.ServerContext.KernelOutputTranslaterSet.AsEnumerable().Cast<KernelOutputTranslaterData>().ToArray();
            Kernels = root.ServerContext.KernelSet.AsEnumerable().Cast<KernelData>().ToList();
            Packages = root.ServerContext.PackageSet.AsEnumerable().Cast<PackageData>().ToList();
            CoinKernels = root.ServerContext.CoinKernelSet.AsEnumerable().Cast<CoinKernelData>().ToList();
            FileWriters = root.ServerContext.FileWriterSet.AsEnumerable().Cast<FileWriterData>().ToList();
            FragmentWriters = root.ServerContext.FragmentWriterSet.AsEnumerable().Cast<FragmentWriterData>().ToList();
            PoolKernels = root.ServerContext.PoolKernelSet.AsEnumerable().Cast<PoolKernelData>().Where(a => !string.IsNullOrEmpty(a.Args)).ToList();
            Pools = root.ServerContext.PoolSet.AsEnumerable().Cast<PoolData>().ToList();
            SysDicItems = root.ServerContext.SysDicItemSet.AsEnumerable().Cast<SysDicItemData>().ToArray();
            SysDics = root.ServerContext.SysDicSet.AsEnumerable().Cast<SysDicData>().ToArray();
            this.TimeStamp = Timestamp.GetTimestamp();
        }

        /// <summary>
        /// 序列化前消减json的体积，但也不比过分担心json的体积，因为会压缩。但是反序列化json时会耗费cpu，或许这个地方应该优化为某种更节省cpu的数据格式。
        /// </summary>
        public void CutJsonSize() {
            foreach (var coinKernel in this.CoinKernels) {
                if (coinKernel.EnvironmentVariables.Count == 0) {
                    coinKernel.EnvironmentVariables = null;
                }
                if (coinKernel.InputSegments.Count == 0) {
                    coinKernel.InputSegments = null;
                }
                if (coinKernel.FileWriterIds.Count == 0) {
                    coinKernel.FileWriterIds = null;
                }
                if (coinKernel.FragmentWriterIds.Count == 0) {
                    coinKernel.FragmentWriterIds = null;
                }
            }
        }

        public void UnCut() {
            foreach (var coinKernel in this.CoinKernels) {
                if (coinKernel.EnvironmentVariables == null) {
                    coinKernel.EnvironmentVariables = new List<EnvironmentVariable>();
                }
                if (coinKernel.InputSegments == null) {
                    coinKernel.InputSegments = new List<InputSegment>();
                }
                if (coinKernel.FileWriterIds == null) {
                    coinKernel.FileWriterIds = new List<Guid>();
                }
                if (coinKernel.FragmentWriterIds == null) {
                    coinKernel.FragmentWriterIds = new List<Guid>();
                }
            }
        }

        public ServerJsonDb(INTMinerContext root, LocalJsonDb localJsonObj) : this() {
            var minerProfile = root.MinerProfile;
            var mainCoinProfile = minerProfile.GetCoinProfile(minerProfile.CoinId);
            root.ServerContext.CoinKernelSet.TryGetCoinKernel(mainCoinProfile.CoinKernelId, out ICoinKernel coinKernel);
            if (coinKernel == null) {
                return;
            }
            root.ServerContext.KernelSet.TryGetKernel(coinKernel.KernelId, out IKernel kernel);
            if (kernel == null) {
                return;
            }
            var coins = root.ServerContext.CoinSet.AsEnumerable().Cast<CoinData>().Where(a => localJsonObj.CoinProfiles.Any(b => b.CoinId == a.Id)).ToArray();
            var coinGroups = root.ServerContext.CoinGroupSet.AsEnumerable().Cast<CoinGroupData>().Where(a => coins.Any(b => b.Id == a.CoinId)).ToArray();
            var pools = root.ServerContext.PoolSet.AsEnumerable().Cast<PoolData>().Where(a => localJsonObj.PoolProfiles.Any(b => b.PoolId == a.Id)).ToList();

            Coins = coins;
            // json向后兼容
            foreach (var coin in Coins) {
                if (root.ServerContext.SysDicItemSet.TryGetDicItem(coin.AlgoId, out ISysDicItem dicItem)) {
                    coin.Algo = dicItem.Value;
                }
            }
            CoinGroups = coinGroups;
            Pools = pools;
            Groups = root.ServerContext.GroupSet.AsEnumerable().Cast<GroupData>().Where(a => coinGroups.Any(b => b.GroupId == a.Id)).ToArray();
            KernelInputs = root.ServerContext.KernelInputSet.AsEnumerable().Cast<KernelInputData>().Where(a => a.Id == kernel.KernelInputId).ToArray();
            KernelOutputs = root.ServerContext.KernelOutputSet.AsEnumerable().Cast<KernelOutputData>().Where(a => a.Id == kernel.KernelOutputId).ToArray();
            KernelOutputTranslaters = root.ServerContext.KernelOutputTranslaterSet.AsEnumerable().Cast<KernelOutputTranslaterData>().Where(a => a.KernelOutputId == kernel.KernelOutputId).ToArray();
            Kernels = new List<KernelData> { (KernelData)kernel };
            Packages = root.ServerContext.PackageSet.AsEnumerable().Cast<PackageData>().Where(a => a.Name == kernel.Package).ToList();
            CoinKernels = root.ServerContext.CoinKernelSet.AsEnumerable().Cast<CoinKernelData>().Where(a => localJsonObj.CoinKernelProfiles.Any(b => b.CoinKernelId == a.Id)).ToList();
            FileWriters = root.ServerContext.FileWriterSet.AsEnumerable().Cast<FileWriterData>().ToList();// 这个数据没几条就不精简了
            FragmentWriters = root.ServerContext.FragmentWriterSet.AsEnumerable().Cast<FragmentWriterData>().ToList();// 这个数据没几条就不精简了
            PoolKernels = root.ServerContext.PoolKernelSet.AsEnumerable().Cast<PoolKernelData>().Where(a => !string.IsNullOrEmpty(a.Args) && pools.Any(b => b.Id == a.PoolId)).ToList();
            SysDicItems = root.ServerContext.SysDicItemSet.AsEnumerable().Cast<SysDicItemData>().ToArray();
            SysDics = root.ServerContext.SysDicSet.AsEnumerable().Cast<SysDicData>().ToArray();
            TimeStamp = NTMinerContext.ServerJson.TimeStamp;
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
                case nameof(CoinData):
                    return this.Coins.Cast<T>();
                case nameof(GroupData):
                    return this.Groups.Cast<T>();
                case nameof(CoinGroupData):
                    return this.CoinGroups.Cast<T>();
                case nameof(CoinKernelData):
                    return this.CoinKernels.Cast<T>();
                case nameof(FileWriterData):
                    return this.FileWriters.Cast<T>();
                case nameof(FragmentWriterData):
                    return this.FragmentWriters.Cast<T>();
                case nameof(KernelData):
                    return this.Kernels.Cast<T>();
                case nameof(PackageData):
                    return this.Packages.Cast<T>();
                case nameof(KernelInputData):
                    return this.KernelInputs.Cast<T>();
                case nameof(KernelOutputData):
                    return this.KernelOutputs.Cast<T>();
                case nameof(KernelOutputTranslaterData):
                    return this.KernelOutputTranslaters.Cast<T>();
                case nameof(PoolData):
                    return this.Pools.Cast<T>();
                case nameof(PoolKernelData):
                    return this.PoolKernels.Cast<T>();
                case nameof(SysDicData):
                    return this.SysDics.Cast<T>();
                case nameof(SysDicItemData):
                    return this.SysDicItems.Cast<T>();
                default:
                    return new List<T>();
            }
        }

        // List<T>类型暗示系统可能会根据内核品牌或矿池品牌或作业做过滤

        public long TimeStamp { get; set; }

        public CoinData[] Coins { get; set; }

        public GroupData[] Groups { get; set; }

        public CoinGroupData[] CoinGroups { get; set; }

        public KernelInputData[] KernelInputs { get; set; }

        public KernelOutputData[] KernelOutputs { get; set; }

        public KernelOutputTranslaterData[] KernelOutputTranslaters { get; set; }

        public List<KernelData> Kernels { get; set; }

        public List<PackageData> Packages { get; set; }

        public List<CoinKernelData> CoinKernels { get; set; }

        public List<FileWriterData> FileWriters { get; set; }

        public List<FragmentWriterData> FragmentWriters { get; set; }

        public List<PoolKernelData> PoolKernels { get; set; }

        public List<PoolData> Pools { get; set; }

        public SysDicData[] SysDics { get; set; }

        public SysDicItemData[] SysDicItems { get; set; }
    }
}

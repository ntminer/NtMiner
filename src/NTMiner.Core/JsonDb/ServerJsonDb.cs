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
            this.Kernels = new List<KernelData>();
            this.KernelInputs = new KernelInputData[0];
            this.KernelOutputs = new KernelOutputData[0];
            this.KernelOutputFilters = new KernelOutputFilterData[0];
            this.KernelOutputTranslaters = new KernelOutputTranslaterData[0];
            this.Pools = new PoolData[0];
            this.PoolKernels = new List<PoolKernelData>();
            this.SysDics = new SysDicData[0];
            this.SysDicItems = new SysDicItemData[0];
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
                case nameof(CoinData):
                    return this.Coins.Cast<T>();
                case nameof(GroupData):
                    return this.Groups.Cast<T>();
                case nameof(CoinGroupData):
                    return this.CoinGroups.Cast<T>();
                case nameof(CoinKernelData):
                    return this.CoinKernels.Cast<T>();
                case nameof(KernelData):
                    return this.Kernels.Cast<T>();
                case nameof(KernelInputData):
                    return this.KernelInputs.Cast<T>();
                case nameof(KernelOutputData):
                    return this.KernelOutputs.Cast<T>();
                case nameof(KernelOutputTranslaterData):
                    return this.KernelOutputTranslaters.Cast<T>();
                case nameof(KernelOutputFilterData):
                    return this.KernelOutputFilters.Cast<T>();
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

        public ulong TimeStamp { get; set; }

        public CoinData[] Coins { get; set; }

        public GroupData[] Groups { get; set; }

        public CoinGroupData[] CoinGroups { get; set; }

        public KernelInputData[] KernelInputs { get; set; }

        public KernelOutputData[] KernelOutputs { get; set; }

        public KernelOutputTranslaterData[] KernelOutputTranslaters { get; set; }

        public KernelOutputFilterData[] KernelOutputFilters { get; set; }

        public List<KernelData> Kernels { get; set; }

        public List<CoinKernelData> CoinKernels { get; set; }

        public List<PoolKernelData> PoolKernels { get; set; }

        public PoolData[] Pools { get; set; }

        public SysDicData[] SysDics { get; set; }

        public SysDicItemData[] SysDicItems { get; set; }
    }
}

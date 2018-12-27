using NTMiner.Core.Kernels.Impl;
using NTMiner.Core.SysDics.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class ServerJson {
        public static void Export() {
            INTMinerRoot root = NTMinerRoot.Current;
            ServerJson data = new ServerJson() {
                CoinKernels = root.CoinKernelSet.Cast<CoinKernelData>().ToList(),
                Coins = root.CoinSet.Cast<CoinData>().ToList(),
                Groups = root.GroupSet.Cast<GroupData>().ToList(),
                CoinGroups = root.CoinGroupSet.Cast<CoinGroupData>().ToList(),
                KernelOutputFilters = root.KernelOutputFilterSet.Cast<KernelOutputFilterData>().ToList(),
                KernelOutputTranslaters = root.KernelOutputTranslaterSet.Cast<KernelOutputTranslaterData>().ToList(),
                Kernels = root.KernelSet.Cast<KernelData>().ToList(),
                Pools = root.PoolSet.Cast<PoolData>().ToList(),
                SysDicItems = root.SysDicItemSet.Cast<SysDicItemData>().ToList(),
                SysDics = root.SysDicSet.Cast<SysDicData>().ToList()
            };
            string json = Global.JsonSerializer.Serialize(data);
            File.WriteAllText(SpecialPath.ServerJsonFileFullName, json);
        }

        public ServerJson() {
            this.Coins = new List<CoinData>();
            this.Groups = new List<GroupData>();
            this.CoinGroups = new List<CoinGroupData>();
            this.CoinKernels = new List<CoinKernelData>();
            this.Kernels = new List<KernelData>();
            this.KernelOutputFilters = new List<KernelOutputFilterData>();
            this.KernelOutputTranslaters = new List<KernelOutputTranslaterData>();
            this.Pools = new List<PoolData>();
            this.SysDics = new List<SysDicData>();
            this.SysDicItems = new List<SysDicItemData>();
            this.TimeStamp = Global.GetTimestamp();
        }

        public bool Exists<T>(Guid key) where T : IDbEntity<Guid> {
            return GetAll<T>().Any(a => a.GetId() == key);
        }

        public T GetByKey<T>(Guid key) where T : IDbEntity<Guid> {
            return GetAll<T>().FirstOrDefault(a => a.GetId() == key);
        }

        public IList<T> GetAll<T>() where T : IDbEntity<Guid> {
            string typeName = typeof(T).Name;
            switch (typeName) {
                case nameof(CoinData):
                    return (IList<T>)this.Coins;
                case nameof(GroupData):
                    return (IList<T>)this.Groups;
                case nameof(CoinGroupData):
                    return (IList<T>)this.CoinGroups;
                case nameof(CoinKernelData):
                    return (IList<T>)this.CoinKernels;
                case nameof(KernelData):
                    return (IList<T>)this.Kernels;
                case nameof(KernelOutputTranslaterData):
                    return (IList<T>)this.KernelOutputTranslaters;
                case nameof(KernelOutputFilterData):
                    return (IList<T>)this.KernelOutputFilters;
                case nameof(PoolData):
                    return (IList<T>)this.Pools;
                case nameof(SysDicData):
                    return (IList<T>)this.SysDics;
                case nameof(SysDicItemData):
                    return (IList<T>)this.SysDicItems;
                default:
                    return new List<T>();
            }
        }

        public ulong TimeStamp { get; set; }

        public List<CoinData> Coins { get; set; }

        public List<GroupData> Groups { get; set; }

        public List<CoinGroupData> CoinGroups { get; set; }

        public List<CoinKernelData> CoinKernels { get; set; }

        public List<KernelData> Kernels { get; set; }

        public List<KernelOutputTranslaterData> KernelOutputTranslaters { get; set; }

        public List<KernelOutputFilterData> KernelOutputFilters { get; set; }

        public List<PoolData> Pools { get; set; }

        public List<SysDicData> SysDics { get; set; }

        public List<SysDicItemData> SysDicItems { get; set; }
    }
}

using NTMiner.Core.Kernels.Impl;
using NTMiner.Core.SysDics.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class ServerJson {
        public static readonly ServerJson Instance = new ServerJson();

        public static string Export() {
            INTMinerRoot root = NTMinerRoot.Current;
            ServerJson data = new ServerJson() {
                CoinKernels = root.CoinKernelSet.Cast<CoinKernelData>().ToArray(),
                Coins = root.CoinSet.Cast<CoinData>().ToArray(),
                Groups = root.GroupSet.Cast<GroupData>().ToArray(),
                CoinGroups = root.CoinGroupSet.Cast<CoinGroupData>().ToArray(),
                KernelInputs = root.KernelInputSet.Cast<KernelInputData>().ToArray(),
                KernelOutputs = root.KernelOutputSet.Cast<KernelOutputData>().ToArray(),
                KernelOutputFilters = root.KernelOutputFilterSet.Cast<KernelOutputFilterData>().ToArray(),
                KernelOutputTranslaters = root.KernelOutputTranslaterSet.Cast<KernelOutputTranslaterData>().ToArray(),
                Kernels = root.KernelSet.Cast<KernelData>().ToArray(),
                Pools = root.PoolSet.Cast<PoolData>().ToArray(),
                PoolKernels = root.PoolKernelSet.Cast<PoolKernelData>().Where(a => !string.IsNullOrEmpty(a.Args)).ToArray(),
                SysDicItems = root.SysDicItemSet.Cast<SysDicItemData>().ToArray(),
                SysDics = root.SysDicSet.Cast<SysDicData>().ToArray()
            };
            string json = VirtualRoot.JsonSerializer.Serialize(data);
            File.WriteAllText(AssemblyInfo.ServerVersionJsonFileFullName, json);
            return Path.GetFileName(AssemblyInfo.ServerVersionJsonFileFullName);
        }

        // 私有构造函数不影响序列化反序列化
        private ServerJson() {
            this.Coins = new CoinData[0];
            this.Groups = new GroupData[0];
            this.CoinGroups = new CoinGroupData[0];
            this.CoinKernels = new CoinKernelData[0];
            this.Kernels = new KernelData[0];
            this.KernelInputs = new KernelInputData[0];
            this.KernelOutputs = new KernelOutputData[0];
            this.KernelOutputFilters = new KernelOutputFilterData[0];
            this.KernelOutputTranslaters = new KernelOutputTranslaterData[0];
            this.Pools = new PoolData[0];
            this.PoolKernels = new PoolKernelData[0];
            this.SysDics = new SysDicData[0];
            this.SysDicItems = new SysDicItemData[0];
            this.TimeStamp = Timestamp.GetTimestamp();
        }

        private readonly object _locker = new object();
        private bool _inited = false;
        public void Init(string rawJson) {
            if (!_inited) {
                lock (_locker) {
                    if (!_inited) {
                        if (!string.IsNullOrEmpty(rawJson)) {
                            try {
                                ServerJson data = VirtualRoot.JsonSerializer.Deserialize<ServerJson>(rawJson);
                                this.Coins = data.Coins ?? new CoinData[0];
                                this.Groups = data.Groups ?? new GroupData[0];
                                this.CoinGroups = data.CoinGroups ?? new CoinGroupData[0];
                                this.CoinKernels = data.CoinKernels ?? new CoinKernelData[0];
                                this.Kernels = data.Kernels ?? new KernelData[0];
                                this.KernelInputs = data.KernelInputs ?? new KernelInputData[0];
                                this.KernelOutputs = data.KernelOutputs ?? new KernelOutputData[0];
                                this.KernelOutputFilters = data.KernelOutputFilters ?? new KernelOutputFilterData[0];
                                this.KernelOutputTranslaters = data.KernelOutputTranslaters ?? new KernelOutputTranslaterData[0];
                                this.Pools = data.Pools ?? new PoolData[0];
                                this.PoolKernels = data.PoolKernels ?? new PoolKernelData[0];
                                this.SysDics = data.SysDics ?? new SysDicData[0];
                                this.SysDicItems = data.SysDicItems ?? new SysDicItemData[0];
                                this.TimeStamp = data.TimeStamp;
                                File.WriteAllText(SpecialPath.ServerJsonFileFullName, rawJson);
                            }
                            catch (Exception e) {
                                Logger.ErrorDebugLine(e.Message, e);
                            }
                        }
                        _inited = true;
                    }
                }
            }
        }

        public void ReInit(string rawJson) {
            _inited = false;
            Init(rawJson);
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

        public KernelData[] Kernels { get; set; }

        public CoinKernelData[] CoinKernels { get; set; }

        public PoolData[] Pools { get; set; }

        public PoolKernelData[] PoolKernels { get; set; }

        public SysDicData[] SysDics { get; set; }

        public SysDicItemData[] SysDicItems { get; set; }
    }
}

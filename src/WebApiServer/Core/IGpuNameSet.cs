using NTMiner.Core.Gpus;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IGpuNameSet {
        /// <summary>
        /// 该集合的成员是异步从redis中加载数据初始化的，所以有了这个IsReadied属性。
        /// </summary>
        bool IsReadied { get; }
        void AddCount(GpuType gpuType, string gpuName, ulong gpuTotalMemory);
        /// <summary>
        /// 注意：整个GpuName是个值对象，且GpuName作为key存储为hash表，所以不支持Update
        /// </summary>
        /// <param name="gpuName"></param>
        void Set(GpuName gpuName);
        void Remove(GpuName gpuName);
        List<GpuNameCount> QueryGpuNameCounts(QueryGpuNameCountsRequest query, out int total);
        List<GpuName> GetAllGpuNames();
    }
}

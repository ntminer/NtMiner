using NTMiner.Gpus;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IGpuNameSet {
        /// <summary>
        /// 该集合的成员是异步从redis中加载数据初始化的，所以有了这个IsReadied属性。
        /// </summary>
        bool IsReadied { get; }
        void AddCount(GpuType gpuType, string gpuName, ulong gpuTotalMemory);
        List<GpuNameCount> QueryGpuNameCounts(QueryGpuNameCountsRequest query, out int total);
    }
}

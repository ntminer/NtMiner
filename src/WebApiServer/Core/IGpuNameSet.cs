using NTMiner.Core.Gpus;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IGpuNameSet {
        /// <summary>
        /// 该集合的成员是异步从redis中加载数据初始化的，所以有了这个IsReadied属性。
        /// </summary>
        bool IsReadied { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gpuName"></param>
        /// <param name="gpuTotalMemory">字节</param>
        void Add(string gpuName, ulong gpuTotalMemory);
        IEnumerable<IGpuName> AsEnumerable();
    }
}

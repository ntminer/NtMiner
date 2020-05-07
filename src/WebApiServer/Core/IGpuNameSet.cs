using NTMiner.Core.Gpus;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IGpuNameSet {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gpuName"></param>
        /// <param name="gpuTotalMemory">字节</param>
        void Add(string gpuName, ulong gpuTotalMemory);
        IEnumerable<IGpuName> AsEnumerable();
    }
}

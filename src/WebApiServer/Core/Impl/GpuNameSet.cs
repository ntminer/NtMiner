using NTMiner.Core.Gpus;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class GpuNameSet : IGpuNameSet {
        private readonly HashSet<GpuName> _hashSet = new HashSet<GpuName>();

        public GpuNameSet() {
        }

        /// <summary>
        /// 如果显存小于2G会被忽略
        /// </summary>
        /// <param name="gpuName"></param>
        /// <param name="gpuTotalMemory"></param>
        public void Add(string gpuName, ulong gpuTotalMemory) {
            if (string.IsNullOrEmpty(gpuName) || gpuTotalMemory < 2 * NTKeyword.ULongG) {
                return;
            }
            _hashSet.Add(new GpuName {
                Name = gpuName,
                TotalMemory = gpuTotalMemory
            });
        }

        public IEnumerable<IGpuName> AsEnumerable() {
            return _hashSet;
        }
    }
}

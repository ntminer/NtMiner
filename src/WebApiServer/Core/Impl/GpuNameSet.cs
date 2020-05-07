using NTMiner.Core.Gpus;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class GpuNameSet : IGpuNameSet {
        private readonly HashSet<GpuName> _hashSet = new HashSet<GpuName>();
        private readonly HashSet<GpuName> _toSaves = new HashSet<GpuName>();

        public GpuNameSet() {
        }

        /// <summary>
        /// 如果显存小于2G会被忽略
        /// </summary>
        /// <param name="gpuName"></param>
        /// <param name="gpuTotalMemory"></param>
        public void Add(string gpuName, ulong gpuTotalMemory) {
            if (string.IsNullOrEmpty(gpuName) || !GpuName.IsValidTotalMemory(gpuTotalMemory)) {
                return;
            }
            var item = new GpuName {
                Name = gpuName,
                TotalMemory = gpuTotalMemory
            };
            bool isNew = _hashSet.Add(item);
            if (isNew) {
                _toSaves.Add(item);
            }
        }

        public IEnumerable<IGpuName> AsEnumerable() {
            return _hashSet;
        }
    }
}

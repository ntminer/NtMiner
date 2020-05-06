using NTMiner.Core.Gpus;
using NTMiner.Core.Redis;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class GpuNameSet : IGpuNameSet {
        private readonly HashSet<GpuName> _hashSet = new HashSet<GpuName>();

        public bool IsReadied {
            get; private set;
        }

        public GpuNameSet(IGpuNameRedis userRedis) {
            userRedis.GetAllAsync().ContinueWith(t => {
                foreach (var item in t.Result) {
                    _hashSet.Add(item);
                }
                IsReadied = true;
                Write.UserOk("Gpu名称集就绪");
                VirtualRoot.RaiseEvent(new GpuNameSetInitedEvent());
            });
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

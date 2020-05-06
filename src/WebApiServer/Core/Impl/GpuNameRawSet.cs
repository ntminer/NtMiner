using NTMiner.Core.Gpus;
using NTMiner.Core.Redis;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class GpuNameRawSet : IGpuNameRawSet {
        private readonly HashSet<GpuName> _hashSet = new HashSet<GpuName>();
        private readonly HashSet<GpuName> _toSaves = new HashSet<GpuName>();

        public bool IsReadied {
            get; private set;
        }

        public GpuNameRawSet(IGpuNameRedis gpuNameRedis) {
            gpuNameRedis.GetAllRawAsync().ContinueWith(t => {
                foreach (var item in t.Result) {
                    _hashSet.Add(item);
                }
                IsReadied = true;
                Write.UserOk("Gpu名称集就绪");
                VirtualRoot.RaiseEvent(new GpuNameSetInitedEvent());
            });
            VirtualRoot.AddEventPath<Per1MinuteEvent>("周期将新发现的GpuName持久化到redis", LogEnum.DevConsole, action: message => {
                if (_toSaves.Count != 0) {
                    gpuNameRedis.SetRawAsync(_toSaves.ToList());
                    _toSaves.Clear();
                }
            }, this.GetType());
        }

        /// <summary>
        /// 如果显存小于2G会被忽略
        /// </summary>
        /// <param name="gpuName"></param>
        /// <param name="gpuTotalMemory"></param>
        public void Add(string gpuName, ulong gpuTotalMemory) {
            if (!IsReadied) {
                return;
            }
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

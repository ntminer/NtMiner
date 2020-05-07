using NTMiner.Core.Gpus;
using NTMiner.Core.Redis;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class GpuNameSet : IGpuNameSet {
        private readonly Dictionary<string, GpuNameCount> _dic = new Dictionary<string, GpuNameCount>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<GpuName> _gpuNameSet = new HashSet<GpuName>();

        private readonly IGpuNameRedis _gpuNameRedis;
        public GpuNameSet(IGpuNameRedis gpuNameRedis) {
            _gpuNameRedis = gpuNameRedis;
        }

        public void Set(GpuName gpuName) {
            if (gpuName == null || !gpuName.IsValid()) {
                return;
            }
            bool isNew = _gpuNameSet.Add(gpuName);
            if (isNew) {
                _gpuNameRedis.SetAsync(gpuName);
            }
        }

        public void Remove(GpuName gpuName) {
            if (gpuName == null || !gpuName.IsValid()) {
                return;
            }
            _gpuNameSet.Remove(gpuName);
            _gpuNameRedis.DeleteAsync(gpuName);
        }

        public void AddCount(GpuType gpuType, string gpuName, ulong gpuTotalMemory) {
            if (gpuType == GpuType.Empty || string.IsNullOrEmpty(gpuName) || !GpuName.IsValidTotalMemory(gpuTotalMemory)) {
                return;
            }
            string key = GpuName.Format(gpuType, gpuName, gpuTotalMemory);
            if (_dic.TryGetValue(key, out GpuNameCount gpuNameCount)) {
                gpuNameCount.Count++;
            }
            else {
                gpuNameCount = GpuNameCount.Create(new GpuName {
                    GpuType = gpuType,
                    Name = gpuName,
                    TotalMemory = gpuTotalMemory
                });
                _dic.Add(key, gpuNameCount);
            }
        }

        public IEnumerable<GpuNameCount> GetGpuNameCounts() {
            return _dic.Values;
        }

        public IEnumerable<GpuName> AsEnumerable() {
            return _gpuNameSet;
        }
    }
}

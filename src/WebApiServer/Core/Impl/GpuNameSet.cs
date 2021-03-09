using NTMiner.Gpus;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class GpuNameSet : IGpuNameSet {
        private readonly Dictionary<GpuName, int> _gpuNameCountDic = new Dictionary<GpuName, int>();

        public GpuNameSet() {
            VirtualRoot.BuildEventPath<ClientSetInitedEvent>("矿机列表初始化后计算显卡名称集合", LogEnum.DevConsole, path: message => {
                Init();
            }, this.GetType());
            VirtualRoot.BuildEventPath<Per10MinuteEvent>("周期刷新显卡名称集合", LogEnum.DevConsole, path: message => {
                Init();
            }, this.GetType());
        }

        private void Init() {
            _gpuNameCountDic.Clear();
            foreach (var clientData in AppRoot.ClientDataSet.AsEnumerable().ToArray()) {
                foreach (var gpuSpeedData in clientData.GpuTable) {
                    AddCount(clientData.GpuType, gpuSpeedData.Name, gpuSpeedData.TotalMemory);
                }
            }
        }

        public void AddCount(GpuType gpuType, string gpuName, ulong gpuTotalMemory) {
            if (gpuType == GpuType.Empty || string.IsNullOrEmpty(gpuName) || !GpuName.IsValidTotalMemory(gpuTotalMemory)) {
                return;
            }
            GpuName key = new GpuName {
                TotalMemory = gpuTotalMemory,
                Name = gpuName,
                GpuType = gpuType
            };
            if (_gpuNameCountDic.TryGetValue(key, out int count)) {
                _gpuNameCountDic[key] = count + 1;
            }
            else {
                _gpuNameCountDic.Add(key, 1);
            }
        }

        public List<GpuNameCount> QueryGpuNameCounts(QueryGpuNameCountsRequest query, out int total) {
            List<KeyValuePair<GpuName, int>> list = new List<KeyValuePair<GpuName, int>>();
            bool isFilterByKeyword = !string.IsNullOrEmpty(query.Keyword);
            if (isFilterByKeyword) {
                foreach (var item in _gpuNameCountDic) {
                    if (item.Key.Name.Contains(query.Keyword)) {
                        list.Add(item);
                    }
                }
            }
            else {
                list.AddRange(_gpuNameCountDic);
            }
            total = list.Count;
            return list.OrderBy(a => a.Key.Name).Take(paging: query).Select(a => new GpuNameCount {
                Name = a.Key.Name,
                Count = a.Value,
                GpuType = a.Key.GpuType,
                TotalMemory = a.Key.TotalMemory
            }).ToList();
        }
    }
}

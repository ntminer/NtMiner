using NTMiner.MinerServer;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Gpus {
    public static class GpuSetExtension {
        public static bool GetIsUseDevice(this IGpuSet gpuSet, int gpuIndex) {
            if (gpuIndex < 0 || gpuIndex >= gpuSet.Count) {
                return false;
            }
            int[] devices = GetUseDevices(gpuSet);
            return devices.Contains(gpuIndex);
        }

        public static void SetIsUseDevice(this IGpuSet gpuSet, int gpuIndex, bool isUse) {
            List<int> devices = GetUseDevices(gpuSet).ToList();
            if (!isUse) {
                devices.Remove(gpuIndex);
            }
            else if (!devices.Contains(gpuIndex)) {
                devices.Add(gpuIndex);
            }
            devices = devices.OrderBy(a => a).ToList();
            SetUseDevices(gpuSet, devices);
        }

        public static int[] GetUseDevices(this IGpuSet gpuSet) {
            List<int> list = new List<int>();
            if (VirtualRoot.LocalAppSettingSet.TryGetAppSetting(NTKeyword.UseDevicesAppSettingKey, out IAppSetting setting) && setting.Value != null) {
                string[] parts = setting.Value.ToString().Split(',');
                foreach (var part in parts) {
                    if (int.TryParse(part, out int index)) {
                        list.Add(index);
                    }
                }
            }
            // 全不选等于全选
            if (list.Count == 0) {
                foreach (var gpu in gpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    list.Add(gpu.Index);
                }
            }
            return list.ToArray();
        }

        private static void SetUseDevices(IGpuSet gpuSet, List<int> gpuIndexes) {
            // 全选等于全不选，所以存储空值就可以了，而且可以避免新插显卡问题
            if (gpuIndexes.Count != 0 && gpuIndexes.Count == gpuSet.Count) {
                gpuIndexes = new List<int>();
            }
            AppSettingData appSettingData = new AppSettingData() {
                Key = NTKeyword.UseDevicesAppSettingKey,
                Value = string.Join(",", gpuIndexes)// 存逗号分隔的字符串，因为litedb处理List、Array有问题
            };
            VirtualRoot.Execute(new SetLocalAppSettingCommand(appSettingData));
        }
    }
}

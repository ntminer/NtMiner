using NTMiner.MinerServer;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Gpus {
    public static class GpuSetExtension {
        public static bool GetIsUseDevice(this IGpuSet gpuSet, int gpuIndex) {
            if (gpuIndex < 0 || gpuIndex >= gpuSet.Count) {
                return false;
            }
            List<int> devices = GetUseDevices(gpuSet);
            return devices.Contains(gpuIndex);
        }

        public static void SetIsUseDevice(this IGpuSet gpuSet, int gpuIndex, bool isUse) {
            List<int> devices = GetUseDevices(gpuSet);
            if (!isUse) {
                devices.Remove(gpuIndex);
            }
            else if (!devices.Contains(gpuIndex)) {
                devices.Add(gpuIndex);
            }
            devices = devices.OrderBy(a => a).ToList();
            SetUseDevices(gpuSet, devices);
        }

        public static List<int> GetUseDevices(this IGpuSet gpuSet) {
            List<int> list = new List<int>();
            if (NTMinerRoot.Instance.LocalAppSettingSet.TryGetAppSetting("UseDevices", out IAppSetting setting) && setting.Value != null) {
                string[] parts = setting.Value.ToString().Split(',');
                foreach (var part in parts) {
                    if (int.TryParse(part, out int index)) {
                        list.Add(index);
                    }
                }
            }
            if (list.Count == 0) {
                foreach (var gpu in gpuSet) {
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        continue;
                    }
                    list.Add(gpu.Index);
                }
            }
            return list;
        }

        private static void SetUseDevices(IGpuSet gpuSet, List<int> gpuIndexes) {
            if (gpuIndexes.Count != 0 && gpuIndexes.Count == gpuSet.Count) {
                gpuIndexes = new List<int>();
            }
            AppSettingData appSettingData = new AppSettingData() {
                Key = "UseDevices",
                Value = string.Join(",", gpuIndexes)// 存逗号分隔的字符串，因为litedb处理List、Array有问题
            };
            VirtualRoot.Execute(new ChangeLocalAppSettingCommand(appSettingData));
        }

        public static bool Has20NCard(this IGpuSet gpuSet) {
            if (gpuSet.GpuType == GpuType.NVIDIA) {
                if (gpuSet.Any(a => Is20NCard(a.Name))) {
                    return true;
                }
            }
            return false;
        }
        private static bool Is20NCard(string cardName) {
            if (string.IsNullOrEmpty(cardName)) {
                return false;
            }
            string[] nv20Cards = new string[] { "2060", "2070", "2080" };
            foreach (var nv20 in nv20Cards) {
                if (cardName.Contains(nv20)) {
                    return true;
                }
            }
            return false;
        }
    }
}

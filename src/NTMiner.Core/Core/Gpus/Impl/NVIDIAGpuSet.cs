using NTMiner.Gpus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Core.Gpus.Impl {
    internal class NVIDIAGpuSet : IGpuSet {
        private readonly Dictionary<int, Gpu> _gpus = new Dictionary<int, Gpu>() {
            {
                NTMinerRoot.GpuAllId, Gpu.GpuAll
            }
        };

        private readonly INTMinerRoot _root;
        private readonly string _driverVersion;

        public int Count {
            get {
                return _gpus.Count - 1;
            }
        }

        private readonly NvapiHelper _nvapiHelper = new NvapiHelper();
        private readonly NvmlHelper _nvmlHelper = new NvmlHelper();
        public NVIDIAGpuSet(INTMinerRoot root) {
            _root = root;
            this.OverClock = new NVIDIAOverClock(_nvapiHelper);
            this.Properties = new List<GpuSetProperty>();
            var gpus = _nvmlHelper.GetGpus();
            if (gpus.Count > 0) {
                foreach (var item in gpus) {
                    var gpu = Gpu.Create(item.GpuIndex, item.BusId.ToString(), item.Name);
                    gpu.TotalMemory = item.TotalMemory;
                    _gpus.Add(item.GpuIndex, gpu);
                }
                _nvmlHelper.GetVersion(out _driverVersion, out string nvmlVersion);
                this.Properties.Add(new GpuSetProperty(GpuSetProperty.DRIVER_VERSION, "驱动版本", _driverVersion));
                try {
                    if (double.TryParse(_driverVersion, out double driverVersionNum)) {
                        var item = root.SysDicItemSet.GetSysDicItems("CudaVersion")
                            .Select(a => new { Version = double.Parse(a.Value), a })
                            .OrderByDescending(a => a.Version)
                            .FirstOrDefault(a => driverVersionNum >= a.Version);
                        if (item != null) {
                            this.Properties.Add(new GpuSetProperty("CudaVersion", "Cuda版本", item.a.Code));
                        }
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
                this.Properties.Add(new GpuSetProperty("NVMLVersion", "NVML版本", nvmlVersion));
                Dictionary<string, string> kvs = new Dictionary<string, string> {
                    {"CUDA_DEVICE_ORDER","PCI_BUS_ID" }
                };
                foreach (var kv in kvs) {
                    var property = new GpuSetProperty(kv.Key, kv.Key, kv.Value);
                    this.Properties.Add(property);
                }
                Task.Factory.StartNew(() => {
                    foreach (var gpu in _gpus.Values) {
                        OverClock.RefreshGpuState(gpu.Index);
                    }
                    // 这里会耗时5秒
                    foreach (var kv in kvs) {
                        Environment.SetEnvironmentVariable(kv.Key, kv.Value);
                    }
                });
            }
        }

        public void LoadGpuState() {
            foreach (Gpu gpu in _gpus.Values) {
                int i = gpu.Index;
                if (i == NTMinerRoot.GpuAllId) {
                    continue;
                }
                uint power = _nvmlHelper.GetPowerUsage(i);
                uint temp = _nvmlHelper.GetTemperature(i);
                uint fanSpeed = _nvapiHelper.GetCooler(gpu.GetBusId());
                bool isChanged = gpu.Temperature != temp || gpu.PowerUsage != power || gpu.FanSpeed != fanSpeed;
                gpu.Temperature = (int)temp;
                gpu.PowerUsage = power;
                gpu.FanSpeed = fanSpeed;

                if (isChanged) {
                    VirtualRoot.Happened(new GpuStateChangedEvent(gpu));
                }
            }
        }

        public GpuType GpuType {
            get {
                return GpuType.NVIDIA;
            }
        }

        public string DriverVersion {
            get { return _driverVersion; }
        }

        public bool TryGetGpu(int index, out IGpu gpu) {
            bool r = _gpus.TryGetValue(index, out Gpu g);
            gpu = g;
            return r;
        }

        public List<GpuSetProperty> Properties { get; private set; }

        public IOverClock OverClock { get; private set; }

        public string GetProperty(string key) {
            GpuSetProperty item = this.Properties.FirstOrDefault(a => a.Code == key);
            if (item == null || item.Value == null) {
                return string.Empty;
            }
            return item.Value.ToString();
        }

        public IEnumerator<IGpu> GetEnumerator() {
            return _gpus.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _gpus.Values.GetEnumerator();
        }
    }
}

using NTMiner.Gpus;
using System;
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
        private readonly Version _driverVersion;

        public int Count {
            get {
                return _gpus.Count - 1;
            }
        }

        private readonly NvapiHelper _nvapiHelper = new NvapiHelper();
        private readonly NvmlHelper _nvmlHelper = new NvmlHelper();
        public NVIDIAGpuSet(INTMinerRoot root) {
            _root = root;
            this.OverClock = new GpuOverClock(_nvapiHelper);
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
                    var item = root.ServerContext.SysDicItemSet.GetSysDicItems(NTKeyword.CudaVersionSysDicCode)
                        .Select(a => new { Version = double.Parse(a.Value), a })
                        .OrderByDescending(a => a.Version)
                        .FirstOrDefault(a => _driverVersion.Major >= a.Version);
                    if (item != null) {
                        this.Properties.Add(new GpuSetProperty(NTKeyword.CudaVersionSysDicCode, "Cuda版本", item.a.Code));
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
                    OverClock.RefreshGpuState(NTMinerRoot.GpuAllId);
                    // 这里会耗时5秒
                    foreach (var kv in kvs) {
                        Environment.SetEnvironmentVariable(kv.Key, kv.Value);
                    }
                });
            }
        }

        public void LoadGpuState() {
            for (int i = 0; i < Count; i++) {
                LoadGpuState(i);
            }
        }

        public void LoadGpuState(int gpuIndex) {
            if (gpuIndex == NTMinerRoot.GpuAllId) {
                return;
            }
            var gpu = _gpus[gpuIndex];
            uint power = _nvmlHelper.GetPowerUsage(gpuIndex);
            uint temp = _nvmlHelper.GetTemperature(gpuIndex);
            uint fanSpeed;
            if (!_nvapiHelper.GetFanSpeed(gpu.GetOverClockId(), out fanSpeed)) {
                fanSpeed = _nvmlHelper.GetFanSpeed(gpuIndex);
            }
            bool isChanged = gpu.Temperature != temp || gpu.PowerUsage != power || gpu.FanSpeed != fanSpeed;
            gpu.Temperature = (int)temp;
            gpu.PowerUsage = power;
            gpu.FanSpeed = fanSpeed;

            if (isChanged) {
                VirtualRoot.RaiseEvent(new GpuStateChangedEvent(gpu));
            }
        }

        public GpuType GpuType {
            get {
                return GpuType.NVIDIA;
            }
        }

        public Version DriverVersion {
            get { return _driverVersion; }
        }

        public bool TryGetGpu(int index, out IGpu gpu) {
            bool r = _gpus.TryGetValue(index, out Gpu g);
            gpu = g;
            return r;
        }

        public List<GpuSetProperty> Properties { get; private set; }

        public IOverClock OverClock { get; private set; }

        public IEnumerable<IGpu> AsEnumerable() {
            return _gpus.Values;
        }
    }
}

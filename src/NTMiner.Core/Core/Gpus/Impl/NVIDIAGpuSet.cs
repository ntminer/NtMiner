using NTMiner.Core.Gpus.Nvml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NTMiner.Core.Gpus.Impl {
    internal class NVIDIAGpuSet : IGpuSet {
        private readonly Dictionary<int, IGpu> _gpus = new Dictionary<int, IGpu>() {
            { Gpu.Total.Index, Gpu.Total }
        };
        private readonly nvmlDevice[] _nvmlDevices = new nvmlDevice[0];

        private readonly INTMinerRoot _root;

        public int Count {
            get {
                return _gpus.Count - 1;
            }
        }

        private NVIDIAGpuSet() { }

        private bool _isNvmlInited = false;
        public NVIDIAGpuSet(INTMinerRoot root) {
            _root = root;
            this.Properties = new List<GpuSetProperty>();
            if (NTMinerRoot.IsInDesignMode) {
                return;
            }
            string nvsmiDir = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles), "NVIDIA Corporation", "NVSMI");
            if (Directory.Exists(nvsmiDir)) {
                Windows.DllDirectory.SetDllDirectory(nvsmiDir);
                NvmlNativeMethods.nvmlInit();
                _isNvmlInited = true;
                uint deviceCount = 0;
                NvmlNativeMethods.nvmlDeviceGetCount(ref deviceCount);
                _nvmlDevices = new nvmlDevice[deviceCount];
                for (int i = 0; i < deviceCount; i++) {
                    NvmlNativeMethods.nvmlDeviceGetHandleByIndex((uint)i, ref _nvmlDevices[i]);
                    string name;
                    uint gClock = 0, mClock = 0;
                    NvmlNativeMethods.nvmlDeviceGetName(_nvmlDevices[i], out name);
                    NvmlNativeMethods.nvmlDeviceGetMaxClockInfo(_nvmlDevices[i], nvmlClockType.Graphics, ref gClock);
                    NvmlNativeMethods.nvmlDeviceGetMaxClockInfo(_nvmlDevices[i], nvmlClockType.Mem, ref mClock);
                    _gpus.Add(i, new Gpu {
                        Index = i,
                        Name = name
                    });
                }
                string driverVersion;
                NvmlNativeMethods.nvmlSystemGetDriverVersion(out driverVersion);
                string nvmlVersion;
                NvmlNativeMethods.nvmlSystemGetNVMLVersion(out nvmlVersion);
                this.Properties.Add(new GpuSetProperty("DriverVersion", "驱动版本", driverVersion));
                this.Properties.Add(new GpuSetProperty("NVMLVersion", "NVML版本", nvmlVersion));

            }
            Global.Access<Per5SecondEvent>(Guid.Parse("7C379223-D494-4213-9659-A086FFDE36DF"), "周期刷新显卡状态", LogEnum.None, action: message => {
                LoadGpuState();
            });
        }

        ~NVIDIAGpuSet() {
            if (_isNvmlInited) {
                NvmlNativeMethods.nvmlShutdown();
            }
        }

        private void LoadGpuState() {
            for (int i = 0; i < _nvmlDevices.Length; i++) {
                var nvmlDevice = _nvmlDevices[i];
                uint power = 0;
                NvmlNativeMethods.nvmlDeviceGetPowerUsage(nvmlDevice, ref power);
                uint temp = 0;
                NvmlNativeMethods.nvmlDeviceGetTemperature(nvmlDevice, nvmlTemperatureSensors.Gpu, ref temp);
                uint speed = 0;
                NvmlNativeMethods.nvmlDeviceGetFanSpeed(nvmlDevice, ref speed);

                Gpu gpu = (Gpu)_gpus[i];
                bool isChanged = gpu.Temperature != temp || gpu.PowerUsage != power || gpu.FanSpeed != speed;
                gpu.Temperature = temp;
                gpu.PowerUsage = power;
                gpu.FanSpeed = speed;

                if (isChanged) {
                    Global.Happened(new GpuStateChangedEvent(gpu));
                }
            }
        }

        public GpuType GpuType {
            get {
                return GpuType.NVIDIA;
            }
        }

        public IGpu this[int index] {
            get {
                if (!_gpus.ContainsKey(index)) {
                    return Gpu.Total;
                }
                return _gpus[index];
            }
        }

        public List<GpuSetProperty> Properties { get; private set; }

        public IEnumerator<IGpu> GetEnumerator() {
            return _gpus.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _gpus.Values.GetEnumerator();
        }
    }
}

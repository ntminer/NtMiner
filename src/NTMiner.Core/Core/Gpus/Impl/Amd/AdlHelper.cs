using System.Collections.Generic;
using System.Globalization;

namespace NTMiner.Core.Gpus.Impl.Amd {
    public class AdlHelper {
        public struct ATIGPU {
            public int AdapterIndex { get; set; }
            public int BusNumber { get; set; }
            public int DeviceNumber { get; set; }
            public string AdapterName { get; set; }
        }

        private System.IntPtr hHandle;
        private List<ATIGPU> _gpuNames = new List<ATIGPU>();
        public bool Init() {
            try {
                int status = ADL.ADL_Main_Control_Create(1);
                Write.DevDebug("AMD Display Library");
                Write.DevDebug("Status: " + (status == ADL.ADL_OK ? "OK" : status.ToString(CultureInfo.InvariantCulture)));
                if (status == ADL.ADL_OK) {
                    int numberOfAdapters = 0;
                    ADL.ADL2_Main_Control_Create(ADL.Main_Memory_Alloc, 1, ref hHandle);
                    ADL.ADL_Adapter_NumberOfAdapters_Get(ref numberOfAdapters);
                    if (numberOfAdapters > 0) {
                        ADLAdapterInfo[] adapterInfo = new ADLAdapterInfo[numberOfAdapters];
                        if (ADL.ADL_Adapter_AdapterInfo_Get(adapterInfo) == ADL.ADL_OK) {
                            for (int i = 0; i < numberOfAdapters; i++) {
                                int isActive;
                                ADL.ADL_Adapter_Active_Get(adapterInfo[i].AdapterIndex, out isActive);
                                if (!string.IsNullOrEmpty(adapterInfo[i].UDID) && adapterInfo[i].VendorID == ADL.ATI_VENDOR_ID) {
                                    bool found = false;
                                    foreach (ATIGPU gpu in _gpuNames)
                                        if (gpu.BusNumber == adapterInfo[i].BusNumber &&
                                          gpu.DeviceNumber == adapterInfo[i].DeviceNumber) {
                                            found = true;
                                            break;
                                        }
                                    if (!found)
                                        _gpuNames.Add(new ATIGPU {
                                            AdapterName = adapterInfo[i].AdapterName.Trim(),
                                            AdapterIndex = adapterInfo[i].AdapterIndex,
                                            BusNumber = adapterInfo[i].BusNumber,
                                            DeviceNumber = adapterInfo[i].DeviceNumber
                                        });
                                }
                            }
                        }
                    }
                }
            }
            catch {
                return false;
            }

            return true;
        }

        public int GpuCount {
            get { return _gpuNames.Count; }
        }

        public string GetGpuName(int gpu) {
            try {
                if (gpu >= _gpuNames.Count) {
                    return string.Empty;
                }
                return _gpuNames[gpu].AdapterName;
            }
            catch {
                return string.Empty;
            }
        }

        public int GetTemperatureByIndex(int adapterIndex) {
            ADLTemperature adlt = new ADLTemperature();
            if (ADL.ADL_Overdrive5_Temperature_Get(adapterIndex, 0, ref adlt)
              == ADL.ADL_OK) {
                return (int)(0.001f * adlt.Temperature);
            }
            else {
                return 0;
            }
        }

        public uint GetFanSpeedByIndex(int adapterIndex) {
            ADLFanSpeedValue adlf = new ADLFanSpeedValue();
            adlf.SpeedType = ADL.ADL_DL_FANCTRL_SPEED_TYPE_PERCENT;
            if (ADL.ADL_Overdrive5_FanSpeed_Get(adapterIndex, 0, ref adlf)
              == ADL.ADL_OK) {
                return (uint)adlf.FanSpeed;
            }
            else {
                return 0;
            }
        }

        public uint GetPowerUsageByIndex(int gpu) {
            int power = 0;
            try {
                if (ADL.ADL2_Overdrive6_CurrentPower_Get(hHandle, gpu, 0, ref power) == 0) {
                    return (uint)(power / 256.0);
                }
            }
            catch {
            }
            return 0;
        }
    }
}

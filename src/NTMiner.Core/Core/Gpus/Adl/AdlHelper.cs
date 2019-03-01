using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NTMiner.Core.Gpus.Adl {
    public class AdlHelper {
        private IntPtr hHandle;
        private List<ADLAdapterInfo> _adlAdapterInfos = new List<ADLAdapterInfo>();

        public bool Init() {
            int ret = 0;
            try {
                ret += AdlNativeMethods.ADL_Main_Control_Create(AdlNativeMethods.ADL_Main_Memory_Alloc, 1);
                ret += AdlNativeMethods.ADL_Main_Control_Refresh();
                AdlNativeMethods.ADL2_Main_Control_Create(AdlNativeMethods.ADL_Main_Memory_Alloc, 1, ref hHandle);
                ret += AdlNativeMethods.ADL2_Main_Control_Refresh(hHandle);
                int iNumberAdapters = 0;
                AdlNativeMethods.ADL_Adapter_NumberOfAdapters_Get(ref iNumberAdapters);
                ADLAdapterInfo[] adapterInfo = new ADLAdapterInfo[iNumberAdapters];

                int elementSize = Marshal.SizeOf(typeof(ADLAdapterInfo));
                int size = adapterInfo.Length * elementSize;
                IntPtr ptr = Marshal.AllocHGlobal(size);
                int result = AdlNativeMethods.ADL_Adapter_AdapterInfo_Get(ptr, size);
                for (int i = 0; i < adapterInfo.Length; i++) {
                    adapterInfo[i] = (ADLAdapterInfo)Marshal.PtrToStructure((IntPtr)((long)ptr + i * elementSize), typeof(ADLAdapterInfo));
                }
                Marshal.FreeHGlobal(ptr);

                // the ADLAdapterInfo.VendorID field reported by ADL is wrong on 
                // Windows systems (parse error), so we fix this here
                int lastAdapterId = 0;
                int gpuCount = 0;
                _adlAdapterInfos = new List<ADLAdapterInfo>();
                for (int i = 0; i < adapterInfo.Length; i++) {
                    int lpAdapterID = -1;
                    ret = AdlNativeMethods.ADL_Adapter_ID_Get(adapterInfo[i].AdapterIndex, ref lpAdapterID);
                    if (ret != 0)
                        continue;
                    if (lastAdapterId == lpAdapterID)
                        continue;
                    lastAdapterId = lpAdapterID;

                    gpuCount++;
                    _adlAdapterInfos.Add(adapterInfo[i]);
                }
            }
            catch {
            }

            return ret == 0;
        }

        public int GpuCount {
            get {
                return _adlAdapterInfos.Count;
            }
        }

        public string GetGpuName(int gpu) {
            try {
                return _adlAdapterInfos[gpu].AdapterName;
            }
            catch {
                return string.Empty;
            }
        }

        public uint GetTemperatureByIndex(int gpu) {
            ADLTemperature temperature = default(ADLTemperature);
            try {
                if (AdlNativeMethods.ADL_Overdrive5_Temperature_Get(gpu, 0, ref temperature) == 0) {
                    return (uint)(temperature.Temperature / 1000.0);
                }
            }
            catch {
            }

            return 0;
        }

        public uint GetFanSpeedByIndex(int gpu) {
            ADLFanSpeedValue fanspeed = default(ADLFanSpeedValue);
            fanspeed.SpeedType = AdlTypes.ADL_DL_FANCTRL_SPEED_TYPE_PERCENT;
            try {
                if (AdlNativeMethods.ADL_Overdrive5_FanSpeed_Get(gpu, 0, ref fanspeed) == 0) {
                    return (uint)fanspeed.FanSpeed;
                }
            }
            catch {
            }
            return 0;
        }

        public uint GetPowerUsageByIndex(int gpu) {
            int power = 0;
            try {
                if (AdlNativeMethods.ADL2_Overdrive6_CurrentPower_Get(hHandle, gpu, 0, ref power) == 0) {
                    return (uint)(power / 256.0);
                }
            }
            catch {
            }
            return 0;
        }

        public string GetDriverVersion() {
            ADLVersionsInfo versioninfo = default(ADLVersionsInfo);
            try {
                if (AdlNativeMethods.ADL_Graphics_Versions_Get(ref versioninfo) >= 0) {
                    return versioninfo.strDriverVer;
                }
            }
            catch {
            }
            return "0.0";
        }
    }
}

using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace NTMiner.Gpus.Adl {
    internal class AdlNativeMethods {
        private delegate AdlStatus ADL_Main_Control_CreateDelegate(ADL_Main_Memory_AllocDelegate callback, int enumConnectedAdapters);
        public delegate AdlStatus ADL2_Main_Control_CreateDelegate(ADL_Main_Memory_AllocDelegate callback, int enumConnectedAdapters, ref IntPtr context);
        private delegate AdlStatus ADL_Adapter_AdapterInfo_GetDelegate(IntPtr info, int size);

        internal delegate AdlStatus ADL_Main_Control_DestroyDelegate();
        internal delegate AdlStatus ADL_Adapter_NumberOfAdapters_GetDelegate(ref int numAdapters);
        internal delegate AdlStatus ADL_Overdrive5_Temperature_GetDelegate(int adapterIndex, int thermalControllerIndex, ref ADLTemperature temperature);
        internal delegate AdlStatus ADL_Overdrive5_FanSpeed_GetDelegate(int adapterIndex, int thermalControllerIndex, ref ADLFanSpeedValue fanSpeedValue);
        internal delegate AdlStatus ADL2_Overdrive5_FanSpeedToDefault_SetDelegate(IntPtr context, int adapterIndex, int thermalControllerIndex);
        internal delegate AdlStatus ADL_Overdrive5_FanSpeed_SetDelegate(int adapterIndex, int thermalControllerIndex, ref ADLFanSpeedValue fanSpeedValue);
        internal delegate AdlStatus ADL2_OverdriveN_PowerLimit_GetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPowerLimitSetting lpODPowerLimit);
        internal delegate AdlStatus ADL2_OverdriveN_PowerLimit_SetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPowerLimitSetting lpODPowerLimit);
        internal delegate AdlStatus ADL2_Overdrive6_CurrentPower_GetDelegate(IntPtr context, int iAdapterIndex, int iPowerType, ref int lpCurrentValue);
        internal delegate AdlStatus ADL_Adapter_MemoryInfo_GetDelegate(int iAdapterIndex, ref ADLMemoryInfo lpMemoryInfo);
        internal delegate AdlStatus ADL2_Graphics_VersionsX2_GetDelegate(IntPtr context, ref ADLVersionsInfoX2 lpVersionsInfo);
        internal delegate AdlStatus ADL2_OverdriveN_MemoryClocksX2_GetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPerformanceLevelsX2 lpODPerformanceLevels);
        internal delegate AdlStatus ADL2_OverdriveN_MemoryClocksX2_SetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPerformanceLevelsX2 lpODPerformanceLevels);
        internal delegate AdlStatus ADL2_OverdriveN_SystemClocksX2_GetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPerformanceLevelsX2 lpODPerformanceLevels);
        internal delegate AdlStatus ADL2_OverdriveN_SystemClocksX2_SetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPerformanceLevelsX2 lpODPerformanceLevels);
        internal delegate AdlStatus ADL2_OverdriveN_CapabilitiesX2_GetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNCapabilitiesX2 lpODCapabilities);
        internal delegate AdlStatus ADL2_Overdrive6_FanSpeed_ResetDelegate(IntPtr context, int iAdapterIndex);

        private static ADL_Main_Control_CreateDelegate _ADL_Main_Control_Create;
        internal static ADL2_Main_Control_CreateDelegate ADL2_Main_Control_Create;
        private static ADL_Adapter_AdapterInfo_GetDelegate _ADL_Adapter_AdapterInfo_Get;

        private static ADL_Main_Control_DestroyDelegate ADL_Main_Control_Destroy;
        internal static ADL_Adapter_NumberOfAdapters_GetDelegate ADL_Adapter_NumberOfAdapters_Get;
        internal static ADL_Overdrive5_Temperature_GetDelegate ADL_Overdrive5_Temperature_Get;
        internal static ADL_Overdrive5_FanSpeed_GetDelegate ADL_Overdrive5_FanSpeed_Get;
        internal static ADL2_Overdrive5_FanSpeedToDefault_SetDelegate ADL2_Overdrive5_FanSpeedToDefault_Set;
        internal static ADL_Overdrive5_FanSpeed_SetDelegate ADL_Overdrive5_FanSpeed_Set;
        internal static ADL2_OverdriveN_PowerLimit_GetDelegate ADL2_OverdriveN_PowerLimit_Get;
        internal static ADL2_Overdrive6_CurrentPower_GetDelegate ADL2_Overdrive6_CurrentPower_Get;
        internal static ADL2_OverdriveN_PowerLimit_SetDelegate ADL2_OverdriveN_PowerLimit_Set;
        internal static ADL_Adapter_MemoryInfo_GetDelegate ADL_Adapter_MemoryInfo_Get;
        internal static ADL2_Graphics_VersionsX2_GetDelegate ADL2_Graphics_VersionsX2_Get;
        internal static ADL2_OverdriveN_MemoryClocksX2_GetDelegate ADL2_OverdriveN_MemoryClocksX2_Get;
        internal static ADL2_OverdriveN_MemoryClocksX2_SetDelegate ADL2_OverdriveN_MemoryClocksX2_Set;
        internal static ADL2_OverdriveN_SystemClocksX2_GetDelegate ADL2_OverdriveN_SystemClocksX2_Get;
        internal static ADL2_OverdriveN_SystemClocksX2_SetDelegate ADL2_OverdriveN_SystemClocksX2_Set;
        internal static ADL2_OverdriveN_CapabilitiesX2_GetDelegate ADL2_OverdriveN_CapabilitiesX2_Get;
        internal static ADL2_Overdrive6_FanSpeed_ResetDelegate ADL2_Overdrive6_FanSpeed_Reset;

        private static string dllName;

        private static void GetDelegate<T>(string entryPoint, out T newDelegate)
          where T : class {
            DllImportAttribute attribute = new DllImportAttribute(dllName) {
                CallingConvention = CallingConvention.Cdecl,
                PreserveSig = true,
                EntryPoint = entryPoint
            };
            PInvokeDelegateFactory.CreateDelegate(attribute, out newDelegate);
        }

        private static void CreateDelegates(string name) {
            dllName = name + ".dll";

            GetDelegate(nameof(ADL_Main_Control_Create), out _ADL_Main_Control_Create);
            GetDelegate(nameof(ADL2_Main_Control_Create), out ADL2_Main_Control_Create);
            GetDelegate(nameof(ADL_Adapter_AdapterInfo_Get), out _ADL_Adapter_AdapterInfo_Get);
            GetDelegate(nameof(ADL_Main_Control_Destroy), out ADL_Main_Control_Destroy);
            GetDelegate(nameof(ADL_Adapter_NumberOfAdapters_Get), out ADL_Adapter_NumberOfAdapters_Get);
            GetDelegate(nameof(ADL_Overdrive5_Temperature_Get), out ADL_Overdrive5_Temperature_Get);
            GetDelegate(nameof(ADL_Overdrive5_FanSpeed_Get), out ADL_Overdrive5_FanSpeed_Get);
            GetDelegate(nameof(ADL2_Overdrive5_FanSpeedToDefault_Set), out ADL2_Overdrive5_FanSpeedToDefault_Set);
            GetDelegate(nameof(ADL_Overdrive5_FanSpeed_Set), out ADL_Overdrive5_FanSpeed_Set);
            GetDelegate(nameof(ADL2_OverdriveN_PowerLimit_Get), out ADL2_OverdriveN_PowerLimit_Get);
            GetDelegate(nameof(ADL2_Overdrive6_CurrentPower_Get), out ADL2_Overdrive6_CurrentPower_Get);
            GetDelegate(nameof(ADL2_OverdriveN_PowerLimit_Set), out ADL2_OverdriveN_PowerLimit_Set);
            GetDelegate(nameof(ADL_Adapter_MemoryInfo_Get), out ADL_Adapter_MemoryInfo_Get);
            GetDelegate(nameof(ADL2_Graphics_VersionsX2_Get), out ADL2_Graphics_VersionsX2_Get);
            GetDelegate(nameof(ADL2_OverdriveN_MemoryClocksX2_Get), out ADL2_OverdriveN_MemoryClocksX2_Get);
            GetDelegate(nameof(ADL2_OverdriveN_MemoryClocksX2_Set), out ADL2_OverdriveN_MemoryClocksX2_Set);
            GetDelegate(nameof(ADL2_OverdriveN_SystemClocksX2_Get), out ADL2_OverdriveN_SystemClocksX2_Get);
            GetDelegate(nameof(ADL2_OverdriveN_SystemClocksX2_Set), out ADL2_OverdriveN_SystemClocksX2_Set);
            GetDelegate(nameof(ADL2_OverdriveN_CapabilitiesX2_Get), out ADL2_OverdriveN_CapabilitiesX2_Get);
            GetDelegate(nameof(ADL2_Overdrive6_FanSpeed_Reset), out ADL2_Overdrive6_FanSpeed_Reset);
        }

        static AdlNativeMethods() {
            CreateDelegates("atiadlxx");
        }

        private AdlNativeMethods() { }

        internal static AdlStatus ADL_Main_Control_Create(int enumConnectedAdapters) {
            try {
                try {
                    return _ADL_Main_Control_Create(Main_Memory_Alloc, enumConnectedAdapters);
                }
                catch {
                    CreateDelegates("atiadlxy");
                    return _ADL_Main_Control_Create(Main_Memory_Alloc, enumConnectedAdapters);
                }
            }
            catch {
                return AdlStatus.ADL_ERR;
            }
        }

        internal static AdlStatus ADL_Adapter_AdapterInfo_Get(ADLAdapterInfo[] info) {
            int elementSize = Marshal.SizeOf(typeof(ADLAdapterInfo));
            int size = info.Length * elementSize;
            IntPtr ptr = Marshal.AllocHGlobal(size);
            AdlStatus result = _ADL_Adapter_AdapterInfo_Get(ptr, size);
            for (int i = 0; i < info.Length; i++) {
                info[i] = (ADLAdapterInfo)Marshal.PtrToStructure((IntPtr)((long)ptr + i * elementSize), typeof(ADLAdapterInfo));
            }
            Marshal.FreeHGlobal(ptr);

            // the ADLAdapterInfo.VendorID field reported by ADL is wrong on 
            // Windows systems (parse error), so we fix this here
            for (int i = 0; i < info.Length; i++) {
                // try Windows UDID format
                Match m = Regex.Match(info[i].UDID, "PCI_VEN_([A-Fa-f0-9]{1,4})&.*");
                if (m.Success && m.Groups.Count == 2) {
                    info[i].VendorID = Convert.ToInt32(m.Groups[1].Value, 16);
                    continue;
                }
                // if above failed, try Unix UDID format
                m = Regex.Match(info[i].UDID, "[0-9]+:[0-9]+:([0-9]+):[0-9]+:[0-9]+");
                if (m.Success && m.Groups.Count == 2) {
                    info[i].VendorID = Convert.ToInt32(m.Groups[1].Value, 10);
                }
            }

            return result;
        }

        internal delegate IntPtr ADL_Main_Memory_AllocDelegate(int size);

        // create a Main_Memory_Alloc delegate and keep it alive
        internal static ADL_Main_Memory_AllocDelegate Main_Memory_Alloc = (int size) => {
            return Marshal.AllocHGlobal(size);
        };
    }
}

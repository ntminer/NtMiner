using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace NTMiner.Gpus.Adl {
    public static class AdlNativeMethods {
        [AttributeUsage(AttributeTargets.Property)]
        public class AdlAttribute : Attribute {
            public AdlAttribute() {
            }
        }

        internal delegate IntPtr ADL_Main_Memory_AllocCallback(int size);
        internal delegate AdlStatus ADL_Main_Control_CreateDelegate(ADL_Main_Memory_AllocCallback callback, int enumConnectedAdapters);
        internal delegate AdlStatus ADL2_Main_Control_CreateDelegate(ADL_Main_Memory_AllocCallback callback, int enumConnectedAdapters, out IntPtr context);
        internal delegate AdlStatus ADL_Adapter_AdapterInfo_GetDelegate(IntPtr info, int size);
        internal delegate AdlStatus ADL2_Adapter_AdapterInfo_GetDelegate(IntPtr context, ref IntPtr info, int size);

        public delegate AdlStatus ADL_Main_Control_DestroyDelegate();
        public delegate AdlStatus ADL2_Main_Control_DestroyDelegate(IntPtr context);
        internal delegate AdlStatus ADL_Adapter_NumberOfAdapters_GetDelegate(ref int numAdapters);
        internal delegate AdlStatus ADL2_Adapter_NumberOfAdapters_GetDelegate(IntPtr context, ref int numAdapters);
        internal delegate AdlStatus ADL_Adapter_Active_GetDelegate(int adapterIndex, out int status);
        internal delegate AdlStatus ADL_Overdrive5_CurrentActivity_GetDelegate(int iAdapterIndex, ref ADLPMActivity activity);
        internal delegate AdlStatus ADL_Overdrive5_Temperature_GetDelegate(int adapterIndex, int thermalControllerIndex, ref ADLTemperature temperature);
        internal delegate AdlStatus ADL2_OverdriveN_Temperature_GetDelegate(IntPtr context, int adapterIndex, ADLODNTemperatureType temperatureType, out int temperature);
        internal delegate AdlStatus ADL_Overdrive5_FanSpeed_GetDelegate(int adapterIndex, int thermalControllerIndex, ref ADLFanSpeedValue fanSpeedValue);
        internal delegate AdlStatus ADL_Overdrive5_FanSpeed_SetDelegate(int adapterIndex, int thermalControllerIndex, ref ADLFanSpeedValue fanSpeedValue);
        internal delegate AdlStatus ADL_Overdrive5_FanSpeedInfo_GetDelegate(int adapterIndex, int thermalControllerIndex, ref ADLFanSpeedInfo fanSpeedInfo);
        internal delegate AdlStatus ADL_Overdrive5_FanSpeedToDefault_SetDelegate(int adapterIndex, int thermalControllerIndex);
        internal delegate AdlStatus ADL2_Overdrive5_FanSpeedToDefault_SetDelegate(IntPtr context, int adapterIndex, int thermalControllerIndex);
        internal delegate AdlStatus ADL2_OverdriveN_FanControl_GetDelegate(IntPtr context, int adapterIndex, out ADLOverdriveFanControl fanControl);
        internal delegate AdlStatus ADL2_OverdriveN_FanControl_SetDelegate(IntPtr context, int iAdapterIndex, ref ADLOverdriveFanControl fanControl);
        internal delegate AdlStatus ADL_Overdrive_CapsDelegate(int adapterIndex, out int supported, out int enabled, out int version);
        internal delegate AdlStatus ADL2_Overdrive_CapsDelegate(IntPtr context, int adapterIndex, out int supported, out int enabled, out int version);
        internal delegate AdlStatus ADL2_OverdriveN_PowerLimit_GetDelegate(IntPtr context, int iAdapterIndex, out ADLODNPowerLimitSetting lpODPowerLimit);
        internal delegate AdlStatus ADL2_OverdriveN_PowerLimit_SetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPowerLimitSetting lpODPowerLimit);
        internal delegate AdlStatus ADL2_Overdrive6_CurrentPower_GetDelegate(IntPtr context, int iAdapterIndex, ADLODNCurrentPowerType powerType, out int lpCurrentValue);
        internal delegate AdlStatus ADL2_New_QueryPMLogData_GetDelegate(IntPtr context, int adapterIndex, ref ADLPMLogDataOutput dataOutput);
        internal delegate AdlStatus ADL_Overdrive5_ODParameters_GetDelegate(int adapterIndex, out ADLODParameters parameters);
        internal delegate AdlStatus ADL2_OverdriveN_PerformanceStatus_GetDelegate(IntPtr context, int adapterIndex, out ADLODNPerformanceStatus performanceStatus);
        internal delegate AdlStatus ADL_Graphics_Versions_GetDelegate(out ADLVersionsInfo versionInfo);
        internal delegate AdlStatus ADL_Adapter_MemoryInfo_GetDelegate(int iAdapterIndex, ref ADLMemoryInfo lpMemoryInfo);
        internal delegate AdlStatus ADL2_Graphics_VersionsX2_GetDelegate(IntPtr context, ref ADLVersionsInfoX2 lpVersionsInfo);
        internal delegate AdlStatus ADL2_OverdriveN_MemoryClocksX2_GetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPerformanceLevelsX2 lpODPerformanceLevels);
        internal delegate AdlStatus ADL2_OverdriveN_MemoryClocksX2_SetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPerformanceLevelsX2 lpODPerformanceLevels);
        internal delegate AdlStatus ADL2_OverdriveN_SystemClocksX2_GetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPerformanceLevelsX2 lpODPerformanceLevels);
        internal delegate AdlStatus ADL2_OverdriveN_SystemClocksX2_SetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPerformanceLevelsX2 lpODPerformanceLevels);
        internal delegate AdlStatus ADL2_OverdriveN_CapabilitiesX2_GetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNCapabilitiesX2 lpODCapabilities);
        internal delegate AdlStatus ADL2_Overdrive6_FanSpeed_ResetDelegate(IntPtr context, int iAdapterIndex);
        internal delegate AdlStatus ADL2_Overdrive8_Current_SettingX2_GetDelegate(IntPtr context, int iAdapterIndex, ref int lpNumberOfFeatures, out IntPtr lppCurrentSettingList);
        internal delegate AdlStatus ADL2_Overdrive8_Init_SettingX2_GetDelegate(IntPtr context, int iAdapterIndex, out int lpOverdrive8Capabilities, ref int lpNumberOfFeatures, out IntPtr lppInitSettingList);
        internal delegate AdlStatus ADL2_Overdrive8_Init_Setting_GetDelegate(IntPtr context, int iAdapterIndex, out ADLOD8InitSetting lpInitSetting);
        internal delegate AdlStatus ADL2_Overdrive8_Setting_SetDelegate(IntPtr context, int iAdapterIndex, ref ADLOD8SetSetting lpSetSetting, out ADLOD8CurrentSetting lpCurrentSetting);

        // 以下属性要求必须是外部可见的static，不能是private的
        // 注意属性名是EnterPoint，不要改名
        [Adl]
        internal static ADL_Main_Control_CreateDelegate ADL_Main_Control_Create { get; private set; }
        [Adl]
        internal static ADL2_Main_Control_CreateDelegate ADL2_Main_Control_Create { get; private set; }
        [Adl]
        internal static ADL_Adapter_AdapterInfo_GetDelegate ADL_Adapter_AdapterInfo_Get { get; private set; }
        [Adl]
        internal static ADL2_Adapter_AdapterInfo_GetDelegate ADL2_Adapter_AdapterInfo_Get { get; set; }
        [Adl]
        internal static ADL_Main_Control_DestroyDelegate ADL_Main_Control_Destroy { get; private set; }
        [Adl]
        internal static ADL2_Main_Control_DestroyDelegate ADL2_Main_Control_Destroy { get; private set; }
        [Adl]
        internal static ADL_Adapter_NumberOfAdapters_GetDelegate ADL_Adapter_NumberOfAdapters_Get { get; private set; }
        [Adl]
        internal static ADL2_Adapter_NumberOfAdapters_GetDelegate ADL2_Adapter_NumberOfAdapters_Get { get; private set; }
        [Adl]
        internal static ADL_Adapter_Active_GetDelegate ADL_Adapter_Active_Get { get; private set; }
        [Adl]
        internal static ADL_Overdrive5_CurrentActivity_GetDelegate ADL_Overdrive5_CurrentActivity_Get { get; private set; }
        [Adl]
        internal static ADL_Overdrive5_Temperature_GetDelegate ADL_Overdrive5_Temperature_Get { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_Temperature_GetDelegate ADL2_OverdriveN_Temperature_Get { get; private set; }
        [Adl]
        internal static ADL_Overdrive5_FanSpeed_GetDelegate ADL_Overdrive5_FanSpeed_Get { get; private set; }
        [Adl]
        internal static ADL_Overdrive5_FanSpeed_SetDelegate ADL_Overdrive5_FanSpeed_Set { get; private set; }
        [Adl]
        internal static ADL_Overdrive5_FanSpeedInfo_GetDelegate ADL_Overdrive5_FanSpeedInfo_Get { get; private set; }
        [Adl]
        internal static ADL_Overdrive5_FanSpeedToDefault_SetDelegate ADL_Overdrive5_FanSpeedToDefault_Set { get; private set; }
        [Adl]
        internal static ADL2_Overdrive5_FanSpeedToDefault_SetDelegate ADL2_Overdrive5_FanSpeedToDefault_Set { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_FanControl_GetDelegate ADL2_OverdriveN_FanControl_Get { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_FanControl_SetDelegate ADL2_OverdriveN_FanControl_Set { get; private set; }
        [Adl]
        internal static ADL_Overdrive_CapsDelegate ADL_Overdrive_Caps { get; private set; }
        [Adl]
        internal static ADL2_Overdrive_CapsDelegate ADL2_Overdrive_Caps { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_PowerLimit_GetDelegate ADL2_OverdriveN_PowerLimit_Get { get; private set; }
        [Adl]
        internal static ADL2_Overdrive6_CurrentPower_GetDelegate ADL2_Overdrive6_CurrentPower_Get { get; private set; }
        [Adl]
        internal static ADL2_New_QueryPMLogData_GetDelegate ADL2_New_QueryPMLogData_Get { get; private set; }
        [Adl]
        internal static ADL_Overdrive5_ODParameters_GetDelegate ADL_Overdrive5_ODParameters_Get { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_PerformanceStatus_GetDelegate ADL2_OverdriveN_PerformanceStatus_Get { get; private set; }
        [Adl]
        internal static ADL_Graphics_Versions_GetDelegate ADL_Graphics_Versions_Get { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_PowerLimit_SetDelegate ADL2_OverdriveN_PowerLimit_Set { get; private set; }
        [Adl]
        internal static ADL_Adapter_MemoryInfo_GetDelegate ADL_Adapter_MemoryInfo_Get { get; private set; }
        [Adl]
        internal static ADL2_Graphics_VersionsX2_GetDelegate ADL2_Graphics_VersionsX2_Get { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_MemoryClocksX2_GetDelegate ADL2_OverdriveN_MemoryClocksX2_Get { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_MemoryClocksX2_SetDelegate ADL2_OverdriveN_MemoryClocksX2_Set { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_SystemClocksX2_GetDelegate ADL2_OverdriveN_SystemClocksX2_Get { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_SystemClocksX2_SetDelegate ADL2_OverdriveN_SystemClocksX2_Set { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_CapabilitiesX2_GetDelegate ADL2_OverdriveN_CapabilitiesX2_Get { get; private set; }
        [Adl]
        internal static ADL2_Overdrive6_FanSpeed_ResetDelegate ADL2_Overdrive6_FanSpeed_Reset { get; private set; }
        [Adl]
        internal static ADL2_Overdrive8_Current_SettingX2_GetDelegate ADL2_Overdrive8_Current_SettingX2_Get { get; private set; }
        [Adl]
        internal static ADL2_Overdrive8_Init_SettingX2_GetDelegate ADL2_Overdrive8_Init_SettingX2_Get { get; private set; }
        [Adl]
        internal static ADL2_Overdrive8_Init_Setting_GetDelegate ADL2_Overdrive8_Init_Setting_Get { get; private set; }
        [Adl]
        internal static ADL2_Overdrive8_Setting_SetDelegate ADL2_Overdrive8_Setting_Set { get; private set; }

        internal static AdlStatus ADLMainControlCreate(out IntPtr context) {
            AdlStatus r;
            string dllName = "atiadlxx.dll";
            try {
                CreateDelegates(dllName);
                r = ADL_Main_Control_Create(Marshal.AllocHGlobal, 1);
            }
            catch(Exception e) {
                Logger.ErrorDebugLine(e);
                try {
                    dllName = "atiadlxy.dll";
                    CreateDelegates(dllName);
                    r = ADL_Main_Control_Create(Marshal.AllocHGlobal, 1);
                }
                catch(Exception ex) {
                    Logger.ErrorDebugLine(ex);
                    r = AdlStatus.ADL_ERR;
                }
            }
            if (r < AdlStatus.ADL_OK) {
                NTMinerConsole.DevError(() => $"{nameof(ADL_Main_Control_Create)} {r.ToString()} {dllName}");
            }
            ADL2MainControlCreate(out context);
            return r;
        }

        private static void ADL2MainControlCreate(out IntPtr context) {
            try {
                var r = ADL2_Main_Control_Create(Marshal.AllocHGlobal, 1, out context);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(ADL2_Main_Control_Create)} {r.ToString()}");
                }
            }
            catch (Exception ex) {
                Logger.ErrorDebugLine(ex);
                context = IntPtr.Zero;
            }
        }

        internal static AdlStatus ADLAdapterAdapterInfoGet(ADLAdapterInfo[] info) {
            int elementSize = Marshal.SizeOf(typeof(ADLAdapterInfo));
            int size = info.Length * elementSize;
            IntPtr ptr = Marshal.AllocHGlobal(size);
            AdlStatus result = ADL_Adapter_AdapterInfo_Get(ptr, size);
            for (int i = 0; i < info.Length; i++) {
                info[i] = (ADLAdapterInfo)Marshal.PtrToStructure((IntPtr)((long)ptr + i * elementSize), typeof(ADLAdapterInfo));
            }
            Marshal.FreeHGlobal(ptr);

            // the ADLAdapterInfo.VendorID field reported by ADL is wrong on 
            // Windows systems (parse error), so we fix this here
            Regex regex1 = new Regex("PCI_VEN_([A-Fa-f0-9]{1,4})&.*");
            Regex regex2 = new Regex("[0-9]+:[0-9]+:([0-9]+):[0-9]+:[0-9]+");
            for (int i = 0; i < info.Length; i++) {
                // try Windows UDID format
                Match m = regex1.Match(info[i].UDID);
                if (m.Success && m.Groups.Count == 2) {
                    info[i].VendorID = Convert.ToInt32(m.Groups[1].Value, 16);
                    continue;
                }
                // if above failed, try Unix UDID format
                m = regex2.Match(info[i].UDID);
                if (m.Success && m.Groups.Count == 2) {
                    info[i].VendorID = Convert.ToInt32(m.Groups[1].Value, 10);
                }
            }

            return result;
        }

        private static void SetDelegate(PropertyInfo property, string dllName) {
            DllImportAttribute attribute = new DllImportAttribute(dllName) {
                CallingConvention = CallingConvention.Cdecl,
                PreserveSig = true,
                EntryPoint = property.Name
            };
            try {
                PInvokeDelegateFactory.CreateDelegate(attribute, property.PropertyType, out object newDelegate);
                property.SetValue(null, newDelegate, null);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static void CreateDelegates(string dllName) {
            Type t = typeof(AdlNativeMethods);
            var properties = t.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetProperty);
            foreach (var property in properties) {
                var attrs = property.GetCustomAttributes(typeof(AdlAttribute), inherit: false);
                if (attrs.Length == 0) {
                    continue;
                }
                SetDelegate(property, dllName);
            }
        }
    }
}

using System;
using System.Runtime.InteropServices;

namespace NTMiner.Core.Gpus.Impl.Nvapi {
    internal static class NvapiNativeMethods {
        internal const int MAX_PHYSICAL_GPUS = 64;
        internal const int MAX_PSTATES_PER_GPU = 8;
        internal const int MAX_COOLER_PER_GPU = 20;
        internal const int MAX_THERMAL_SENSORS_PER_GPU = 3;
        internal const int MAX_POWER_ENTRIES_PER_GPU = 4;

        public static readonly uint GPU_PSTATES_VER = (uint)Marshal.SizeOf(typeof(NvPStates)) | 0x10000;
        public static readonly uint GPU_THERMAL_SETTINGS_VER = (uint)Marshal.SizeOf(typeof(NvGPUThermalSettings)) | 0x10000;
        public static readonly uint GPU_POWER_STATUS_VER = (uint)Marshal.SizeOf(typeof(NvGPUPowerStatus)) | 0x10000;
        public static readonly uint GPU_POWER_INFO_VER = (uint)Marshal.SizeOf(typeof(NvGPUPowerInfo)) | 0x10000;

        #region Delegates
        private delegate IntPtr nvapi_QueryInterfaceDelegate(uint id);
        private delegate NvStatus NvAPI_InitializeDelegate();

        internal delegate NvStatus NvAPI_EnumPhysicalGPUsDelegate([Out] NvPhysicalGpuHandle[] gpuHandles, out int gpuCount);
        internal delegate NvStatus NvAPI_GPU_GetBusIdDelegate(NvPhysicalGpuHandle gpuHandle, out int busID);
        internal delegate NvStatus NvAPI_GPU_GetTachReadingDelegate(NvPhysicalGpuHandle gpuHandle, out int value);
        internal delegate NvStatus NvAPI_GPU_GetPStatesDelegate(NvPhysicalGpuHandle gpuHandle, ref NvPStates nvPStates);
        internal delegate NvStatus NvAPI_GPU_GetThermalSettingsDelegate(NvPhysicalGpuHandle gpuHandle, int sensorIndex, ref NvGPUThermalSettings nvGPUThermalSettings);

        internal delegate NvStatus NvAPI_DLL_ClientPowerPoliciesGetInfoDelegate(NvPhysicalGpuHandle gpuHandle, ref NvGPUPowerInfo info);
        internal delegate NvStatus NvAPI_DLL_ClientPowerPoliciesGetStatusDelegate(NvPhysicalGpuHandle gpuHandle, ref NvGPUPowerStatus status);
        internal delegate NvStatus NvAPI_DLL_ClientPowerPoliciesSetStatusDelegate(NvPhysicalGpuHandle gpuHandle, ref NvGPUPowerStatus status);


        private static readonly nvapi_QueryInterfaceDelegate nvapi_QueryInterface;
        private static readonly NvAPI_InitializeDelegate NvAPI_Initialize;
        private static readonly bool available;

        internal static readonly NvAPI_EnumPhysicalGPUsDelegate NvAPI_EnumPhysicalGPUs;
        internal static readonly NvAPI_GPU_GetBusIdDelegate NvAPI_GPU_GetBusID;
        internal static readonly NvAPI_GPU_GetTachReadingDelegate NvAPI_GPU_GetTachReading;
        internal static readonly NvAPI_GPU_GetPStatesDelegate NvAPI_GPU_GetPStates;
        internal static readonly NvAPI_GPU_GetThermalSettingsDelegate NvAPI_GPU_GetThermalSettings;

        internal static readonly NvAPI_DLL_ClientPowerPoliciesGetInfoDelegate NvAPI_DLL_ClientPowerPoliciesGetInfo;
        internal static readonly NvAPI_DLL_ClientPowerPoliciesGetStatusDelegate NvAPI_DLL_ClientPowerPoliciesGetStatus;
        internal static readonly NvAPI_DLL_ClientPowerPoliciesSetStatusDelegate NvAPI_DLL_ClientPowerPoliciesSetStatus;

        #endregion

        private static void GetDelegate<T>(uint id, out T newDelegate)
            where T : class {
            IntPtr ptr = nvapi_QueryInterface(id);
            if (ptr != IntPtr.Zero) {
                newDelegate = Marshal.GetDelegateForFunctionPointer(ptr, typeof(T)) as T;
            }
            else {
                newDelegate = null;
            }
        }

        static NvapiNativeMethods() {
            DllImportAttribute attribute = new DllImportAttribute("nvapi64.dll");
            attribute.CallingConvention = CallingConvention.Cdecl;
            attribute.PreserveSig = true;
            attribute.EntryPoint = "nvapi_QueryInterface";
            PInvokeDelegateFactory.CreateDelegate(attribute, out nvapi_QueryInterface);

            try {
                GetDelegate(0x0150E828, out NvAPI_Initialize);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }

            if (NvAPI_Initialize() == NvStatus.OK) {
                GetDelegate(0x5F608315, out NvAPI_GPU_GetTachReading);
                GetDelegate(0x60DED2ED, out NvAPI_GPU_GetPStates);
                GetDelegate(0xE3640A56, out NvAPI_GPU_GetThermalSettings);
                GetDelegate(0xE5AC921F, out NvAPI_EnumPhysicalGPUs);
                GetDelegate(0x1BE0B8E5, out NvAPI_GPU_GetBusID);
                GetDelegate(0x34206D86, out NvAPI_DLL_ClientPowerPoliciesGetInfo);
                GetDelegate(0x70916171, out NvAPI_DLL_ClientPowerPoliciesGetStatus);
                GetDelegate(0xAD95F5ED, out NvAPI_DLL_ClientPowerPoliciesSetStatus);
            }

            available = true;
        }

        public static bool IsAvailable { get { return available; } }

        public static uint MakeNVAPIVersion(object param, uint version) {
            return 72 | (version << 16);
        }
    }
}

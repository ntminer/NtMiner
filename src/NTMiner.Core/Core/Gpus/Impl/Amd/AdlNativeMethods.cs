using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace NTMiner.Core.Gpus.Impl.Amd {
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLAdapterInfo {
        public int Size;
        public int AdapterIndex;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
        public string UDID;
        public int BusNumber;
        public int DeviceNumber;
        public int FunctionNumber;
        public int VendorID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
        public string AdapterName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
        public string DisplayName;
        public int Present;
        public int Exist;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
        public string DriverPath;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
        public string DriverPathExt;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
        public string PNPString;
        public int OSDisplayIndex;

        public override string ToString() {
            return $"Size={Size},AdapterIndex={AdapterIndex},UDID={UDID},BusNumber={BusNumber},DeviceNumber={DeviceNumber},FunctionNumber={FunctionNumber},VendorID={VendorID},AdapterName={AdapterName},DisplayName={DisplayName},Present={Present},Exist={Exist},PNPString={PNPString},OSDisplayIndex={OSDisplayIndex}";
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLPMActivity {
        public int Size;
        public int EngineClock;
        public int MemoryClock;
        public int Vddc;
        public int ActivityPercent;
        public int CurrentPerformanceLevel;
        public int CurrentBusSpeed;
        public int CurrentBusLanes;
        public int MaximumBusLanes;
        public int Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLTemperature {
        public int Size;
        public int Temperature;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLFanSpeedValue {
        public int Size;
        public int SpeedType;
        public int FanSpeed;
        public int Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLFanSpeedInfo {
        public int Size;
        public int Flags;
        public int MinPercent;
        public int MaxPercent;
        public int MinRPM;
        public int MaxRPM;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLMemoryInfo {
        public ulong MemorySize;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
        public string MemoryType;
        public long MemoryBandwidth;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODNPowerLimitSetting {
        public int iMode;
        public int iTDPLimit;
        public int iMaxOperatingTemperature;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ADLVersionsInfoX2 {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
        public string strDriverVer;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
        public string strCatalystVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
        public string strCrimsonVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ADL.ADL_MAX_PATH)]
        public string strCatalystWebLink;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ADLODNPerformanceLevelX2 {
        public int iClock;
        public int iVddc;
        public int iEnabled;
        public int iControl;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ADLODNPerformanceLevelsX2 {
        public static ADLODNPerformanceLevelsX2 Create() {
            ADLODNPerformanceLevelsX2 lpODPerformanceLevels = new ADLODNPerformanceLevelsX2 {
                aLevels = new ADLODNPerformanceLevelX2[ADL.ADL_PERFORMANCE_LEVELS]
            };
            for (int i = 0; i < lpODPerformanceLevels.aLevels.Length; i++) {
                lpODPerformanceLevels.aLevels[i] = new ADLODNPerformanceLevelX2();
            }
            lpODPerformanceLevels.iSize = Marshal.SizeOf(typeof(ADLODNPerformanceLevelsX2));
            lpODPerformanceLevels.iNumberOfPerformanceLevels = ADL.ADL_PERFORMANCE_LEVELS;

            return lpODPerformanceLevels;
        }

        public int iSize;
        public int iMode;
        public int iNumberOfPerformanceLevels;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ADL.ADL_PERFORMANCE_LEVELS)]
        public ADLODNPerformanceLevelX2[] aLevels;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ADLODNParameterRange {
        public int iMode;
        public int iMin;
        public int iMax;
        public int iStep;
        public int iDefault;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ADLODNCapabilitiesX2 {
        public static ADLODNCapabilitiesX2 Create() {
            return new ADLODNCapabilitiesX2 {
                sEngineClockRange = new ADLODNParameterRange(),
                sMemoryClockRange = new ADLODNParameterRange(),
                svddcRange = new ADLODNParameterRange(),
                power = new ADLODNParameterRange(),
                powerTuneTemperature = new ADLODNParameterRange(),
                fanTemperature = new ADLODNParameterRange(),
                fanSpeed = new ADLODNParameterRange(),
                minimumPerformanceClock = new ADLODNParameterRange(),
                throttleNotificaion = new ADLODNParameterRange(),
                autoSystemClock = new ADLODNParameterRange()
            };
        }

        public int iMaximumNumberOfPerformanceLevels;
        public int iFlags;
        public ADLODNParameterRange sEngineClockRange;
        public ADLODNParameterRange sMemoryClockRange;
        public ADLODNParameterRange svddcRange;
        public ADLODNParameterRange power;
        public ADLODNParameterRange powerTuneTemperature;
        public ADLODNParameterRange fanTemperature;
        public ADLODNParameterRange fanSpeed;
        public ADLODNParameterRange minimumPerformanceClock;
        public ADLODNParameterRange throttleNotificaion;
        public ADLODNParameterRange autoSystemClock;
    }

    internal class ADL {
        public const int ADL_PERFORMANCE_LEVELS = 8;
        public const int ADL_MAX_PATH = 256;
        public const int ADL_MAX_ADAPTERS = 40;
        public const int ADL_MAX_DISPLAYS = 40;
        public const int ADL_MAX_DEVICENAME = 32;
        public const int ADL_OK = 0;
        public const int ADL_ERR = -1;
        public const int ADL_DRIVER_OK = 0;
        public const int ADL_MAX_GLSYNC_PORTS = 8;
        public const int ADL_MAX_GLSYNC_PORT_LEDS = 8;
        public const int ADL_MAX_NUM_DISPLAYMODES = 1024;

        public const int ADL_DL_FANCTRL_SPEED_TYPE_PERCENT = 1;
        public const int ADL_DL_FANCTRL_SPEED_TYPE_RPM = 2;

        public const int ADL_DL_FANCTRL_SUPPORTS_PERCENT_READ = 1;
        public const int ADL_DL_FANCTRL_SUPPORTS_PERCENT_WRITE = 2;
        public const int ADL_DL_FANCTRL_SUPPORTS_RPM_READ = 4;
        public const int ADL_DL_FANCTRL_SUPPORTS_RPM_WRITE = 8;
        public const int ADL_DL_FANCTRL_FLAG_USER_DEFINED_SPEED = 1;

        public const int ODNControlType_Current = 0;
        public const int ODNControlType_Default = 1;
        public const int ODNControlType_Auto = 2;
        public const int ODNControlType_Manual = 3;

        public const int ATI_VENDOR_ID = 0x1002;

        private delegate int ADL_Main_Control_CreateDelegate(ADL_Main_Memory_AllocDelegate callback, int enumConnectedAdapters);
        public delegate int ADL2_Main_Control_CreateDelegate(ADL_Main_Memory_AllocDelegate callback, int enumConnectedAdapters, ref IntPtr context);
        private delegate int ADL_Adapter_AdapterInfo_GetDelegate(IntPtr info, int size);

        internal delegate int ADL_Main_Control_DestroyDelegate();
        internal delegate int ADL_Adapter_NumberOfAdapters_GetDelegate(ref int numAdapters);
        internal delegate int ADL_Adapter_Active_GetDelegate(int adapterIndex, out int status);
        internal delegate int ADL_Overdrive5_CurrentActivity_GetDelegate(int iAdapterIndex, ref ADLPMActivity activity);
        internal delegate int ADL_Overdrive5_Temperature_GetDelegate(int adapterIndex, int thermalControllerIndex, ref ADLTemperature temperature);
        internal delegate int ADL_Overdrive5_FanSpeed_GetDelegate(int adapterIndex, int thermalControllerIndex, ref ADLFanSpeedValue fanSpeedValue);
        internal delegate int ADL_Overdrive5_FanSpeedInfo_GetDelegate(int adapterIndex, int thermalControllerIndex, ref ADLFanSpeedInfo fanSpeedInfo);
        internal delegate int ADL_Overdrive5_FanSpeedToDefault_SetDelegate(int adapterIndex, int thermalControllerIndex);
        internal delegate int ADL_Overdrive5_FanSpeed_SetDelegate(int adapterIndex, int thermalControllerIndex, ref ADLFanSpeedValue fanSpeedValue);
        internal delegate int ADL2_OverdriveN_PowerLimit_GetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPowerLimitSetting lpODPowerLimit);
        internal delegate int ADL2_OverdriveN_PowerLimit_SetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPowerLimitSetting lpODPowerLimit);
        internal delegate int ADL2_Overdrive6_CurrentPower_GetDelegate(IntPtr context, int iAdapterIndex, int iPowerType, ref int lpCurrentValue);
        internal delegate int ADL_Adapter_MemoryInfo_GetDelegate(int iAdapterIndex, ref ADLMemoryInfo lpMemoryInfo);
        internal delegate int ADL2_Graphics_VersionsX2_GetDelegate(IntPtr context, ref ADLVersionsInfoX2 lpVersionsInfo);
        internal delegate int ADL2_OverdriveN_MemoryClocksX2_GetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPerformanceLevelsX2 lpODPerformanceLevels);
        internal delegate int ADL2_OverdriveN_MemoryClocksX2_SetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPerformanceLevelsX2 lpODPerformanceLevels);
        internal delegate int ADL2_OverdriveN_SystemClocksX2_GetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPerformanceLevelsX2 lpODPerformanceLevels);
        internal delegate int ADL2_OverdriveN_SystemClocksX2_SetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPerformanceLevelsX2 lpODPerformanceLevels);
        internal delegate int ADL2_OverdriveN_CapabilitiesX2_GetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNCapabilitiesX2 lpODCapabilities);

        private static ADL_Main_Control_CreateDelegate _ADL_Main_Control_Create;
        public static ADL2_Main_Control_CreateDelegate ADL2_Main_Control_Create;
        private static ADL_Adapter_AdapterInfo_GetDelegate _ADL_Adapter_AdapterInfo_Get;

        public static ADL_Main_Control_DestroyDelegate ADL_Main_Control_Destroy;
        public static ADL_Adapter_NumberOfAdapters_GetDelegate ADL_Adapter_NumberOfAdapters_Get;
        public static ADL_Adapter_Active_GetDelegate ADL_Adapter_Active_Get;
        public static ADL_Overdrive5_CurrentActivity_GetDelegate ADL_Overdrive5_CurrentActivity_Get;
        public static ADL_Overdrive5_Temperature_GetDelegate ADL_Overdrive5_Temperature_Get;
        public static ADL_Overdrive5_FanSpeed_GetDelegate ADL_Overdrive5_FanSpeed_Get;
        public static ADL_Overdrive5_FanSpeedInfo_GetDelegate ADL_Overdrive5_FanSpeedInfo_Get;
        public static ADL_Overdrive5_FanSpeedToDefault_SetDelegate ADL_Overdrive5_FanSpeedToDefault_Set;
        public static ADL_Overdrive5_FanSpeed_SetDelegate ADL_Overdrive5_FanSpeed_Set;
        public static ADL2_OverdriveN_PowerLimit_GetDelegate ADL2_OverdriveN_PowerLimit_Get;
        public static ADL2_Overdrive6_CurrentPower_GetDelegate ADL2_Overdrive6_CurrentPower_Get;
        public static ADL2_OverdriveN_PowerLimit_SetDelegate ADL2_OverdriveN_PowerLimit_Set;
        public static ADL_Adapter_MemoryInfo_GetDelegate ADL_Adapter_MemoryInfo_Get;
        public static ADL2_Graphics_VersionsX2_GetDelegate ADL2_Graphics_VersionsX2_Get;
        public static ADL2_OverdriveN_MemoryClocksX2_GetDelegate ADL2_OverdriveN_MemoryClocksX2_Get;
        public static ADL2_OverdriveN_MemoryClocksX2_SetDelegate ADL2_OverdriveN_MemoryClocksX2_Set;
        public static ADL2_OverdriveN_SystemClocksX2_GetDelegate ADL2_OverdriveN_SystemClocksX2_Get;
        public static ADL2_OverdriveN_SystemClocksX2_SetDelegate ADL2_OverdriveN_SystemClocksX2_Set;
        public static ADL2_OverdriveN_CapabilitiesX2_GetDelegate ADL2_OverdriveN_CapabilitiesX2_Get;

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
            GetDelegate(nameof(ADL_Adapter_Active_Get), out ADL_Adapter_Active_Get);
            GetDelegate(nameof(ADL_Overdrive5_CurrentActivity_Get), out ADL_Overdrive5_CurrentActivity_Get);
            GetDelegate(nameof(ADL_Overdrive5_Temperature_Get), out ADL_Overdrive5_Temperature_Get);
            GetDelegate(nameof(ADL_Overdrive5_FanSpeed_Get), out ADL_Overdrive5_FanSpeed_Get);
            GetDelegate(nameof(ADL_Overdrive5_FanSpeedInfo_Get), out ADL_Overdrive5_FanSpeedInfo_Get);
            GetDelegate(nameof(ADL_Overdrive5_FanSpeedToDefault_Set), out ADL_Overdrive5_FanSpeedToDefault_Set);
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
        }

        static ADL() {
            CreateDelegates("atiadlxx");
        }

        private ADL() { }

        public static int ADL_Main_Control_Create(int enumConnectedAdapters) {
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
                return ADL_ERR;
            }
        }

        public static int ADL_Adapter_AdapterInfo_Get(ADLAdapterInfo[] info) {
            int elementSize = Marshal.SizeOf(typeof(ADLAdapterInfo));
            int size = info.Length * elementSize;
            IntPtr ptr = Marshal.AllocHGlobal(size);
            int result = _ADL_Adapter_AdapterInfo_Get(ptr, size);
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

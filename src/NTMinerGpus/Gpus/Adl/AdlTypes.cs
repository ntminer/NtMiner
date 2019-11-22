using System.Runtime.InteropServices;

namespace NTMiner.Gpus.Adl {
    internal enum AdlStatus {
        ADL_OK_WAIT = 4,
        ADL_OK_RESTART = 3,
        ADL_OK_MODE_CHANGE = 2,
        ADL_OK_WARNING = 1,
        ADL_OK = 0,
        ADL_ERR = -1,
        ADL_ERR_NOT_INIT = -2,
        ADL_ERR_INVALID_PARAM = -3,
        ADL_ERR_INVALID_PARAM_SIZE = -4,
        ADL_ERR_INVALID_ADL_IDX = -5,
        ADL_ERR_INVALID_CONTROLLER_IDX = -6,
        ADL_ERR_INVALID_DIPLAY_IDX = -7,
        ADL_ERR_NOT_SUPPORTED = -8,
        ADL_ERR_NULL_POINTER = -9,
        ADL_ERR_DISABLED_ADAPTER = -10,
        ADL_ERR_INVALID_CALLBACK = -11,
        ADL_ERR_RESOURCE_CONFLICT = -12,
        ADL_ERR_SET_INCOMPLETE = -20,
        ADL_ERR_NO_XDISPLAY = -21
    }

    internal static class AdlConst {
        public const int ADL_PERFORMANCE_LEVELS = 8;
        public const int ADL_MAX_PATH = 256;
        public const int ADL_MAX_ADAPTERS = 40;
        public const int ADL_MAX_DISPLAYS = 40;
        public const int ADL_MAX_DEVICENAME = 32;

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
    }
    internal enum ADLODNControlType {
        ODNControlType_Current = 0,
        ODNControlType_Default = 1,
        ODNControlType_Auto = 2,
        ODNControlType_Manual = 3
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLAdapterInfo {
        public int Size;
        public int AdapterIndex;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlConst.ADL_MAX_PATH)]
        public string UDID;
        public int BusNumber;
        public int DeviceNumber;
        public int FunctionNumber;
        public int VendorID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlConst.ADL_MAX_PATH)]
        public string AdapterName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlConst.ADL_MAX_PATH)]
        public string DisplayName;
        public int Present;
        public int Exist;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlConst.ADL_MAX_PATH)]
        public string DriverPath;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlConst.ADL_MAX_PATH)]
        public string DriverPathExt;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlConst.ADL_MAX_PATH)]
        public string PNPString;
        public int OSDisplayIndex;

        public override string ToString() {
            return $"Size={Size.ToString()},AdapterIndex={AdapterIndex.ToString()},UDID={UDID},BusNumber={BusNumber.ToString()},DeviceNumber={DeviceNumber.ToString()},FunctionNumber={FunctionNumber.ToString()},VendorID={VendorID.ToString()},AdapterName={AdapterName},DisplayName={DisplayName},Present={Present.ToString()},Exist={Exist.ToString()},PNPString={PNPString},OSDisplayIndex={OSDisplayIndex.ToString()}";
        }
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
    internal struct ADLMemoryInfo {
        public ulong MemorySize;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlConst.ADL_MAX_PATH)]
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
    internal struct ADLVersionsInfoX2 {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlConst.ADL_MAX_PATH)]
        public string strDriverVer;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlConst.ADL_MAX_PATH)]
        public string strCatalystVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlConst.ADL_MAX_PATH)]
        public string strCrimsonVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlConst.ADL_MAX_PATH)]
        public string strCatalystWebLink;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODNPerformanceLevelX2 {
        public int iClock;
        public int iVddc;
        public int iEnabled;
        public int iControl;

        public override string ToString() {
            return $"iClock:{iClock.ToString()},iVddc:{iVddc.ToString()},iEnabled={iEnabled.ToString()},iControl={iControl.ToString()}";
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODNPerformanceLevelsX2 {
        public static ADLODNPerformanceLevelsX2 Create() {
            ADLODNPerformanceLevelsX2 lpODPerformanceLevels = new ADLODNPerformanceLevelsX2 {
                aLevels = new ADLODNPerformanceLevelX2[AdlConst.ADL_PERFORMANCE_LEVELS]
            };
            for (int i = 0; i < lpODPerformanceLevels.aLevels.Length; i++) {
                lpODPerformanceLevels.aLevels[i] = new ADLODNPerformanceLevelX2();
            }
            lpODPerformanceLevels.iSize = Marshal.SizeOf(typeof(ADLODNPerformanceLevelsX2));
            lpODPerformanceLevels.iNumberOfPerformanceLevels = AdlConst.ADL_PERFORMANCE_LEVELS;

            return lpODPerformanceLevels;
        }

        public int iSize;
        public int iMode;
        public int iNumberOfPerformanceLevels;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = AdlConst.ADL_PERFORMANCE_LEVELS)]
        public ADLODNPerformanceLevelX2[] aLevels;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODNParameterRange {
        public int iMode;
        public int iMin;
        public int iMax;
        public int iStep;
        public int iDefault;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODNCapabilitiesX2 {
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
}

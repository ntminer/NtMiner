using System.Runtime.InteropServices;

namespace NTMiner.Gpus.Adl {
    internal enum AdlStatus {
        OK = 0,
        ERR = -1
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
    struct ADLVersionsInfoX2 {
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
}

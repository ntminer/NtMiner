using System.Runtime.InteropServices;

namespace NTMiner.Gpus.Adl {
    public enum AdlStatus {
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
        public const int ADL_PMLOG_MAX_SENSORS = 256;

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
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLTemperature {
        public int Size;
        public int Temperature;

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }

    internal enum ADLODNTemperatureType {
        CORE = 1,
        MEMORY = 2,
        VRM_CORE = 3,
        VRM_MEMORY = 4,
        LIQUID = 5,
        PLX = 6,
        HOTSPOT = 7,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLFanSpeedValue {
        public int Size;
        public int SpeedType;
        public int FanSpeed;
        public int Flags;

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLMemoryInfo {
        public ulong MemorySize;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlConst.ADL_MAX_PATH)]
        public string MemoryType;
        public long MemoryBandwidth;

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODNPowerLimitSetting {
        public int iMode;
        public int iTDPLimit;
        public int iMaxOperatingTemperature;

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }

    internal enum ADLODNCurrentPowerType {
        TOTAL_POWER = 0,
        PPT_POWER,
        SOCKET_POWER,
        CHIP_POWER,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLSingleSensorData {
        public int Supported;
        public int Value;

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLPMLogDataOutput {
        public int Size;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = AdlConst.ADL_PMLOG_MAX_SENSORS)]
        public ADLSingleSensorData[] Sensors;

        public static ADLPMLogDataOutput Create() {
            ADLPMLogDataOutput r = new ADLPMLogDataOutput {
                Sensors = new ADLSingleSensorData[AdlConst.ADL_PMLOG_MAX_SENSORS]
            };
            for (int i = 0; i < r.Sensors.Length; i++) {
                r.Sensors[i] = new ADLSingleSensorData();
            }
            return r;
        }

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }

    internal enum ADLSensorType {
        SENSOR_MAXTYPES = 0,
        PMLOG_CLK_GFXCLK = 1,
        PMLOG_CLK_MEMCLK = 2,
        PMLOG_CLK_SOCCLK = 3,
        PMLOG_CLK_UVDCLK1 = 4,
        PMLOG_CLK_UVDCLK2 = 5,
        PMLOG_CLK_VCECLK = 6,
        PMLOG_CLK_VCNCLK = 7,
        PMLOG_TEMPERATURE_EDGE = 8,
        PMLOG_TEMPERATURE_MEM = 9,
        PMLOG_TEMPERATURE_VRVDDC = 10,
        PMLOG_TEMPERATURE_VRMVDD = 11,
        PMLOG_TEMPERATURE_LIQUID = 12,
        PMLOG_TEMPERATURE_PLX = 13,
        PMLOG_FAN_RPM = 14,
        PMLOG_FAN_PERCENTAGE = 15,
        PMLOG_SOC_VOLTAGE = 16,
        PMLOG_SOC_POWER = 17,
        PMLOG_SOC_CURRENT = 18,
        PMLOG_INFO_ACTIVITY_GFX = 19,
        PMLOG_INFO_ACTIVITY_MEM = 20,
        PMLOG_GFX_VOLTAGE = 21,
        PMLOG_MEM_VOLTAGE = 22,
        PMLOG_ASIC_POWER = 23,
        PMLOG_TEMPERATURE_VRSOC = 24,
        PMLOG_TEMPERATURE_VRMVDD0 = 25,
        PMLOG_TEMPERATURE_VRMVDD1 = 26,
        PMLOG_TEMPERATURE_HOTSPOT = 27,
        PMLOG_TEMPERATURE_GFX = 28,
        PMLOG_TEMPERATURE_SOC = 29,
        PMLOG_GFX_POWER = 30,
        PMLOG_GFX_CURRENT = 31,
        PMLOG_TEMPERATURE_CPU = 32,
        PMLOG_CPU_POWER = 33,
        PMLOG_CLK_CPUCLK = 34,
        PMLOG_THROTTLER_STATUS = 35,
        PMLOG_CLK_VCN1CLK1 = 36,
        PMLOG_CLK_VCN1CLK2 = 37,
        PMLOG_SMART_POWERSHIFT_CPU = 38,
        PMLOG_SMART_POWERSHIFT_DGPU = 39,
        PMLOG_MAX_SENSORS_REAL
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLVersionsInfoX2 {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlConst.ADL_MAX_PATH)]
        public string strDriverVer;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlConst.ADL_MAX_PATH)]
        public string strCatalystVersion;
        // 这个才是版本号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlConst.ADL_MAX_PATH)]
        public string strCrimsonVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlConst.ADL_MAX_PATH)]
        public string strCatalystWebLink;

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODNPerformanceLevelX2 {
        public int iClock;
        public int iVddc;
        public int iEnabled;
        public int iControl;

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODNPerformanceLevelsX2 {
        public static ADLODNPerformanceLevelsX2 Create() {
            ADLODNPerformanceLevelsX2 r = new ADLODNPerformanceLevelsX2 {
                aLevels = new ADLODNPerformanceLevelX2[AdlConst.ADL_PERFORMANCE_LEVELS]
            };
            for (int i = 0; i < r.aLevels.Length; i++) {
                r.aLevels[i] = new ADLODNPerformanceLevelX2();
            }
            r.iSize = Marshal.SizeOf(typeof(ADLODNPerformanceLevelsX2));
            r.iNumberOfPerformanceLevels = AdlConst.ADL_PERFORMANCE_LEVELS;

            return r;
        }

        public int iSize;
        public int iMode;
        public int iNumberOfPerformanceLevels;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = AdlConst.ADL_PERFORMANCE_LEVELS)]
        public ADLODNPerformanceLevelX2[] aLevels;

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLODNParameterRange {
        public int iMode;
        public int iMin;
        public int iMax;
        public int iStep;
        public int iDefault;

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
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

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }

    //OD8 Capability features bits
    enum ADLOD8FeatureControl {
        ADL_OD8_GFXCLK_LIMITS = 1 << 0,
        ADL_OD8_GFXCLK_CURVE = 1 << 1,
        ADL_OD8_UCLK_MAX = 1 << 2,
        ADL_OD8_POWER_LIMIT = 1 << 3,
        ADL_OD8_ACOUSTIC_LIMIT_SCLK = 1 << 4,   //FanMaximumRpm
        ADL_OD8_FAN_SPEED_MIN = 1 << 5,   //FanMinimumPwm
        ADL_OD8_TEMPERATURE_FAN = 1 << 6,   //FanTargetTemperature
        ADL_OD8_TEMPERATURE_SYSTEM = 1 << 7,    //MaxOpTemp
        ADL_OD8_MEMORY_TIMING_TUNE = 1 << 8,
        ADL_OD8_FAN_ZERO_RPM_CONTROL = 1 << 9,
        ADL_OD8_AUTO_UV_ENGINE = 1 << 10,  //Auto under voltage
        ADL_OD8_AUTO_OC_ENGINE = 1 << 11,  //Auto overclock engine     
        ADL_OD8_AUTO_OC_MEMORY = 1 << 12,  //Auto overclock memory
        ADL_OD8_FAN_CURVE = 1 << 13,   //Fan curve
        ADL_OD8_WS_AUTO_FAN_ACOUSTIC_LIMIT = 1 << 14, //Workstation Manual Fan controller
        ADL_OD8_POWER_GAUGE = 1 << 15 //Power Gauge
    };

    internal enum ADLOD8SettingId {
        OD8_GFXCLK_FMAX = 0,
        OD8_GFXCLK_FMIN,
        OD8_GFXCLK_FREQ1,
        OD8_GFXCLK_VOLTAGE1,
        OD8_GFXCLK_FREQ2,
        OD8_GFXCLK_VOLTAGE2,
        OD8_GFXCLK_FREQ3,
        OD8_GFXCLK_VOLTAGE3,
        OD8_UCLK_FMAX,
        OD8_POWER_PERCENTAGE,
        OD8_FAN_MIN_SPEED,
        OD8_FAN_ACOUSTIC_LIMIT,
        OD8_FAN_TARGET_TEMP,
        OD8_OPERATING_TEMP_MAX,
        OD8_AC_TIMING,
        OD8_FAN_ZERORPM_CONTROL,
        OD8_AUTO_UV_ENGINE_CONTROL,
        OD8_AUTO_OC_ENGINE_CONTROL,
        OD8_AUTO_OC_MEMORY_CONTROL,
        OD8_FAN_CURVE_TEMPERATURE_1,
        OD8_FAN_CURVE_SPEED_1,
        OD8_FAN_CURVE_TEMPERATURE_2,
        OD8_FAN_CURVE_SPEED_2,
        OD8_FAN_CURVE_TEMPERATURE_3,
        OD8_FAN_CURVE_SPEED_3,
        OD8_FAN_CURVE_TEMPERATURE_4,
        OD8_FAN_CURVE_SPEED_4,
        OD8_FAN_CURVE_TEMPERATURE_5,
        OD8_FAN_CURVE_SPEED_5,
        OD8_COUNT
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ADLOD8CurrentSetting {
        public int count;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)ADLOD8SettingId.OD8_COUNT)]
        public int[] Od8SettingTable;

        public static ADLOD8CurrentSetting Create() {
            ADLOD8CurrentSetting r = new ADLOD8CurrentSetting {
                count = (int)ADLOD8SettingId.OD8_COUNT,
                Od8SettingTable = new int[(int)ADLOD8SettingId.OD8_COUNT]
            };
            return r;
        }

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLOD8SingleInitSetting {
        public int featureID;
        public int minValue;
        public int maxValue;
        public int defaultValue;

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ADLOD8SingleSetSetting {
        public int value;
        public int requested;      // 0 - default , 1 - requested
        public int reset;          // 0 - do not reset , 1 - reset setting back to default

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ADLOD8SetSetting {
        public int count;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)ADLOD8SettingId.OD8_COUNT)]
        public ADLOD8SingleSetSetting[] od8SettingTable;

        public static ADLOD8SetSetting Create() {
            ADLOD8SetSetting r = new ADLOD8SetSetting {
                count = (int)ADLOD8SettingId.OD8_COUNT,
                od8SettingTable = new ADLOD8SingleSetSetting[(int)ADLOD8SettingId.OD8_COUNT]
            };
            for (int i = 0; i < r.od8SettingTable.Length; i++) {
                r.od8SettingTable[i] = new ADLOD8SingleSetSetting();
            }

            return r;
        }

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ADLOD8InitSetting {
        public int count;
        public int overdrive8Capabilities;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)ADLOD8SettingId.OD8_COUNT)]
        public ADLOD8SingleInitSetting[] od8SettingTable;

        public static ADLOD8InitSetting Create() {
            ADLOD8InitSetting r = new ADLOD8InitSetting {
                count = (int)ADLOD8SettingId.OD8_COUNT,
                od8SettingTable = new ADLOD8SingleInitSetting[(int)ADLOD8SettingId.OD8_COUNT]
            };
            return r;
        }

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }
}

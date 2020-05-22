using System;
using System.Runtime.InteropServices;
using NvS32 = System.Int32;
using NvU32 = System.UInt32;

namespace NTMiner.Gpus.Nvapi {
    internal static class NvapiConst {
        internal const int MAX_PHYSICAL_GPUS = 64;
        internal const int MAX_PSTATES_PER_GPU = 8;
        internal const int MAX_COOLER_PER_GPU = 20;
        internal const int MAX_THERMAL_SENSORS_PER_GPU = 3;
        internal const int MAX_POWER_ENTRIES_PER_GPU = 4;

        internal const int NVAPI_MAX_GPU_CLOCKS = 32;
        internal const int NVAPI_MAX_GPU_PUBLIC_CLOCKS = 32;
        internal const int NVAPI_MAX_GPU_PERF_CLOCKS = 32;
        internal const int NVAPI_MAX_GPU_PERF_VOLTAGES = 16;
        internal const int NVAPI_MAX_GPU_PERF_PSTATES = 16;

        internal const int NVAPI_MAX_GPU_PSTATE20_PSTATES = 16;
        internal const int NVAPI_MAX_GPU_PSTATE20_CLOCKS = 8;
        internal const int NVAPI_MAX_GPU_PSTATE20_BASE_VOLTAGES = 4;

        internal const int NV_GPU_CLOCK_FREQUENCIES_CURRENT_FREQ = 0;
        internal const int NV_GPU_CLOCK_FREQUENCIES_BASE_CLOCK = 1;
        internal const int NV_GPU_CLOCK_FREQUENCIES_BOOST_CLOCK = 2;
        internal const int NV_GPU_CLOCK_FREQUENCIES_CLOCK_TYPE_NUM = 3;
        internal const int NV_GPU_CLOCK_FREQUENCIES_CLOCK_TYPE = 4;

        internal const int NVAPI_MAX_COOLERS_PER_GPU = 3;
        internal const int MaxNumberOfFanCoolerControlEntries = 32;
        internal const int MaxNumberOfFanCoolerStatusEntries = 32;
        internal const int MaxNumberOfFanCoolerInfoEntries = 32;
    }

    #region Enumms
    internal enum NvStatus {
        NVAPI_OK = 0, 
        NVAPI_ERROR = -1, 
        NVAPI_LIBRARY_NOT_FOUND = -2, 
        NVAPI_NO_IMPLEMENTATION = -3,
        NVAPI_API_NOT_INITIALIZED = -4, 
        NVAPI_INVALID_ARGUMENT = -5, 
        NVAPI_NVIDIA_DEVICE_NOT_FOUND = -6, 
        NVAPI_END_ENUMERATION = -7,
        NVAPI_INVALID_HANDLE = -8, 
        NVAPI_INCOMPATIBLE_STRUCT_VERSION = -9, 
        NVAPI_HANDLE_INVALIDATED = -10, 
        NVAPI_OPENGL_CONTEXT_NOT_CURRENT = -11,
        NVAPI_INVALID_POINTER = -14, 
        NVAPI_NO_GL_EXPERT = -12, 
        NVAPI_INSTRUMENTATION_DISABLED = -13, 
        NVAPI_NO_GL_NSIGHT = -15,
        NVAPI_EXPECTED_LOGICAL_GPU_HANDLE = -100, 
        NVAPI_EXPECTED_PHYSICAL_GPU_HANDLE = -101, 
        NVAPI_EXPECTED_DISPLAY_HANDLE = -102, 
        NVAPI_INVALID_COMBINATION = -103,
        NVAPI_NOT_SUPPORTED = -104, 
        NVAPI_PORTID_NOT_FOUND = -105, 
        NVAPI_EXPECTED_UNATTACHED_DISPLAY_HANDLE = -106, 
        NVAPI_INVALID_PERF_LEVEL = -107,
        NVAPI_DEVICE_BUSY = -108,
        NVAPI_NV_PERSIST_FILE_NOT_FOUND = -109, 
        NVAPI_PERSIST_DATA_NOT_FOUND = -110, 
        NVAPI_EXPECTED_TV_DISPLAY = -111,
        NVAPI_EXPECTED_TV_DISPLAY_ON_DCONNECTOR = -112, 
        NVAPI_NO_ACTIVE_SLI_TOPOLOGY = -113, 
        NVAPI_SLI_RENDERING_MODE_NOTALLOWED = -114, 
        NVAPI_EXPECTED_DIGITAL_FLAT_PANEL = -115,
        NVAPI_ARGUMENT_EXCEED_MAX_SIZE = -116, 
        NVAPI_DEVICE_SWITCHING_NOT_ALLOWED = -117,
        NVAPI_TESTING_CLOCKS_NOT_SUPPORTED = -118, 
        NVAPI_UNKNOWN_UNDERSCAN_CONFIG = -119,
        NVAPI_TIMEOUT_RECONFIGURING_GPU_TOPO = -120, 
        NVAPI_DATA_NOT_FOUND = -121, 
        NVAPI_EXPECTED_ANALOG_DISPLAY = -122, 
        NVAPI_NO_VIDLINK = -123,
        NVAPI_REQUIRES_REBOOT = -124, 
        NVAPI_INVALID_HYBRID_MODE = -125, 
        NVAPI_MIXED_TARGET_TYPES = -126, 
        NVAPI_SYSWOW64_NOT_SUPPORTED = -127,
        NVAPI_IMPLICIT_SET_GPU_TOPOLOGY_CHANGE_NOT_ALLOWED = -128, 
        NVAPI_REQUEST_USER_TO_CLOSE_NON_MIGRATABLE_APPS = -129, 
        NVAPI_OUT_OF_MEMORY = -130, 
        NVAPI_WAS_STILL_DRAWING = -131,
        NVAPI_FILE_NOT_FOUND = -132, 
        NVAPI_TOO_MANY_UNIQUE_STATE_OBJECTS = -133, 
        NVAPI_INVALID_CALL = -134, 
        NVAPI_D3D10_1_LIBRARY_NOT_FOUND = -135,
        NVAPI_FUNCTION_NOT_FOUND = -136, 
        NVAPI_INVALID_USER_PRIVILEGE = -137, 
        NVAPI_EXPECTED_NON_PRIMARY_DISPLAY_HANDLE = -138, 
        NVAPI_EXPECTED_COMPUTE_GPU_HANDLE = -139,
        NVAPI_STEREO_NOT_INITIALIZED = -140, 
        NVAPI_STEREO_REGISTRY_ACCESS_FAILED = -141, 
        NVAPI_STEREO_REGISTRY_PROFILE_TYPE_NOT_SUPPORTED = -142, 
        NVAPI_STEREO_REGISTRY_VALUE_NOT_SUPPORTED = -143,
        NVAPI_STEREO_NOT_ENABLED = -144, 
        NVAPI_STEREO_NOT_TURNED_ON = -145, 
        NVAPI_STEREO_INVALID_DEVICE_INTERFACE = -146, 
        NVAPI_STEREO_PARAMETER_OUT_OF_RANGE = -147,
        NVAPI_STEREO_FRUSTUM_ADJUST_MODE_NOT_SUPPORTED = -148, 
        NVAPI_TOPO_NOT_POSSIBLE = -149, 
        NVAPI_MODE_CHANGE_FAILED = -150, 
        NVAPI_D3D11_LIBRARY_NOT_FOUND = -151,
        NVAPI_INVALID_ADDRESS = -152, 
        NVAPI_STRING_TOO_SMALL = -153, 
        NVAPI_MATCHING_DEVICE_NOT_FOUND = -154, 
        NVAPI_DRIVER_RUNNING = -155,
        NVAPI_DRIVER_NOTRUNNING = -156, 
        NVAPI_ERROR_DRIVER_RELOAD_REQUIRED = -157, 
        NVAPI_SET_NOT_ALLOWED = -158, 
        NVAPI_ADVANCED_DISPLAY_TOPOLOGY_REQUIRED = -159,
        NVAPI_SETTING_NOT_FOUND = -160, 
        NVAPI_SETTING_SIZE_TOO_LARGE = -161, 
        NVAPI_TOO_MANY_SETTINGS_IN_PROFILE = -162, 
        NVAPI_PROFILE_NOT_FOUND = -163,
        NVAPI_PROFILE_NAME_IN_USE = -164, 
        NVAPI_PROFILE_NAME_EMPTY = -165, 
        NVAPI_EXECUTABLE_NOT_FOUND = -166, 
        NVAPI_EXECUTABLE_ALREADY_IN_USE = -167,
        NVAPI_DATATYPE_MISMATCH = -168, 
        NVAPI_PROFILE_REMOVED = -169, 
        NVAPI_UNREGISTERED_RESOURCE = -170, 
        NVAPI_ID_OUT_OF_RANGE = -171,
        NVAPI_DISPLAYCONFIG_VALIDATION_FAILED = -172, 
        NVAPI_DPMST_CHANGED = -173, 
        NVAPI_INSUFFICIENT_BUFFER = -174, 
        NVAPI_ACCESS_DENIED = -175,
        NVAPI_MOSAIC_NOT_ACTIVE = -176, 
        NVAPI_SHARE_RESOURCE_RELOCATED = -177, 
        NVAPI_REQUEST_USER_TO_DISABLE_DWM = -178, 
        NVAPI_D3D_DEVICE_LOST = -179,
        NVAPI_INVALID_CONFIGURATION = -180, 
        NVAPI_STEREO_HANDSHAKE_NOT_DONE = -181, 
        NVAPI_EXECUTABLE_PATH_IS_AMBIGUOUS = -182, 
        NVAPI_DEFAULT_STEREO_PROFILE_IS_NOT_DEFINED = -183,
        NVAPI_DEFAULT_STEREO_PROFILE_DOES_NOT_EXIST = -184,
        NVAPI_CLUSTER_ALREADY_EXISTS = -185, 
        NVAPI_DPMST_DISPLAY_ID_EXPECTED = -186,
        NVAPI_INVALID_DISPLAY_ID = -187,
        NVAPI_STREAM_IS_OUT_OF_SYNC = -188, 
        NVAPI_INCOMPATIBLE_AUDIO_DRIVER = -189, 
        NVAPI_VALUE_ALREADY_SET = -190, 
        NVAPI_TIMEOUT = -191,
        NVAPI_GPU_WORKSTATION_FEATURE_INCOMPLETE = -192,
        NVAPI_STEREO_INIT_ACTIVATION_NOT_DONE = -193, 
        NVAPI_SYNC_NOT_ACTIVE = -194, 
        NVAPI_SYNC_MASTER_NOT_FOUND = -195,
        NVAPI_INVALID_SYNC_TOPOLOGY = -196, 
        NVAPI_ECID_SIGN_ALGO_UNSUPPORTED = -197, 
        NVAPI_ECID_KEY_VERIFICATION_FAILED = -198, 
        NVAPI_FIRMWARE_OUT_OF_DATE = -199,
        NVAPI_FIRMWARE_REVISION_NOT_SUPPORTED = -200, 
        NVAPI_LICENSE_CALLER_AUTHENTICATION_FAILED = -201,
        NVAPI_D3D_DEVICE_NOT_REGISTERED = -202, 
        NVAPI_RESOURCE_NOT_ACQUIRED = -203,
        NVAPI_TIMING_NOT_SUPPORTED = -204, 
        NVAPI_HDCP_ENCRYPTION_FAILED = -205, 
        NVAPI_PCLK_LIMITATION_FAILED = -206, 
        NVAPI_NO_CONNECTOR_FOUND = -207,
        NVAPI_HDCP_DISABLED = -208, 
        NVAPI_API_IN_USE = -209, 
        NVAPI_NVIDIA_DISPLAY_NOT_FOUND = -210, 
        NVAPI_PRIV_SEC_VIOLATION = -211,
        NVAPI_INCORRECT_VENDOR = -212, 
        NVAPI_DISPLAY_IN_USE = -213, 
        NVAPI_UNSUPPORTED_CONFIG_NON_HDCP_HMD = -214,
        NVAPI_MAX_DISPLAY_LIMIT_REACHED = -215,
        NVAPI_INVALID_DIRECT_MODE_DISPLAY = -216, 
        NVAPI_GPU_IN_DEBUG_MODE = -217, 
        NVAPI_D3D_CONTEXT_NOT_FOUND = -218, 
        NVAPI_STEREO_VERSION_MISMATCH = -219,
        NVAPI_GPU_NOT_POWERED = -220, 
        NVAPI_ERROR_DRIVER_RELOAD_IN_PROGRESS = -221,
        NVAPI_WAIT_FOR_HW_RESOURCE = -222, 
        NVAPI_REQUIRE_FURTHER_HDCP_ACTION = -223,
        NVAPI_DISPLAY_MUX_TRANSITION_FAILED = -224
    }
    internal enum NvThermalController {
        NONE = 0,
        GPU_INTERNAL,
        ADM1032,
        MAX6649,
        MAX1617,
        LM99,
        LM89,
        LM64,
        ADT7473,
        SBMAX6649,
        VBIOSEVT,
        OS,
        UNKNOWN = -1,
    }
    internal enum NvThermalTarget {
        NONE = 0,
        GPU = 1,
        MEMORY = 2,
        POWER_SUPPLY = 4,
        BOARD = 8,
        ALL = 15,
        UNKNOWN = -1
    };
    internal enum NvGpuPublicClockId : NvU32 {
        NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS = 0,
        NVAPI_GPU_PUBLIC_CLOCK_MEMORY = 4,
        NVAPI_GPU_PUBLIC_CLOCK_PROCESSOR = 7,
        NVAPI_GPU_PUBLIC_CLOCK_VIDEO = 8,
        NVAPI_GPU_PUBLIC_CLOCK_UNDEFINED = NvapiConst.NVAPI_MAX_GPU_PUBLIC_CLOCKS,
    }
    internal enum NvGpuPerfPStateId : NvU32 {
        NVAPI_GPU_PERF_PSTATE_P0 = 0,
        NVAPI_GPU_PERF_PSTATE_P1,
        NVAPI_GPU_PERF_PSTATE_P2,
        NVAPI_GPU_PERF_PSTATE_P3,
        NVAPI_GPU_PERF_PSTATE_P4,
        NVAPI_GPU_PERF_PSTATE_P5,
        NVAPI_GPU_PERF_PSTATE_P6,
        NVAPI_GPU_PERF_PSTATE_P7,
        NVAPI_GPU_PERF_PSTATE_P8,
        NVAPI_GPU_PERF_PSTATE_P9,
        NVAPI_GPU_PERF_PSTATE_P10,
        NVAPI_GPU_PERF_PSTATE_P11,
        NVAPI_GPU_PERF_PSTATE_P12,
        NVAPI_GPU_PERF_PSTATE_P13,
        NVAPI_GPU_PERF_PSTATE_P14,
        NVAPI_GPU_PERF_PSTATE_P15,
        NVAPI_GPU_PERF_PSTATE_UNDEFINED = NvapiConst.NVAPI_MAX_GPU_PERF_PSTATES,
        NVAPI_GPU_PERF_PSTATE_ALL,
    }
    internal enum NvGpuPerfPState20ClockTypeId : NvU32 {
        NVAPI_GPU_PERF_PSTATE20_CLOCK_TYPE_SINGLE = 0,
        NVAPI_GPU_PERF_PSTATE20_CLOCK_TYPE_RANGE,
    }
    internal enum NvGpuPerfVoltageInfoDomainId : NvU32 {
        NVAPI_GPU_PERF_VOLTAGE_INFO_DOMAIN_CORE = 0,
        NVAPI_GPU_PERF_VOLTAGE_INFO_DOMAIN_UNDEFINED = NvapiConst.NVAPI_MAX_GPU_PERF_VOLTAGES,
    }
    /// <summary>
    ///     Holds possible fan cooler control modes
    /// </summary>
    [Flags]
    internal enum FanCoolersControlMode : uint {
        /// <summary>
        ///     Automatic fan cooler control
        /// </summary>
        Auto = 0,

        /// <summary>
        ///     Manual fan cooler control
        /// </summary>
        Manual = 0b1,
    }

    internal enum NvCoolerType : NvU32 {
        NVAPI_COOLER_TYPE_NONE = 0,
        NVAPI_COOLER_TYPE_FAN,
        NVAPI_COOLER_TYPE_WATER,
        NVAPI_COOLER_TYPE_LIQUID_NO2,
    }
    internal enum NvCoolerController {
        NVAPI_COOLER_CONTROLLER_NONE = 0,
        NVAPI_COOLER_CONTROLLER_ADI,
        NVAPI_COOLER_CONTROLLER_INTERNAL,
    }

    internal enum NvCoolerPolicy {
        NVAPI_COOLER_POLICY_NONE = 0,
        NVAPI_COOLER_POLICY_MANUAL = 1,                 // Manual adjustment of cooler level. Gets applied right away independent of temperature or performance level.
        NVAPI_COOLER_POLICY_PERF = 2,                   // GPU performance controls the cooler level.
        NVAPI_COOLER_POLICY_TEMPERATURE_DISCRETE = 4,   // Discrete thermal levels control the cooler level.
        NVAPI_COOLER_POLICY_TEMPERATURE_CONTINUOUS = 8, // Cooler level adjusted at continuous thermal levels.
        NVAPI_COOLER_POLICY_HYBRID = 16,                // Hybrid of performance and temperature levels.

        NVAPI_COOLER_POLICY_AUTO = 32,                  // AIMiner custom .
    }

    internal enum NvCoolerTarget {
        NVAPI_COOLER_TARGET_NONE = 0,
        NVAPI_COOLER_TARGET_GPU,
        NVAPI_COOLER_TARGET_MEMORY,
        NVAPI_COOLER_TARGET_POWER_SUPPLY = 4,
        NVAPI_COOLER_TARGET_ALL = 7                    // This cooler cools all of the components related to its target gpu.
    }

    internal enum NvCoolerControl {
        NVAPI_COOLER_CONTROL_NONE = 0,
        NVAPI_COOLER_CONTROL_TOGGLE,                   // ON/OFF
        NVAPI_COOLER_CONTROL_VARIABLE,                 // Suppports variable control.
    }

    internal enum NvCoolerActivityLevel {
        NVAPI_INACTIVE = 0,                             // inactive or unsupported
        NVAPI_ACTIVE = 1,                               // active and spinning in case of fan
    }

    #endregion

    #region Structs
    [StructLayout(LayoutKind.Sequential)]
    internal struct NvPhysicalGpuHandle {
        private readonly IntPtr ptr;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NvPState {
        public bool Present;
        public int Percentage;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NvPStates {
        public uint Version;
        public uint Flags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvapiConst.MAX_PSTATES_PER_GPU)]
        public NvPState[] PStates;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NvGpuPerfPStates20ParamDelta {
        public NvS32 value;
        public NvS32 mindelta;
        public NvS32 maxdelta;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct NvGpuSingleRangeDataUnion {
        [FieldOffset(0)]
        public NvU32 freq_kHz;

        [FieldOffset(0)]
        public NvU32 minFreq_kHz;
        [FieldOffset(4)]
        public NvU32 maxFreq_kHz;
        [FieldOffset(8)]
        public NvGpuPerfVoltageInfoDomainId domainId;
        [FieldOffset(12)]
        public NvU32 minVoltage_uV;
        [FieldOffset(16)]
        public NvU32 maxVoltage_uV;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NvGpuPState20ClockEntryV1 {
        public NvGpuPublicClockId domainId;
        public NvGpuPerfPState20ClockTypeId typeId;
        public NvU32 bIsEditable_reserved;
        public NvGpuPerfPStates20ParamDelta freqDelta_kHz;

        public NvGpuSingleRangeDataUnion data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NvGpuPState20BaseVoltageEntryV1 {
        public NvGpuPerfVoltageInfoDomainId domainId;
        public NvU32 bIsEditable_reserved;
        public NvU32 volt_uV;
        public NvGpuPerfPStates20ParamDelta voltDelta_uV;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PStatesArray16 {
        public NvGpuPerfPStateId pstateId;
        public NvU32 bIsEditable_reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvapiConst.NVAPI_MAX_GPU_PSTATE20_CLOCKS)]
        public NvGpuPState20ClockEntryV1[] clocks;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvapiConst.NVAPI_MAX_GPU_PSTATE20_BASE_VOLTAGES)]
        public NvGpuPState20BaseVoltageEntryV1[] baseVoltages;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NvGpuPState20V2Ov {
        public NvU32 numVoltages;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvapiConst.NVAPI_MAX_GPU_PSTATE20_BASE_VOLTAGES)]
        public NvGpuPState20BaseVoltageEntryV1[] voltages;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NvGpuPerfPStates20InfoV2 {
        public NvU32 version;
        public NvU32 bIsEditable_reserved;
        public NvU32 numPStates;
        public NvU32 numClocks;
        public NvU32 numBaseVoltages;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvapiConst.NVAPI_MAX_GPU_PERF_PSTATES)]
        public PStatesArray16[] pstates;
        public NvGpuPState20V2Ov ov;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NvGpuPerfPStates20InfoV1 {
        public NvU32 version;
        public NvU32 bIsEditable_reserved;
        public NvU32 numPStates;
        public NvU32 numClocks;
        public NvU32 numBaseVoltages;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvapiConst.NVAPI_MAX_GPU_PERF_PSTATES)]
        public PStatesArray16[] pstates;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NvGpuClockRrequenciesDomain {
        public NvU32 bIsPresent_reserved;
        public NvU32 frequency;
        public NvU32 bIsPresent {
            get {
                return bIsPresent_reserved & 0x01;
            }
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct NvGpuClockFrequenciesV2 {
        public NvU32 version;
        public NvU32 ClockType_reserved_reserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvapiConst.NVAPI_MAX_GPU_PUBLIC_CLOCKS)]
        public NvGpuClockRrequenciesDomain[] domain;
        public NvU32 ClockType {
            set {
                NvU32 tmp = ClockType_reserved_reserved1 & ~(NvU32)0x03;
                tmp |= value & 0x03;
                ClockType_reserved_reserved1 = tmp;
            }
            get {
                return ClockType_reserved_reserved1 & 0x03;
            }
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct NvGpuThermalInfoEntries {
        public NvU32 controller;
        public NvU32 unknown;
        public NvS32 min_temp;
        public NvS32 def_temp;
        public NvS32 max_temp;
        public NvU32 defaultFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NvGpuThermalInfo {
        public NvU32 version;
        public NvU32 flags;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public NvGpuThermalInfoEntries[] entries;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NvGpuThermalLimitEntries {
        public NvU32 controller;
        public NvU32 value;
        public NvU32 flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NvGpuThermalLimit {
        public NvU32 version;
        public NvU32 flags;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public NvGpuThermalLimitEntries[] entries;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct NvGpuPowerStatusEntry {
        public NvU32 unknown1;
        public NvU32 unknown2;

        public NvU32 power;
        public NvU32 unknown4;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NvGpuPowerStatus {
        public NvU32 version;
        public NvU32 flags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public NvGpuPowerStatusEntry[] entries;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NvGpuPowerInfoEntry {
        public NvU32 pstate;
        public NvU32 unknown1_0;
        public NvU32 unknown1_1;
        public NvU32 min_power;
        public NvU32 unknown2_0;
        public NvU32 unknown2_1;
        public NvU32 def_power;
        public NvU32 unknown3_0;
        public NvU32 unknown3_1;
        public NvU32 max_power;
        public NvU32 unknown4;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NvGpuPowerInfo {
        public NvU32 version;
        public NvU32 valid_count_reserver1_reserver2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public NvGpuPowerInfoEntry[] entries;

        public NvU32 valid {
            get {
                return valid_count_reserver1_reserver2 & 0xff;
            }
        }
        public NvU32 count {
            get {
                return (valid_count_reserver1_reserver2 >> 8) & 0xff;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NvCoolerItem {
        public NvCoolerType type;
        public NvCoolerController controller;
        public NvU32 defaultMinLevel;
        public NvU32 defaultMaxLevel;
        public NvU32 currentMinLevel;
        public NvU32 currentMaxLevel;
        public NvU32 currentLevel;
        public NvCoolerPolicy defaultPolicy;
        public NvCoolerPolicy currentPolicy;
        public NvCoolerTarget target;
        public NvCoolerControl controlType;
        public NvCoolerActivityLevel active;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NvCoolerSettings {
        public NvU32 version;
        public NvU32 count;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvapiConst.NVAPI_MAX_COOLERS_PER_GPU)]
        public NvCoolerItem[] cooler;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NvCoolerLevelItem {
        public NvU32 currentLevel;
        public NvCoolerPolicy currentPolicy;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NvCoolerLevel {
        public NvU32 version;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvapiConst.NVAPI_MAX_COOLERS_PER_GPU)]
        public NvCoolerLevelItem[] coolers;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct FanCoolersInfoEntry {
        internal readonly uint CoolerId;
        internal readonly uint UnknownUInt3;
        internal readonly uint UnknownUInt4;
        internal readonly uint MaximumRPM;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        internal readonly uint[] Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PrivateFanCoolersInfoV1 {
        public NvU32 version;
        internal readonly uint UnknownUInt1;
        internal readonly uint FanCoolersInfoCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        internal readonly uint[] Reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvapiConst.MaxNumberOfFanCoolerInfoEntries)]
        internal readonly FanCoolersInfoEntry[] FanCoolersInfoEntries;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct FanCoolersStatusEntry {
        internal readonly uint CoolerId;
        internal readonly uint CurrentRPM;
        internal readonly uint CurrentMinimumLevel;
        internal readonly uint CurrentMaximumLevel;
        internal readonly uint CurrentLevel;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        internal readonly uint[] Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PrivateFanCoolersStatusV1 {
        internal NvU32 version;
        internal readonly uint FanCoolersStatusCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        internal readonly uint[] Reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvapiConst.MaxNumberOfFanCoolerStatusEntries)]
        internal FanCoolersStatusEntry[] FanCoolersStatusEntries;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct FanCoolersControlEntry {
        internal readonly uint CoolerId;
        internal uint Level;
        internal FanCoolersControlMode ControlMode;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        internal readonly uint[] Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PrivateFanCoolersControlV1 {
        internal NvU32 version;
        internal readonly uint UnknownUInt;
        internal readonly uint FanCoolersControlCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        internal readonly uint[] Reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvapiConst.MaxNumberOfFanCoolerControlEntries)]
        internal readonly FanCoolersControlEntry[] FanCoolersControlEntries;
    }
    #endregion
}

using System;
using System.Runtime.InteropServices;

namespace NTMiner.Gpus.Nvapi {
    #region Enumms
    public enum NvStatus {
        OK = 0,
        ERROR = -1,
        LIBRARY_NOT_FOUND = -2,
        NO_IMPLEMENTATION = -3,
        API_NOT_INTIALIZED = -4,
        INVALID_ARGUMENT = -5,
        NVIDIA_DEVICE_NOT_FOUND = -6,
        END_ENUMERATION = -7,
        INVALID_HANDLE = -8,
        INCOMPATIBLE_STRUCT_VERSION = -9,
        HANDLE_INVALIDATED = -10,
        OPENGL_CONTEXT_NOT_CURRENT = -11,
        NO_GL_EXPERT = -12,
        INSTRUMENTATION_DISABLED = -13,
        EXPECTED_LOGICAL_GPU_HANDLE = -100,
        EXPECTED_PHYSICAL_GPU_HANDLE = -101,
        EXPECTED_DISPLAY_HANDLE = -102,
        INVALID_COMBINATION = -103,
        NOT_SUPPORTED = -104,
        PORTID_NOT_FOUND = -105,
        EXPECTED_UNATTACHED_DISPLAY_HANDLE = -106,
        INVALID_PERF_LEVEL = -107,
        DEVICE_BUSY = -108,
        NV_PERSIST_FILE_NOT_FOUND = -109,
        PERSIST_DATA_NOT_FOUND = -110,
        EXPECTED_TV_DISPLAY = -111,
        EXPECTED_TV_DISPLAY_ON_DCONNECTOR = -112,
        NO_ACTIVE_SLI_TOPOLOGY = -113,
        SLI_RENDERING_MODE_NOTALLOWED = -114,
        EXPECTED_DIGITAL_FLAT_PANEL = -115,
        ARGUMENT_EXCEED_MAX_SIZE = -116,
        DEVICE_SWITCHING_NOT_ALLOWED = -117,
        TESTING_CLOCKS_NOT_SUPPORTED = -118,
        UNKNOWN_UNDERSCAN_CONFIG = -119,
        TIMEOUT_RECONFIGURING_GPU_TOPO = -120,
        DATA_NOT_FOUND = -121,
        EXPECTED_ANALOG_DISPLAY = -122,
        NO_VIDLINK = -123,
        REQUIRES_REBOOT = -124,
        INVALID_HYBRID_MODE = -125,
        MIXED_TARGET_TYPES = -126,
        SYSWOW64_NOT_SUPPORTED = -127,
        IMPLICIT_SET_GPU_TOPOLOGY_CHANGE_NOT_ALLOWED = -128,
        REQUEST_USER_TO_CLOSE_NON_MIGRATABLE_APPS = -129,
        OUT_OF_MEMORY = -130,
        WAS_STILL_DRAWING = -131,
        FILE_NOT_FOUND = -132,
        TOO_MANY_UNIQUE_STATE_OBJECTS = -133,
        INVALID_CALL = -134,
        D3D10_1_LIBRARY_NOT_FOUND = -135,
        FUNCTION_NOT_FOUND = -136
    }
    public enum NvThermalController {
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
    public enum NvThermalTarget {
        NONE = 0,
        GPU = 1,
        MEMORY = 2,
        POWER_SUPPLY = 4,
        BOARD = 8,
        ALL = 15,
        UNKNOWN = -1
    };

    #endregion

    #region Structs
    [StructLayout(LayoutKind.Sequential)]
    public struct NvPhysicalGpuHandle {
        private readonly IntPtr ptr;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NvPState {
        public bool Present;
        public int Percentage;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NvPStates {
        public uint Version;
        public uint Flags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvapiNativeMethods.MAX_PSTATES_PER_GPU)]
        public NvPState[] PStates;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NvLevel {
        public int Level;
        public int Policy;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NvSensor {
        public NvThermalController Controller;
        public uint DefaultMinTemp;
        public uint DefaultMaxTemp;
        public uint CurrentTemp;
        public NvThermalTarget Target;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NvGPUThermalSettings {
        public uint Version;
        public uint Count;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvapiNativeMethods.MAX_THERMAL_SENSORS_PER_GPU)]
        public NvSensor[] Sensor;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NvGPUPowerInfo {
        public uint Version;
        public uint Flags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvapiNativeMethods.MAX_POWER_ENTRIES_PER_GPU)]
        public NvGPUPowerInfoEntry[] Entries;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NvGPUPowerInfoEntry {
        public uint PState;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] Unknown1;
        public uint MinPower;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] Unknown2;
        public uint DefPower;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] Unknown3;
        public uint MaxPower;
        public uint Unknown4;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NvGPUPowerStatus {
        public uint Version;
        public uint Flags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvapiNativeMethods.MAX_POWER_ENTRIES_PER_GPU)]
        public NvGPUPowerStatusEntry[] Entries;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NvGPUPowerStatusEntry {
        public uint Unknown1;
        public uint Unknown2;
        /// <summary>
        /// Power percentage * 1000 (e.g. 50% is 50000)
        /// </summary>
        public uint Power;
        public uint Unknown4;
    }

    #endregion
}

using System.Runtime.InteropServices;

namespace NTMiner.Core.Gpus.Adl {
    internal static class AdlTypes {
        #region Internal Constant
        /// <summary> Define the maximum path</summary>
        internal const int ADL_MAX_PATH = 256;
        /// <summary> Define the maximum adapters</summary>
        internal const int ADL_MAX_ADAPTERS = 250;
        /// <summary> Define the maximum displays</summary>
        internal const int ADL_MAX_DISPLAYS = 40 /* 150 */;
        /// <summary> Define the maximum device name length</summary>
        internal const int ADL_MAX_DEVICENAME = 32;
        /// <summary> Define the successful</summary>
        internal const int ADL_SUCCESS = 0;
        /// <summary> Define the failure</summary>
        internal const int ADL_FAIL = -1;
        internal const int ADL_NOT_SUPPORTED = -8;
        /// <summary> Define the driver ok</summary>
        internal const int ADL_DRIVER_OK = 0;
        /// <summary> Maximum number of GL-Sync ports on the GL-Sync module </summary>
        internal const int ADL_MAX_GLSYNC_PORTS = 8;
        /// <summary> Maximum number of GL-Sync ports on the GL-Sync module </summary>
        internal const int ADL_MAX_GLSYNC_PORT_LEDS = 8;
        /// <summary> Maximum number of ADLMOdes for the adapter </summary>
        internal const int ADL_MAX_NUM_DISPLAYMODES = 1024;

        internal const int ADL_DL_FANCTRL_SPEED_TYPE_PERCENT = 1;
        internal const int ADL_DL_FANCTRL_SPEED_TYPE_RPM = 2;
        #endregion Internal Constant
    }

    #region ADLAdapterInfo
    /// <summary> ADLAdapterInfo Structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLAdapterInfo {
        /// <summary>The size of the structure</summary>
        int Size;
        /// <summary> Adapter Index</summary>
        internal int AdapterIndex;
        /// <summary> Adapter UDID</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)AdlTypes.ADL_MAX_PATH)]
        internal string UDID;
        /// <summary> Adapter Bus Number</summary>
        internal int BusNumber;
        /// <summary> Adapter Driver Number</summary>
        internal int DriverNumber;
        /// <summary> Adapter Function Number</summary>
        internal int FunctionNumber;
        /// <summary> Adapter Vendor ID</summary>
        internal int VendorID;
        /// <summary> Adapter Adapter name</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)AdlTypes.ADL_MAX_PATH)]
        internal string AdapterName;
        /// <summary> Adapter Display name</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)AdlTypes.ADL_MAX_PATH)]
        internal string DisplayName;
        /// <summary> Adapter Present status</summary>
        internal int Present;
        /// <summary> Adapter Exist status</summary>
        internal int Exist;
        /// <summary> Adapter Driver Path</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)AdlTypes.ADL_MAX_PATH)]
        internal string DriverPath;
        /// <summary> Adapter Driver Ext Path</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)AdlTypes.ADL_MAX_PATH)]
        internal string DriverPathExt;
        /// <summary> Adapter PNP String</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)AdlTypes.ADL_MAX_PATH)]
        internal string PNPString;
        /// <summary> OS Display Index</summary>
        internal int OSDisplayIndex;
    }

    #endregion ADLAdapterInfo

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLTemperature {
        public int Size;
        /// <summary>
        /// Temperature in millidegrees Celsius
        /// </summary>
        public int Temperature;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ADLFanSpeedValue {
        public int Size;
        public int SpeedType;
        public int FanSpeed;
        public int Flags;
    }
}

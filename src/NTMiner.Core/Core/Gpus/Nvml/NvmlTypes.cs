using System;
using System.Runtime.InteropServices;

namespace NTMiner.Core.Gpus.Nvml {
    #region enums
    /// <summary>
    /// enum to represent type of bridge chip
    /// </summary>
    public enum nvmlBridgeChipType {
        PLX = 0,
        BRO4 = 1
    }
    
    /// <summary>
    /// enum to represent the NvLink utilization counter packet units
    /// </summary>
    public enum nvmlNvLinkUtilizationCountUnits {
        /// <summary>
        /// count by cycles
        /// </summary>
        Cycles = 0,
        /// <summary>
        /// count by packets
        /// </summary>
        Packets = 1,
        /// <summary>
        /// count by bytes
        /// </summary>
        Bytes = 2,
    }

    /// <summary>
    /// enum to represent the NvLink utilization counter packet types to count<para/>
    /// ** this is ONLY applicable with the units as packets or bytes<para/>
    /// ** as specified in \a nvmlNvLinkUtilizationCountUnits<para/>
    /// ** all packet filter descriptions are target GPU centric<para/>
    /// ** these can be "OR'd" together 
    /// </summary>
    public enum nvmlNvLinkUtilizationCountPktTypes {
        /// <summary>
        /// no operation packets
        /// </summary>
        PktFilterNop = 0x1,
        /// <summary>
        /// read packets
        /// </summary>
        PktFilterRead = 0x2,
        /// <summary>
        /// write packets
        /// </summary>
        PktFilterWrite = 0x4,
        /// <summary>
        /// reduction atomic requests
        /// </summary>
        PktFilterRAtom = 0x8,
        /// <summary>
        /// non-reduction atomic requests
        /// </summary>
        PktFilterNRAtom = 0x10,
        /// <summary>
        /// flush requests
        /// </summary>
        PktFilterFlush = 0x20,
        /// <summary>
        /// responses with data
        /// </summary>
        PktFilterRespData = 0x40,
        /// <summary>
        /// responses without data
        /// </summary>
        PktFilterRespNoData = 0x80,
        /// <summary>
        /// all packets
        /// </summary>
        PktFilterAll = 0xFF
    }

    /// <summary>
    /// enum to represent NvLink queryable capabilities
    /// </summary>
    public enum nvmlNvLinkCapability {
        /// <summary>
        /// P2P over NVLink is supported
        /// </summary>
        P2PSupported = 0,
        /// <summary>
        /// Access to system memory is supported
        /// </summary>
        SysMemAccess = 1,
        /// <summary>
        /// P2P atomics are supported
        /// </summary>
        P2PAtomics = 2,
        /// <summary>
        /// System memory atomics are supported
        /// </summary>
        SysMemAtomics = 3,
        /// <summary>
        /// SLI is supported over this link
        /// </summary>
        SLIBridge = 4,
        /// <summary>
        /// Link is supported on this device
        /// </summary>
        Valid = 5,

    }

    /// <summary>
    /// enum to represent NvLink queryable error counters
    /// </summary>
    public enum nvmlNvLinkErrorCounter {
        /// <summary>
        /// Data link transmit replay error counter
        /// </summary>
        DLReplay = 0,
        /// <summary>
        /// Data link transmit recovery error counter
        /// </summary>
        DLRecovery = 1,
        /// <summary>
        /// Data link receive flow control digit CRC error counter
        /// </summary>
        DL_CRC_FLIT = 2,
        /// <summary>
        /// Data link receive data CRC error counter
        /// </summary>
        DL_CRC_Data = 3,
    }

    /// <summary>
    /// Represents level relationships within a system between two GPUs
    /// The enums are spaced to allow for future relationships
    /// </summary>
    public enum nvmlGpuTopologyLevel {
        /// <summary>
        /// e.g. Tesla K80
        /// </summary>
        Internal = 0,
        /// <summary>
        /// all devices that only need traverse a single PCIe switch
        /// </summary>
        Single = 10,
        /// <summary>
        /// all devices that need not traverse a host bridge
        /// </summary>
        Multiple = 20,
        /// <summary>
        /// all devices that are connected to the same host bridge
        /// </summary>
        Hostbridge = 30,
        /// <summary>
        /// all devices that are connected to the same CPU but possibly multiple host bridges
        /// </summary>
        CPU = 40,
        /// <summary>
        /// all devices in the system
        /// </summary>
        System = 50,
    }

    /// <summary>
    /// Represents Type of Sampling Event
    /// </summary>
    public enum nvmlSamplingType {
        /// <summary>
        /// To represent total power drawn by GPU
        /// </summary>
        NVMLOTAL_POWER_SAMPLES = 0,
        /// <summary>
        /// To represent percent of time during which one or more kernels was executing on the GPU
        /// </summary>
        NVML_GPU_UTILIZATION_SAMPLES = 1,
        /// <summary>
        /// To represent percent of time during which global (device) memory was being read or written
        /// </summary>
        NVML_MEMORY_UTILIZATION_SAMPLES = 2,
        /// <summary>
        /// To represent percent of time during which NVENC remains busy
        /// </summary>
        NVML_ENC_UTILIZATION_SAMPLES = 3,
        /// <summary>
        /// To represent percent of time during which NVDEC remains busy     
        /// </summary>
        NVML_DEC_UTILIZATION_SAMPLES = 4,
        /// <summary>
        /// To represent processor clock samples
        /// </summary>       
        NVML_PROCESSOR_CLK_SAMPLES = 5,
        /// <summary>
        /// To represent memory clock samples
        /// </summary>
        NVML_MEMORY_CLK_SAMPLES = 6
    }

    /// <summary>
    /// Represents the queryable PCIe utilization counters
    /// </summary>
    public enum nvmlPcieUtilCounter {
        /// <summary>
        /// 1KB granularity
        /// </summary>
        TXBytes = 0,
        /// <summary>
        /// 1KB granularity
        /// </summary>
        RXBytes = 1
    }

    /// <summary>
    /// Represents the type for sample value returned
    /// </summary>
    public enum nvmlValueType {
        Double = 0,
        UInt = 1,
        ULong = 2,
        ULongLong = 3
    }

    /// <summary>
    /// Represents type of perf policy for which violation times can be queried 
    /// </summary>
    public enum nvmlPerfPolicyType {
        Power = 0,
        Thermal = 1,
        SyncBoost = 2
    }

    /// <summary>
    /// Generic enable/disable enum.
    /// </summary>
    public enum nvmlEnableState {
        /// <summary>
        /// Feature disabled 
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// Feature enabled
        /// </summary>
        Enabled = 1
    }

    /// <summary>
    /// The Brand of the GPU
    /// </summary>
    public enum nvmlBrandType {
        Unknown = 0,
        Quadro = 1,
        Tesla = 2,
        Nvs = 3,
        Grid = 4,
        GeForce = 5
    }

    /// <summary>
    /// Temperature thresholds.
    /// </summary>
    public enum nvmlTemperatureThresholds {
        /// <summary>
        /// Temperature at which the GPU will shut down for HW protection
        /// </summary>
        Shutdown = 0,
        /// <summary>
        /// Temperature at which the GPU will begin slowdown
        /// </summary>
        Slowdown = 1
    }

    /// <summary>
    /// Temperature sensors.
    /// </summary>
    public enum nvmlTemperatureSensors {
        /// <summary>
        /// Temperature sensor for the GPU die
        /// </summary>
        Gpu = 0
    }

    /// <summary>
    /// Compute mode. <para/>
    /// NVML_COMPUTEMODE_EXCLUSIVE_PROCESS was added in CUDA 4.0.<para/>
    /// Earlier CUDA versions supported a single exclusive mode, 
    /// which is equivalent to NVML_COMPUTEMODE_EXCLUSIVE_THREAD in CUDA 4.0 and beyond.
    /// </summary>
    public enum nvmlComputeMode {
        /// <summary>
        /// Default compute mode -- multiple contexts per device
        /// </summary>
        Default = 0,
        /// <summary>
        /// Support Removed
        /// </summary>
        ExclusiveThread = 1,
        /// <summary>
        /// Compute-prohibited mode -- no contexts per device
        /// </summary>
        Prohibited = 2,
        /// <summary>
        /// Compute-exclusive-process mode -- only one context per device, usable from multiple threads at a time
        /// </summary>
        ExclusiveProcess = 3,
    }

    /// <summary>
    /// Memory error types
    /// </summary>
    public enum nvmlMemoryErrorType {
        /// <summary>
        /// A memory error that was corrected<para/>
        /// For ECC errors, these are single bit errors<para/>
        /// For Texture memory, these are errors fixed by resend
        /// </summary>
        Corrected = 0,
        /// <summary>
        /// A memory error that was not corrected<para/>
        /// For ECC errors, these are double bit errors<para/>
        /// For Texture memory, these are errors where the resend fails
        /// </summary>
        Uncorrected = 1

    }

    /// <summary>
    /// ECC counter types. 
    /// </summary>
    public enum nvmlEccCounterType {
        /// <summary>
        /// Volatile counts are reset each time the driver loads.
        /// </summary>
        VolatileECC = 0,
        /// <summary>
        /// Aggregate counts persist across reboots (i.e. for the lifetime of the device)
        /// </summary>
        AggregateECC = 1,
    }

    /// <summary>
    /// Clock types. All speeds are in Mhz.
    /// </summary>
    public enum nvmlClockType {
        /// <summary>
        /// Graphics clock domain 
        /// </summary>
        Graphics = 0,
        /// <summary>
        /// SM clock domain
        /// </summary>
        SM = 1,
        /// <summary>
        /// Memory clock domain
        /// </summary>
        Mem = 2,
        /// <summary>
        /// Video encoder/decoder clock domain
        /// </summary>
        Video = 3
    }

    /// <summary>
    /// Clock Ids.  These are used in combination with nvmlClockType to specify a single clock value.
    /// </summary>
    public enum nvmlClockId {
        /// <summary>
        /// Current actual clock value
        /// </summary>
        Current = 0,
        /// <summary>
        /// Target application clock
        /// </summary>
        AppClockTarget = 1,
        /// <summary>
        /// Default application clock target
        /// </summary>
        ClockDefault = 2,
        /// <summary>
        /// OEM-defined maximum clock rate
        /// </summary>
        CustomerBoostMax = 3,
    }

    /// <summary>
    /// Driver models. Windows only.
    /// </summary>
    public enum nvmlDriverModel {
        /// <summary>
        /// WDDM driver model -- GPU treated as a display device
        /// </summary>
        WDDM = 0,
        /// <summary>
        /// WDM (TCC) model (recommended) -- GPU treated as a generic device
        /// </summary>
        WDM = 1
    }

    /// <summary>
    /// Allowed PStates.
    /// </summary>
    public enum nvmlPstates {
        /// <summary>
        /// Performance state 0 -- Maximum Performance
        /// </summary>
        PState_0 = 0,
        /// <summary>
        /// Performance state 1 
        /// </summary>
        PState_1 = 1,
        /// <summary>
        /// Performance state 2
        /// </summary>
        PState_2 = 2,
        /// <summary>
        /// Performance state 3
        /// </summary>
        PState_3 = 3,
        /// <summary>
        /// Performance state 4
        /// </summary>
        PState_4 = 4,
        /// <summary>
        /// Performance state 5
        /// </summary>
        PState_5 = 5,
        /// <summary>
        /// Performance state 6
        /// </summary>
        PState_6 = 6,
        /// <summary>
        /// Performance state 7
        /// </summary>
        PState_7 = 7,
        /// <summary>
        /// Performance state 8
        /// </summary>
        PState_8 = 8,
        /// <summary>
        /// Performance state 9
        /// </summary>
        PState_9 = 9,
        /// <summary>
        /// Performance state 10
        /// </summary>
        PState_10 = 10,
        /// <summary>
        /// Performance state 11
        /// </summary>
        PState_11 = 11,
        /// <summary>
        /// Performance state 12
        /// </summary>
        PState_12 = 12,
        /// <summary>
        /// Performance state 13
        /// </summary>
        PState_13 = 13,
        /// <summary>
        /// Performance state 14
        /// </summary>
        PState_14 = 14,
        /// <summary>
        /// Performance state 15 -- Minimum Performance 
        /// </summary>
        PState_15 = 15,
        /// <summary>
        /// Unknown performance state
        /// </summary>
        PState_UNKNOWN = 32
    }

    /// <summary>
    /// GPU Operation Mode<para/>
    /// GOM allows to reduce power usage and optimize GPU throughput by disabling GPU features.<para/>
    /// Each GOM is designed to meet specific user needs.
    /// </summary>
    public enum nvmlGpuOperationMode {
        /// <summary>
        /// Everything is enabled and running at full speed
        /// </summary>
        AllOn = 0,
        /// <summary>
        /// Designed for running only compute tasks. Graphics operations are not allowed
        /// </summary>
        Compute = 1,
        /// <summary>
        /// Designed for running graphics applications that don't require high bandwidth double precision
        /// </summary>
        LowDP = 2
    }

    /// <summary>
    /// Available infoROM objects.
    /// </summary>
    public enum nvmlInforomObject {
        /// <summary>
        /// An object defined by OEM
        /// </summary>
        OEM = 0,
        /// <summary>
        /// The ECC object determining the level of ECC support
        /// </summary>
        ECC = 1,
        /// <summary>
        /// The power management object
        /// </summary>
        Power = 2
    }

    /// <summary>
    /// Return values for NVML API calls. 
    /// </summary>
    public enum nvmlReturn {
        /// <summary>
        /// The operation was successful
        /// </summary>
        Success = 0,
        /// <summary>
        /// NVML was not first initialized with nvmlInit()
        /// </summary>
        Uninitialized = 1,
        /// <summary>
        /// A supplied argument is invalid
        /// </summary>
        InvalidArgument = 2,
        /// <summary>
        /// The requested operation is not available on target device
        /// </summary>
        NotSupported = 3,
        /// <summary>
        /// The current user does not have permission for operation
        /// </summary>
        NoPermission = 4,
        /// <summary>
        /// Deprecated: Multiple initializations are now allowed through ref counting
        /// </summary>
        [Obsolete("Deprecated: Multiple initializations are now allowed through ref counting")]
        AlreadyInitialized = 5,
        /// <summary>
        /// A query to find an object was unsuccessful
        /// </summary>
        NotFound = 6,
        /// <summary>
        /// An input argument is not large enough
        /// </summary>
        InsufficientSize = 7,
        /// <summary>
        /// A device's external power cables are not properly attached
        /// </summary>
        InsufficientPower = 8,
        /// <summary>
        /// NVIDIA driver is not loaded
        /// </summary>
        DriverNotLoaded = 9,
        /// <summary>
        /// User provided timeout passed
        /// </summary>
        TimeOut = 10,
        /// <summary>
        /// NVIDIA Kernel detected an interrupt issue with a GPU
        /// </summary>
        IRQIssue = 11,
        /// <summary>
        /// NVML Shared Library couldn't be found or loaded
        /// </summary>
        LibraryNotFound = 12,
        /// <summary>
        /// Local version of NVML doesn't implement this function
        /// </summary>
        FunctionNotFound = 13,
        /// <summary>
        /// infoROM is corrupted
        /// </summary>
        CorruptedInfoROM = 14,
        /// <summary>
        /// The GPU has fallen off the bus or has otherwise become inaccessible
        /// </summary>
        GPUIsLost = 15,
        /// <summary>
        /// The GPU requires a reset before it can be used again
        /// </summary>
        ResetRequired = 16,
        /// <summary>
        /// The GPU control device has been blocked by the operating system/cgroups
        /// </summary>
        OperatingSystem = 17,
        /// <summary>
        /// RM detects a driver/library version mismatch
        /// </summary>
        LibRMVersionMismatch = 18,
        /// <summary>
        /// An operation cannot be performed because the GPU is currently in use
        /// </summary>
        InUse = 19,
        /// <summary>
        /// An internal driver error occurred
        /// </summary>
        Unknown = 999
    }

    /// <summary>
    /// Memory locations
    /// </summary>
    public enum nvmlMemoryLocation {
        /// <summary>
        /// GPU L1 Cache
        /// </summary>
        L1Cache = 0,
        /// <summary>
        /// GPU L2 Cache
        /// </summary>
        L2Cache = 1,
        /// <summary>
        /// GPU Device Memory
        /// </summary>
        DeviceMemory = 2,
        /// <summary>
        /// GPU Register File
        /// </summary>
        RegisterFile = 3,
        /// <summary>
        /// GPU Texture Memory
        /// </summary>
        TextureMemory = 4,
        /// <summary>
        /// Shared memory
        /// </summary>
        TextureSHM = 5
    }

    /// <summary>
    /// Causes for page retirement
    /// </summary>
    public enum nvmlPageRetirementCause {
        /// <summary>
        /// Page was retired due to multiple single bit ECC error
        /// </summary>
        MultipleSingleBitECCErrors = 0,
        /// <summary>
        /// Page was retired due to double bit ECC error
        /// </summary>
        DoubleBitECCError = 1
    }

    /// <summary>
    /// API types that allow changes to default permission restrictions
    /// </summary>
    public enum nvmlRestrictedAPI {
        /// <summary>
        /// APIs that change application clocks, see nvmlDeviceSetApplicationsClocks and see nvmlDeviceResetApplicationsClocks
        /// </summary>
        ApplicationClocks = 0,
        /// <summary>
        /// APIs that enable/disable auto boosted clocks see nvmlDeviceSetAutoBoostedClocksEnabled
        /// </summary>
        AutoBoostedClocks = 1
    }

    /// <summary>
    /// Fan state enum. 
    /// </summary>
    public enum nvmlFanState {
        /// <summary>
        /// Fan is working properly
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Fan has failed 
        /// </summary>
        Failed = 1
    }

    /// <summary>
    /// Led color enum. 
    /// </summary>
    public enum nvmlLedColor {
        /// <summary>
        /// GREEN, indicates good health
        /// </summary>
        Green = 0,
        /// <summary>
        /// AMBER, indicates problem
        /// </summary>
        Amber = 1
    }

    #endregion

    #region enum (Flags)

    /// <summary>
    /// Event Types which user can be notified about. Types can be combined with bitwise or operator '|' when passed to \ref nvmlDeviceRegisterEvents
    /// </summary>
    [Flags]
    public enum nvmlEventType : long {
        /// <summary>
        /// Event about single bit ECC errors
        /// </summary>
        SingleBitEccError = 0x0000000000000001L,
        /// <summary>
        /// Event about double bit ECC errors
        /// </summary>
        DoubleBitEccError = 0x0000000000000002L,
        /// <summary>
        /// Event about PState changes
        /// </summary>
        PState = 0x0000000000000004L,
        /// <summary>
        /// Event that Xid critical error occurred
        /// </summary>
        XidCriticalError = 0x0000000000000008L,
        /// <summary>
        /// Event about clock changes
        /// </summary>
        Clock = 0x0000000000000010L,
        /// <summary>
        /// Mask with no events
        /// </summary>
        None = 0x0000000000000000L,
        /// <summary>
        /// Mask of all events
        /// </summary>
        All = (None | SingleBitEccError | DoubleBitEccError | PState | Clock | XidCriticalError)
    }

    /// <summary>
    /// nvmlClocksThrottleReasons
    /// </summary>
    [Flags]
    public enum nvmlClocksThrottleReason : ulong {
        /// <summary>
        /// Nothing is running on the GPU and the clocks are dropping to Idle state
        /// </summary>
        GpuIdle = 0x0000000000000001L,
        /// <summary>
        /// GPU clocks are limited by current setting of applications clocks
        /// </summary>
        ApplicationsClocksSetting = 0x0000000000000002L,
        /// <summary>
        /// SW Power Scaling algorithm is reducing the clocks below requested clocks 
        /// </summary>
        SwPowerCap = 0x0000000000000004L,

        /// <summary>
        /// HW Slowdown (reducing the core clocks by a factor of 2 or more) is engaged<para/>
        /// This is an indicator of:<para/>
        ///  - temperature being too high<para/>
        ///  - External Power Brake Assertion is triggered (e.g. by the system power supply)<para/>
        ///  - Power draw is too high and Fast Trigger protection is reducing the clocks<para/>
        ///  - May be also reported during PState or clock change<para/>
        ///  - This behavior may be removed in a later release.<para/>
        /// </summary>
        HwSlowdown = 0x0000000000000008L,
        /// <summary>
        /// Sync Boost. 
        /// This GPU has been added to a Sync boost group with nvidia-smi or DCGM in
        /// order to maximize performance per watt. All GPUs in the sync boost group
        /// will boost to the minimum possible clocks across the entire group. Look at
        /// the throttle reasons for other GPUs in the system to see why those GPUs are
        /// holding this one at lower clocks.
        /// </summary>
        SyncBoost = 0x0000000000000010L,
        /// <summary>
        /// Some other unspecified factor is reducing the clocks
        /// </summary>
        Unknown = 0x8000000000000000L,
        /// <summary>
        /// Bit mask representing no clocks throttling. Clocks are as high as possible.
        /// </summary>
        None = 0x0000000000000000L,
        /// <summary>
        /// Bit mask representing all supported clocks throttling reasons. New reasons might be added to this list in the future
        /// </summary>
        All = (None | GpuIdle | ApplicationsClocksSetting | SwPowerCap | HwSlowdown | SyncBoost | Unknown)
    }
    #endregion

    #region structs
    /// <summary>
    /// PCI information about a GPU device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlPciInfo {
        /// <summary>
        /// The tuple domain:bus:device.function PCI identifier (&amp; NULL terminator)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string busId;
        /// <summary>
        /// The PCI domain on which the device's bus resides, 0 to 0xffff 
        /// </summary>
        public uint domain;
        /// <summary>
        /// The bus on which the device resides, 0 to 0xff
        /// </summary>
        public uint bus;
        /// <summary>
        /// The device's id on the bus, 0 to 31
        /// </summary>
        public uint device;
        /// <summary>
        /// The combined 16-bit device id and 16-bit vendor id
        /// </summary>
        public uint pciDeviceId;

        /// <summary>
        /// The 32-bit Sub System Device ID. Added in NVML 2.285 API
        /// </summary>
        public uint pciSubSystemId;

        /// <summary>
        /// NVIDIA reserved for internal use only
        /// </summary>
        public uint reserved0;
        public uint reserved1;
        public uint reserved2;
        public uint reserved3;
    }

    /// <summary> 
    /// Utilization information for a device.
    /// Each sample period may be between 1 second and 1/6 second, depending on the product being queried.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlUtilization {
        /// <summary>
        /// Percent of time over the past sample period during which one or more kernels was executing on the GPU
        /// </summary>
        public uint gpu;
        /// <summary>
        /// Percent of time over the past sample period during which global (device) memory was being read or written
        /// </summary>
        public uint memory;
    }

    /// <summary> 
    /// Memory allocation information for a device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlMemory {
        /// <summary>
        /// Total installed FB memory (in bytes)
        /// </summary>
        public ulong total;
        /// <summary>
        /// Unallocated FB memory (in bytes)
        /// </summary>
        public ulong free;
        /// <summary>
        /// Allocated FB memory (in bytes). Note that the driver/GPU always sets aside a small amount of memory for bookkeeping
        /// </summary>
        public ulong used;
    }

    /// <summary>
    /// BAR1 Memory allocation Information for a device
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlBAR1Memory {
        /// <summary>
        /// Total BAR1 Memory (in bytes)
        /// </summary>
        public ulong bar1Total;
        /// <summary>
        /// Unallocated BAR1 Memory (in bytes)
        /// </summary>
        public ulong bar1Free;
        /// <summary>
        /// Allocated Used Memory (in bytes)
        /// </summary>
        public ulong bar1Used;
    }

    /// <summary>
    /// Information about running compute processes on the GPU
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlProcessInfo {
        /// <summary>
        /// Process ID
        /// </summary>
        public uint pid;
        /// <summary>
        /// Amount of used GPU memory in bytes.
        /// </summary>
        public ulong usedGpuMemory;
    }

    /// <summary> 
    /// struct to define the NVLINK counter controls
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlNvLinkUtilizationControl {
        public nvmlNvLinkUtilizationCountUnits units;
        public nvmlNvLinkUtilizationCountPktTypes pktfilter;
    }

    /// <summary>
    /// Information about the Bridge Chip Firmware
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlBridgeChipInfo {
        /// <summary>
        /// Type of Bridge Chip 
        /// </summary>
        public nvmlBridgeChipType type;
        /// <summary>
        /// Firmware Version. 0=Version is unavailable
        /// </summary>
        public uint fwVersion;
    }

    /// <summary>
    /// This structure stores the complete Hierarchy of the Bridge Chip within the board. The immediate 
    /// bridge is stored at index 0 of bridgeInfoList, parent to immediate bridge is at index 1 and so forth.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlBridgeChipHierarchy {
        /// <summary>
        /// Number of Bridge Chips on the Board
        /// </summary>
        public byte bridgeCount;
        /// <summary>
        /// Hierarchy of Bridge Chips on the board
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.U8)]
        public nvmlBridgeChipInfo[] bridgeChipInfo;
    }

    /// <summary>
    /// Union to represent different types of Value
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct nvmlValue {
        /// <summary>
        /// If the value is double
        /// </summary>
        [FieldOffset(0)]
        public double dVal;
        /// <summary>
        /// If the value is uint
        /// </summary>
        [FieldOffset(0)]
        public uint uiVal;
        /// <summary>
        /// If the value is unsigned long
        /// </summary>
        [FieldOffset(0)]
        public uint ulVal;
        /// <summary>
        /// If the value is ulong
        /// </summary>
        [FieldOffset(0)]
        public ulong ullVal;
    }

    /// <summary>
    /// Information for Sample
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlSample {
        /// <summary>
        /// CPU Timestamp in microseconds
        /// </summary>
        public ulong timeStamp;
        /// <summary>
        /// Sample Value
        /// </summary>
        public nvmlValue sampleValue;
    }

    /// <summary>
    /// struct to hold perf policy violation status data
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlViolationTime {
        /// <summary>
        /// referenceTime represents CPU timestamp in microseconds
        /// </summary>
        public ulong referenceTime;
        /// <summary>
        /// violationTime in Nanoseconds
        /// </summary>
        public ulong violationTime;
    }

    /// <summary> 
    /// Description of HWBC entry 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlHwbcEntry {
        public uint hwbcId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string firmwareVersion;
    }

    /// <summary> 
    /// LED states for an S-class unit.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlLedState {
        /// <summary>
        /// If amber, a text description of the cause
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string cause;
        /// <summary>
        /// GREEN or AMBER
        /// </summary>
        public nvmlLedColor color;
    }

    /// <summary> 
    /// Static S-class unit info.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlUnitInfo {
        /// <summary>
        /// Product name
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 96)]
        public string name;
        /// <summary>
        /// Product identifier
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 96)]
        public string id;
        /// <summary>
        /// Product serial number
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 96)]
        public string serial;
        /// <summary>
        /// Firmware version
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 96)]
        public string firmwareVersion;
    }

    /// <summary> 
    /// Power usage information for an S-class unit.
    /// The power supply state is a human readable string that equals "Normal" or contains
    /// a combination of "Abnormal" plus one or more of the following:
    ///    
    ///    - High voltage
    ///    - Fan failure
    ///    - Heatsink temperature
    ///    - Current limit
    ///    - Voltage below UV alarm threshold
    ///    - Low-voltage
    ///    - SI2C remote off command
    ///    - MOD_DISABLE input
    ///    - Short pin transition 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlPSUInfo {
        /// <summary>
        /// The power supply state
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string state;
        /// <summary>
        /// PSU current (A)
        /// </summary>
        public uint current;
        /// <summary>
        /// PSU voltage (V)
        /// </summary>
        public uint voltage;
        /// <summary>
        /// PSU power draw (W)
        /// </summary>
        public uint power;
    }

    /// <summary> 
    /// Fan speed reading for a single fan in an S-class unit.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlUnitFanInfo {
        /// <summary>
        /// Fan speed (RPM)
        /// </summary>
        public uint speed;
        /// <summary>
        /// Flag that indicates whether fan is working properly
        /// </summary>
        public nvmlFanState state;
    }

    /// <summary> 
    /// Fan speed readings for an entire S-class unit.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlUnitFanSpeeds {
        /// <summary>
        /// Fan speed data for each fan
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24, ArraySubType = UnmanagedType.U8)]
        public nvmlUnitFanInfo[] fans;
        /// <summary>
        /// Number of fans in unit
        /// </summary>
        public uint count;
    }

    /// <summary> 
    /// Information about occurred event
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlEventData {
        /// <summary>
        /// Specific device where the event occurred
        /// </summary>
        public nvmlDevice device;
        /// <summary>
        /// Information about what specific event occurred
        /// </summary>
        public ulong eventType;
        /// <summary>
        /// Stores last XID error for the device in the event of nvmlEventTypeXidCriticalError, eventData is 0 for any other event. eventData is set as 999 for unknown xid error.
        /// </summary>
        public ulong eventData;
    }

    /// <summary>
    /// Describes accounting statistics of a process.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlAccountingStats {
        /// <summary>
        /// Percent of time over the process's lifetime during which one or more kernels was executing on the GPU. 
        /// Utilization stats just like returned by \ref nvmlDeviceGetUtilizationRates but for the life time of a 
        /// process (not just the last sample period). Set to NVML_VALUE_NOT_AVAILABLE if nvmlDeviceGetUtilizationRates is not supported
        /// </summary>
        public uint gpuUtilization;

        /// <summary>
        /// Percent of time over the process's lifetime during which global (device) memory was being read or written. Set to NVML_VALUE_NOT_AVAILABLE if nvmlDeviceGetUtilizationRates is not supported
        /// </summary>
        public uint memoryUtilization;

        /// <summary>
        /// Maximum total memory in bytes that was ever allocated by the process. Set to NVML_VALUE_NOT_AVAILABLE if nvmlProcessInfo->usedGpuMemory is not supported
        /// </summary>
        public ulong maxMemoryUsage;

        /// <summary>
        /// Amount of time in ms during which the compute context was active. The time is reported as 0 if the process is not terminated
        /// </summary>
        public ulong time;

        /// <summary>
        /// CPU Timestamp in usec representing start time for the process
        /// </summary>
        public ulong startTime;

        /// <summary>
        /// Flag to represent if the process is running (1 for running, 0 for terminated)
        /// </summary>
        public uint isRunning;

        /// <summary>
        /// Reserved for future use
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = UnmanagedType.U4)]
        public uint[] reserved;
    }

    #endregion

    #region structs as types
    /// <summary>
    /// nvmlDevice
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlDevice {
        /// <summary>
        /// 
        /// </summary>
        private IntPtr Pointer;
    }

    /// <summary>
    /// nvmlUnit
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlUnit {
        /// <summary>
        /// 
        /// </summary>
        private IntPtr Pointer;
    }

    /// <summary>
    /// Handle to an event set
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvmlEventSet {
        /// <summary>
        /// 
        /// </summary>
        private IntPtr Pointer;
    }
    #endregion

    /// <summary>
    /// Constants used in NVML API (defines in original header file)
    /// </summary>
    public static class NVMLConstants {
        /// <summary>
        /// Buffer size guaranteed to be large enough for pci bus id
        /// </summary>
        public const uint DevicePCIBusIDBufferSize = 16;
        /// <summary>
        /// Maximum number of NvLink links supported 
        /// </summary>
        public const uint NVLinkMaxLinks = 4;
        /// <summary>
        /// Maximum limit on Physical Bridges per Board
        /// </summary>
        public const uint MaxPhysicalBridge = 128;
        /// <summary>
        /// Buffer size guaranteed to be large enough for \ref nvmlDeviceGetInforomVersion and \ref nvmlDeviceGetInforomImageVersion
        /// </summary>
        public const uint DeviceInformVersionBufferSize = 16;
        /// <summary>
        /// Buffer size guaranteed to be large enough for \ref nvmlDeviceGetUUID
        /// </summary>
        public const uint DeviceUUIDBufferSize = 80;
        /// <summary>
        /// Buffer size guaranteed to be large enough for \ref nvmlDeviceGetBoardPartNumber
        /// </summary>
        public const uint DevicePartNumberBufferSize = 80;
        /// <summary>
        /// Buffer size guaranteed to be large enough for \ref nvmlSystemGetDriverVersion
        /// </summary>
        public const uint SystemDriverVersionBufferSize = 80;
        /// <summary>
        /// Buffer size guaranteed to be large enough for \ref nvmlSystemGetNVMLVersion
        /// </summary>
        public const uint SystemNVMLVersionBufferSize = 80;
        /// <summary>
        /// Buffer size guaranteed to be large enough for \ref nvmlDeviceGetName
        /// </summary>
        public const uint DeviceNameBufferSize = 64;
        /// <summary>
        /// Buffer size guaranteed to be large enough for \ref nvmlDeviceGetSerial
        /// </summary>
        public const uint DeviceSerialBufferSize = 30;
        /// <summary>
        /// Buffer size guaranteed to be large enough for \ref nvmlDeviceGetVbiosVersion
        /// </summary>
        public const uint DeviceVBIOSVersionBufferSize = 32;
    }
    /// <summary>
    /// NVML API versioning support
    /// </summary>
    //#define NVML_API_VERSION            8
    //#define NVML_API_VERSIONR        "8"
    //#define nvmlInit                    nvmlInit_v2
    //#define nvmlDeviceGetPciInfo        nvmlDeviceGetPciInfo_v2
    //#define nvmlDeviceGetCount          nvmlDeviceGetCount_v2
    //#define nvmlDeviceGetHandleByIndex  nvmlDeviceGetHandleByIndex_v2
    //#define nvmlDeviceGetHandleByPciBusId nvmlDeviceGetHandleByPciBusId_v2
}

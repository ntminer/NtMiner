using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NTMiner.Gpus.Nvml {
    internal static class NvmlNativeMethods {
        public static nvmlReturn NvmlInit() {
            try {
                var result = nvmlInit_v2();
                if (result != nvmlReturn.Success) {
                    result = nvmlInit();
                }
                return result;
            }
            catch {
                return nvmlReturn.LibraryNotFound;
            }
        }

        private const string NVML_API_DLL_NAME = "nvml";


        /// <summary>
        /// Initialize NVML, but don't initialize any GPUs yet.
        /// \note In NVML 5.319 new nvmlInit_v2 has replaced nvmlInit"_v1" (default in NVML 4.304 and older) that
        ///       did initialize all GPU devices in the system.
        ///       
        /// This allows NVML to communicate with a GPU
        /// when other GPUs in the system are unstable or in a bad state.  When using this API, GPUs are
        /// discovered and initialized in nvmlDeviceGetHandleBy* functions instead.
        /// 
        /// \note To contrast nvmlInit_v2 with nvmlInit"_v1", NVML 4.304 nvmlInit"_v1" will fail when any detected GPU is in
        ///       a bad or unstable state.
        /// 
        /// For all products.
        /// This method, should be called once before invoking any other methods in the library.
        /// A reference count of the number of initializations is maintained.  Shutdown only occurs
        /// when the reference count reaches zero.
        /// </summary>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                   if NVML has been properly initialized
        ///         - \ref NVML_ERROR_DRIVER_NOT_LOADED   if NVIDIA driver is not running
        ///         - \ref NVML_ERROR_NO_PERMISSION       if NVML does not have permission to talk to the driver
        ///         - \ref NVML_ERROR_UNKNOWN             on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME, EntryPoint = "nvmlInit_v2")]
        private static extern nvmlReturn nvmlInit_v2();

        [DllImport(NVML_API_DLL_NAME, EntryPoint = "nvmlInit")]
        private static extern nvmlReturn nvmlInit();


        /// <summary>
        /// Shut down NVML by releasing all GPU resources previously allocated with \ref nvmlInit().
        /// 
        /// For all products.
        /// This method should be called after NVML work is done, once for each call to \ref nvmlInit()
        /// A reference count of the number of initializations is maintained.  Shutdown only occurs
        /// when the reference count reaches zero.  For backwards compatibility, no error is reported if
        /// nvmlShutdown() is called more times than nvmlInit().
        /// </summary>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if NVML has been properly shut down
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlShutdown();


        [DllImport(NVML_API_DLL_NAME, EntryPoint = "nvmlErrorString")]
        private static extern IntPtr nvmlErrorStringInternal(nvmlReturn result);

        /// <summary>
        /// Helper method for converting NVML error codes into readable strings.
        /// For all products.
        /// </summary>
        /// <param name="result">NVML error code to convert</param>
        /// <returns>
        /// String representation of the error.
        /// </returns>
        internal static string nvmlErrorString(nvmlReturn result) {
            IntPtr ptr = nvmlErrorStringInternal(result);
            string error;
            error = Marshal.PtrToStringAnsi(ptr);
            return error.Replace("\0", "");
        }


        [DllImport(NVML_API_DLL_NAME, EntryPoint = "nvmlSystemGetDriverVersion")]
        private static extern nvmlReturn nvmlSystemGetDriverVersionInternal(byte[] version, uint length);

        /// <summary>
        /// Retrieves the version of the system's graphics driver.
        /// 
        /// For all products.
        /// The version identifier is an alphanumeric string.  It will not exceed 80 characters in length
        /// (including the NULL terminator).  See \ref nvmlConstants::NVML_SYSTEM_DRIVER_VERSION_BUFFER_SIZE.
        /// </summary>
        /// <param name="version">Reference in which to return the version identifier</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a version has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a version is NULL
        ///         - \ref NVML_ERROR_INSUFFICIENT_SIZE if \a length is too small 
        /// </returns>
        internal static nvmlReturn nvmlSystemGetDriverVersion(out string name) {
            byte[] temp = new byte[NvmlConstant.SystemDriverVersionBufferSize];
            nvmlReturn ret = nvmlSystemGetDriverVersionInternal(temp, NvmlConstant.SystemDriverVersionBufferSize);
            name = string.Empty;
            if (ret == nvmlReturn.Success) {
                name = ASCIIEncoding.ASCII.GetString(temp).Replace("\0", "");
            }
            return ret;
        }


        [DllImport(NVML_API_DLL_NAME, EntryPoint = "nvmlSystemGetNVMLVersion")]
        private static extern nvmlReturn nvmlSystemGetNVMLVersionInternal(byte[] version, uint length);
        /// <summary>
        /// Retrieves the version of the NVML library.
        /// 
        /// For all products.
        /// The version identifier is an alphanumeric string.  It will not exceed 80 characters in length
        /// (including the NULL terminator).  See \ref nvmlConstants::NVML_SYSTEM_NVML_VERSION_BUFFER_SIZE.
        /// </summary>
        /// <param name="version">Reference in which to return the version identifier</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a version has been set
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a version is NULL
        ///         - \ref NVML_ERROR_INSUFFICIENT_SIZE if \a length is too small 
        /// </returns>
        internal static nvmlReturn nvmlSystemGetNVMLVersion(out string name) {
            byte[] temp = new byte[NvmlConstant.SystemNVMLVersionBufferSize];
            nvmlReturn ret = nvmlSystemGetNVMLVersionInternal(temp, NvmlConstant.SystemNVMLVersionBufferSize);
            name = string.Empty;
            if (ret == nvmlReturn.Success) {
                name = ASCIIEncoding.ASCII.GetString(temp).Replace("\0", "");
            }
            return ret;
        }


        [DllImport(NVML_API_DLL_NAME, EntryPoint = "nvmlSystemGetProcessName")]
        private static extern nvmlReturn nvmlSystemGetProcessNameInternal(uint pid, byte[] name, uint length);

        /// <summary>
        /// Gets name of the process with provided process id
        /// For all products.
        /// Returned process name is cropped to provided length.
        /// name string is encoded in ANSI.
        /// </summary>
        /// <param name="pid">The identifier of the process</param>
        /// <param name="name">Reference in which to return the process name</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a name has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a name is NULL or \a length is 0.
        ///         - \ref NVML_ERROR_NOT_FOUND         if process doesn't exists
        ///         - \ref NVML_ERROR_NO_PERMISSION     if the user doesn't have permission to perform this operation
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        internal static nvmlReturn nvmlSystemGetProcessName(uint pid, out string name) {
            byte[] temp = new byte[2048];
            nvmlReturn ret = nvmlSystemGetProcessNameInternal(pid, temp, 2048);
            name = string.Empty;
            if (ret == nvmlReturn.Success) {
                name = ASCIIEncoding.ASCII.GetString(temp).Replace("\0", "");
            }
            return ret;
        }


        /// <summary>
        /// Retrieves the number of units in the system.
        /// For S-class products.
        /// </summary>
        /// <param name="unitCount">Reference in which to return the number of units</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a unitCount has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a unitCount is NULL
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlUnitGetCount(ref uint unitCount);


        /// <summary>
        /// Acquire the handle for a particular unit, based on its index.
        /// For S-class products.
        /// Valid indices are derived from the \a unitCount returned by \ref nvmlUnitGetCount(). 
        ///   For example, if \a unitCount is 2 the valid indices are 0 and 1, corresponding to UNIT 0 and UNIT 1.
        /// The order in which NVML enumerates units has no guarantees of consistency between reboots.
        /// </summary>
        /// <param name="index">The index of the target unit, >= 0 and < \a unitCount</param>
        /// <param name="unit">Reference in which to return the unit handle</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a unit has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a index is invalid or \a unit is NULL
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlUnitGetHandleByIndex(uint index, ref nvmlUnit unit);


        /// <summary>
        /// Retrieves the static information associated with a unit.
        /// For S-class products.
        /// See \ref nvmlUnitInfo for details on available unit info.
        /// </summary>
        /// <param name="unit">The identifier of the target unit</param>
        /// <param name="info">Reference in which to return the unit information</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a info has been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a unit is invalid or \a info is NULL
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlUnitGetUnitInfo(nvmlUnit unit, ref nvmlUnitInfo info);


        /// <summary>
        /// Retrieves the LED state associated with this unit.
        /// For S-class products.
        /// See \ref nvmlLedState for details on allowed states.
        /// </summary>
        /// <param name="unit">The identifier of the target unit</param>
        /// <param name="state">Reference in which to return the current LED state</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a state has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a unit is invalid or \a state is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if this is not an S-class product
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// 
        /// @see nvmlUnitSetLedState()
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlUnitGetLedState(nvmlUnit unit, ref nvmlLedState state);


        /// <summary>
        /// Retrieves the PSU stats for the unit.
        /// For S-class products.
        /// See \ref nvmlPSUInfo for details on available PSU info.
        /// </summary>
        /// <param name="unit">The identifier of the target unit</param>
        /// <param name="psu">Reference in which to return the PSU information</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a psu has been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a unit is invalid or \a psu is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if this is not an S-class product
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlUnitGetPsuInfo(nvmlUnit unit, ref nvmlPSUInfo psu);


        /// <summary>
        /// Retrieves the temperature readings for the unit, in degrees C.
        /// For S-class products.
        /// Depending on the product, readings may be available for intake (type=0), 
        /// exhaust (type=1) and board (type=2).
        /// </summary>
        /// <param name="unit">The identifier of the target unit</param>
        /// <param name="type">The type of reading to take</param>
        /// <param name="temp">Reference in which to return the intake temperature</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a temp has been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a unit or \a type is invalid or \a temp is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if this is not an S-class product
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlUnitGetTemperature(nvmlUnit unit, uint type, ref uint temp);


        /// <summary>
        /// Retrieves the fan speed readings for the unit.
        /// For S-class products.
        /// See \ref nvmlUnitFanSpeeds for details on available fan speed info.
        /// </summary>
        /// <param name="unit">The identifier of the target unit</param>
        /// <param name="fanSpeeds">Reference in which to return the fan speed information</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a fanSpeeds has been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a unit is invalid or \a fanSpeeds is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if this is not an S-class product
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlUnitGetFanSpeedInfo(nvmlUnit unit, ref nvmlUnitFanSpeeds fanSpeeds);


        /// <summary>
        /// Retrieves the set of GPU devices that are attached to the specified unit.
        /// For S-class products.
        /// The \a deviceCount argument is expected to be set to the size of the input \a devices array.
        /// </summary>
        /// <param name="unit">The identifier of the target unit</param>
        /// <param name="deviceCount">Reference in which to provide the \a devices array size, and</param>
        /// <param name="devices">Reference in which to return the references to the attached GPU devices</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a deviceCount and \a devices have been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INSUFFICIENT_SIZE if \a deviceCount indicates that the \a devices array is too small
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a unit is invalid, either of \a deviceCount or \a devices is NULL
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlUnitGetDevices(nvmlUnit unit, ref uint deviceCount, nvmlDevice[] devices);


        /// <summary>
        /// Retrieves the IDs and firmware versions for any Host Interface Cards (HICs) in the system.
        /// 
        /// For S-class products.
        /// The \a hwbcCount argument is expected to be set to the size of the input \a hwbcEntries array.
        /// The HIC must be connected to an S-class system for it to be reported by this function.
        /// </summary>
        /// <param name="hwbcCount">Size of hwbcEntries array</param>
        /// <param name="hwbcEntries">Array holding information about hwbc</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a hwbcCount and \a hwbcEntries have been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if either \a hwbcCount or \a hwbcEntries is NULL
        ///         - \ref NVML_ERROR_INSUFFICIENT_SIZE if \a hwbcCount indicates that the \a hwbcEntries array is too small
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlSystemGetHicVersion(ref uint hwbcCount, nvmlHwbcEntry[] hwbcEntries);


        /// <summary>
        /// Retrieves the number of compute devices in the system. A compute device is a single GPU.
        /// 
        /// For all products.
        /// Note: New nvmlDeviceGetCount_v2 (default in NVML 5.319) returns count of all devices in the system
        ///       even if nvmlDeviceGetHandleByIndex_v2 returns NVML_ERROR_NO_PERMISSION for such device.
        ///       Update your code to handle this error, or use NVML 4.304 or older nvml header file.
        ///       For backward binary compatibility reasons _v1 version of the API is still present in the shared
        ///       library.
        ///       Old _v1 version of nvmlDeviceGetCount doesn't count devices that NVML has no permission to talk to.
        /// </summary>
        /// <param name="deviceCount">Reference in which to return the number of accessible devices</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a deviceCount has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a deviceCount is NULL
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME, EntryPoint = "nvmlDeviceGetCount_v2")]
        internal static extern nvmlReturn nvmlDeviceGetCount(ref uint deviceCount);


        /// <summary>
        /// Acquire the handle for a particular device, based on its index.
        /// 
        /// For all products.
        /// Valid indices are derived from the \a accessibleDevices count returned by 
        ///   \ref nvmlDeviceGetCount(). For example, if \a accessibleDevices is 2 the valid indices  
        ///   are 0 and 1, corresponding to GPU 0 and GPU 1.
        /// The order in which NVML enumerates devices has no guarantees of consistency between reboots. For that reason it
        ///   is recommended that devices be looked up by their PCI ids or UUID. See 
        ///   \ref nvmlDeviceGetHandleByUUID() and \ref nvmlDeviceGetHandleByPciBusId().
        /// Note: The NVML index may not correlate with other APIs, such as the CUDA device index.
        /// Starting from NVML 5, this API causes NVML to initialize the target GPU
        /// NVML may initialize additional GPUs if:
        ///  - The target GPU is an SLI slave
        /// 
        /// Note: New nvmlDeviceGetCount_v2 (default in NVML 5.319) returns count of all devices in the system
        ///       even if nvmlDeviceGetHandleByIndex_v2 returns NVML_ERROR_NO_PERMISSION for such device.
        ///       Update your code to handle this error, or use NVML 4.304 or older nvml header file.
        ///       For backward binary compatibility reasons _v1 version of the API is still present in the shared
        ///       library.
        ///       Old _v1 version of nvmlDeviceGetCount doesn't count devices that NVML has no permission to talk to.
        ///       This means that nvmlDeviceGetHandleByIndex_v2 and _v1 can return different devices for the same index.
        ///       If you don't touch macros that map old (_v1) versions to _v2 versions at the top of the file you don't
        ///       need to worry about that.
        /// </summary>
        /// <param name="index">The index of the target GPU, >= 0 and < \a accessibleDevices</param>
        /// <param name="device">Reference in which to return the device handle</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                  if \a device has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED      if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT   if \a index is invalid or \a device is NULL
        ///         - \ref NVML_ERROR_INSUFFICIENT_POWER if any attached devices have improperly attached external power cables
        ///         - \ref NVML_ERROR_NO_PERMISSION      if the user doesn't have permission to talk to this device
        ///         - \ref NVML_ERROR_IRQ_ISSUE          if NVIDIA kernel detected an interrupt issue with the attached GPUs
        ///         - \ref NVML_ERROR_GPU_IS_LOST        if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN            on any unexpected error
        /// @see nvmlDeviceGetIndex
        /// @see nvmlDeviceGetCount
        /// </returns>
        [DllImport(NVML_API_DLL_NAME, EntryPoint = "nvmlDeviceGetHandleByIndex_v2")]
        internal static extern nvmlReturn nvmlDeviceGetHandleByIndex(uint index, ref nvmlDevice device);


        /// <summary>
        /// Acquire the handle for a particular device, based on its globally unique immutable UUID associated with each device.
        /// For all products.
        /// </summary>
        /// <param name="uuid">The UUID of the target GPU</param>
        /// <param name="device">Reference in which to return the device handle</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                  if \a device has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED      if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT   if \a uuid is invalid or \a device is null
        ///         - \ref NVML_ERROR_NOT_FOUND          if \a uuid does not match a valid device on the system
        ///         - \ref NVML_ERROR_INSUFFICIENT_POWER if any attached devices have improperly attached external power cables
        ///         - \ref NVML_ERROR_IRQ_ISSUE          if NVIDIA kernel detected an interrupt issue with the attached GPUs
        ///         - \ref NVML_ERROR_GPU_IS_LOST        if any GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN            on any unexpected error
        /// @see nvmlDeviceGetUUID
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetHandleByUUID([MarshalAs(UnmanagedType.LPStr)] string uuid, ref nvmlDevice device);


        /// <summary>
        /// Acquire the handle for a particular device, based on its PCI bus id.
        /// 
        /// For all products.
        /// This value corresponds to the nvmlPciInfo::busId returned by \ref nvmlDeviceGetPciInfo().
        /// Starting from NVML 5, this API causes NVML to initialize the target GPU
        /// NVML may initialize additional GPUs if:
        ///  - The target GPU is an SLI slave
        /// \note NVML 4.304 and older version of nvmlDeviceGetHandleByPciBusId"_v1" returns NVML_ERROR_NOT_FOUND 
        ///       instead of NVML_ERROR_NO_PERMISSION.
        /// </summary>
        /// <param name="pciBusId">The PCI bus id of the target GPU</param>
        /// <param name="device">Reference in which to return the device handle</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                  if \a device has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED      if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT   if \a pciBusId is invalid or \a device is NULL
        ///         - \ref NVML_ERROR_NOT_FOUND          if \a pciBusId does not match a valid device on the system
        ///         - \ref NVML_ERROR_INSUFFICIENT_POWER if the attached device has improperly attached external power cables
        ///         - \ref NVML_ERROR_NO_PERMISSION      if the user doesn't have permission to talk to this device
        ///         - \ref NVML_ERROR_IRQ_ISSUE          if NVIDIA kernel detected an interrupt issue with the attached GPUs
        ///         - \ref NVML_ERROR_GPU_IS_LOST        if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN            on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME, EntryPoint = "nvmlDeviceGetHandleByPciBusId_v2")]
        internal static extern nvmlReturn nvmlDeviceGetHandleByPciBusId([MarshalAs(UnmanagedType.LPStr)] string pciBusId, ref nvmlDevice device);


        [DllImport(NVML_API_DLL_NAME, EntryPoint = "nvmlDeviceGetName")]
        private static extern nvmlReturn nvmlDeviceGetNameInternal(nvmlDevice device, byte[] name, uint length);

        /// <summary>
        /// Retrieves the name of this device. 
        /// 
        /// For all products.
        /// The name is an alphanumeric string that denotes a particular product, e.g. Tesla C2070. It will not
        /// exceed 64 characters in length (including the NULL terminator).  See \ref
        /// nvmlConstants::NVML_DEVICE_NAME_BUFFER_SIZE.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="name">Reference in which to return the product name</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a name has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, or \a name is NULL
        ///         - \ref NVML_ERROR_INSUFFICIENT_SIZE if \a length is too small
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        internal static nvmlReturn nvmlDeviceGetName(nvmlDevice device, out string name) {
            byte[] temp = new byte[NvmlConstant.DeviceNameBufferSize];
            nvmlReturn ret = nvmlDeviceGetNameInternal(device, temp, NvmlConstant.DeviceNameBufferSize);
            name = string.Empty;
            if (ret == nvmlReturn.Success) {
                name = ASCIIEncoding.ASCII.GetString(temp).Replace("\0", "");
            }
            return ret;
        }


        /// <summary>
        /// Retrieves the brand of this device.
        /// For all products.
        /// The type is a member of \ref nvmlBrandType defined above.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="type">Reference in which to return the product brand type</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a name has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, or \a type is NULL
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetBrand(nvmlDevice device, ref nvmlBrandType type);


        /// <summary>
        /// Retrieves the NVML index of this device.
        /// For all products.
        /// 
        /// Valid indices are derived from the \a accessibleDevices count returned by 
        ///   \ref nvmlDeviceGetCount(). For example, if \a accessibleDevices is 2 the valid indices  
        ///   are 0 and 1, corresponding to GPU 0 and GPU 1.
        /// The order in which NVML enumerates devices has no guarantees of consistency between reboots. For that reason it
        ///   is recommended that devices be looked up by their PCI ids or GPU UUID. See 
        ///   \ref nvmlDeviceGetHandleByPciBusId() and \ref nvmlDeviceGetHandleByUUID().
        /// Note: The NVML index may not correlate with other APIs, such as the CUDA device index.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="index">Reference in which to return the NVML index of the device</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a index has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, or \a index is NULL
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlDeviceGetHandleByIndex()
        /// @see nvmlDeviceGetCount()
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetIndex(nvmlDevice device, ref uint index);


        [DllImport(NVML_API_DLL_NAME, EntryPoint = "nvmlDeviceGetSerial")]
        private static extern nvmlReturn nvmlDeviceGetSerialInternal(nvmlDevice device, byte[] serial, uint length);

        /// <summary>
        /// Retrieves the globally unique board serial number associated with this device's board.
        /// For all products with an inforom.
        /// The serial number is an alphanumeric string that will not exceed 30 characters (including the NULL terminator).
        /// This number matches the serial number tag that is physically attached to the board.  See \ref
        /// nvmlConstants::NVML_DEVICE_SERIAL_BUFFER_SIZE.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="serial">Reference in which to return the board/module serial number</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a serial has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, or \a serial is NULL
        ///         - \ref NVML_ERROR_INSUFFICIENT_SIZE if \a length is too small
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        internal static nvmlReturn nvmlDeviceGetSerial(nvmlDevice device, out string serial) {
            byte[] temp = new byte[NvmlConstant.DeviceSerialBufferSize];
            nvmlReturn ret = nvmlDeviceGetSerialInternal(device, temp, NvmlConstant.DeviceSerialBufferSize);
            serial = string.Empty;
            if (ret == nvmlReturn.Success) {
                serial = ASCIIEncoding.ASCII.GetString(temp).Replace("\0", "");
            }
            return ret;
        }


        /// <summary>
        /// Retrieves an array of uints (sized to cpuSetSize) of bitmasks with the ideal CPU affinity for the device
        /// For example, if processors 0, 1, 32, and 33 are ideal for the device and cpuSetSize == 2,
        ///     result[0] = 0x3, result[1] = 0x3
        /// For Kepler or newer fully supported devices.
        /// Supported on Linux only.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="cpuSetSize">The size of the cpuSet array that is safe to access</param>
        /// <param name="cpuSet">Array reference in which to return a bitmask of CPUs, 64 CPUs per </param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a cpuAffinity has been filled
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, cpuSetSize == 0, or cpuSet is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetCpuAffinity(nvmlDevice device, uint cpuSetSize, ulong[] cpuSet);


        /// <summary>
        /// Sets the ideal affinity for the calling thread and device using the guidelines 
        /// given in nvmlDeviceGetCpuAffinity().  Note, this is a change as of version 8.0.  
        /// Older versions set the affinity for a calling process and all children.
        /// Currently supports up to 64 processors.
        /// For Kepler or newer fully supported devices.
        /// Supported on Linux only.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the calling process has been successfully bound
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceSetCpuAffinity(nvmlDevice device);


        /// <summary>
        /// Clear all affinity bindings for the calling thread.  Note, this is a change as of version
        /// 8.0 as older versions cleared the affinity for a calling process and all children.
        /// For Kepler or newer fully supported devices.
        /// Supported on Linux only.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the calling process has been successfully unbound
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceClearCpuAffinity(nvmlDevice device);


        /// <summary>
        /// Retrieve the common ancestor for two devices
        /// For all products.
        /// Supported on Linux only.
        /// </summary>
        /// <param name="device1">The identifier of the first device</param>
        /// <param name="device2">The identifier of the second device</param>
        /// <param name="pathInfo">A \ref nvmlGpuTopologyLevel that gives the path type</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a pathInfo has been set
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device1, or \a device2 is invalid, or \a pathInfo is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device or OS does not support this feature
        ///         - \ref NVML_ERROR_UNKNOWN           an error has occurred in underlying topology discovery
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetTopologyCommonAncestor(nvmlDevice device1, nvmlDevice device2, ref nvmlGpuTopologyLevel pathInfo);


        /// <summary>
        /// Retrieve the set of GPUs that are nearest to a given device at a specific interconnectivity level
        /// For all products.
        /// Supported on Linux only.
        /// </summary>
        /// <param name="device">The identifier of the first device</param>
        /// <param name="level">The \ref nvmlGpuTopologyLevel level to search for other GPUs</param>
        /// <param name="count">When zero, is set to the number of matching GPUs such that \a deviceArray </param>
        /// <param name="deviceArray">An array of device handles for GPUs found at \a level</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a deviceArray or \a count (if initially zero) has been set
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device, \a level, or \a count is invalid, or \a deviceArray is NULL with a non-zero \a count
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device or OS does not support this feature
        ///         - \ref NVML_ERROR_UNKNOWN           an error has occurred in underlying topology discovery
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetTopologyNearestGpus(nvmlDevice device, nvmlGpuTopologyLevel level, ref uint count, nvmlDevice[] deviceArray);


        /// <summary>
        /// Retrieve the set of GPUs that have a CPU affinity with the given CPU number
        /// For all products.
        /// Supported on Linux only.
        /// </summary>
        /// <param name="cpuNumber">The CPU number</param>
        /// <param name="count">When zero, is set to the number of matching GPUs such that \a deviceArray </param>
        /// <param name="deviceArray">An array of device handles for GPUs found with affinity to \a cpuNumber</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a deviceArray or \a count (if initially zero) has been set
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a cpuNumber, or \a count is invalid, or \a deviceArray is NULL with a non-zero \a count
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device or OS does not support this feature
        ///         - \ref NVML_ERROR_UNKNOWN           an error has occurred in underlying topology discovery
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlSystemGetTopologyGpuSet(uint cpuNumber, ref uint count, nvmlDevice[] deviceArray);


        [DllImport(NVML_API_DLL_NAME, EntryPoint = "nvmlDeviceGetUUID")]
        private static extern nvmlReturn nvmlDeviceGetUUIDInternal(nvmlDevice device, byte[] uuid, uint length);

        /// <summary>
        /// Retrieves the globally unique immutable UUID associated with this device, as a 5 part hexadecimal string,
        /// that augments the immutable, board serial identifier.
        /// For all products.
        /// The UUID is a globally unique identifier. It is the only available identifier for pre-Fermi-architecture products.
        /// It does NOT correspond to any identifier printed on the board.  It will not exceed 80 characters in length
        /// (including the NULL terminator).  See \ref nvmlConstants::NVML_DEVICE_UUID_BUFFER_SIZE.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="uuid">Reference in which to return the GPU UUID</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a uuid has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, or \a uuid is NULL
        ///         - \ref NVML_ERROR_INSUFFICIENT_SIZE if \a length is too small 
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        internal static nvmlReturn nvmlDeviceGetUUID(nvmlDevice device, out string uuid) {
            byte[] temp = new byte[NvmlConstant.DevicePartNumberBufferSize];
            nvmlReturn ret = nvmlDeviceGetUUIDInternal(device, temp, NvmlConstant.DevicePartNumberBufferSize);
            uuid = string.Empty;
            if (ret == nvmlReturn.Success) {
                uuid = ASCIIEncoding.ASCII.GetString(temp).Replace("\0", "");
            }
            return ret;
        }


        /// <summary>
        /// Retrieves minor number for the device. The minor number for the device is such that the Nvidia device node file for 
        /// each GPU will have the form /dev/nvidia[minor number].
        /// For all products.
        /// Supported only for Linux
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="minorNumber">Reference in which to return the minor number for the device</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the minor number is successfully retrieved
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a minorNumber is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if this query is not supported by the device
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetMinorNumber(nvmlDevice device, ref uint minorNumber);


        [DllImport(NVML_API_DLL_NAME, EntryPoint = "nvmlDeviceGetBoardPartNumber")]
        private static extern nvmlReturn nvmlDeviceGetBoardPartNumberInternal(nvmlDevice device, byte[] partNumber, uint length);

        /// <summary>
        /// Retrieves the the device board part number which is programmed into the board's InfoROM
        /// For all products.
        /// </summary>
        /// <param name="device">Identifier of the target device</param>
        /// <param name="partNumber">Reference to the buffer to return</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                  if \a partNumber has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED      if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_NOT_SUPPORTED      if the needed VBIOS fields have not been filled
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT   if \a device is invalid or \a serial is NULL
        ///         - \ref NVML_ERROR_GPU_IS_LOST        if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN            on any unexpected error
        /// </returns>
        internal static nvmlReturn nvmlDeviceGetBoardPartNumber(nvmlDevice device, out string partNumber) {
            byte[] temp = new byte[NvmlConstant.DevicePartNumberBufferSize];
            nvmlReturn ret = nvmlDeviceGetBoardPartNumberInternal(device, temp, NvmlConstant.DevicePartNumberBufferSize);
            partNumber = string.Empty;
            if (ret == nvmlReturn.Success) {
                partNumber = ASCIIEncoding.ASCII.GetString(temp).Replace("\0", "");
            }
            return ret;
        }


        [DllImport(NVML_API_DLL_NAME, EntryPoint = "nvmlDeviceGetInforomVersion")]
        private static extern nvmlReturn nvmlDeviceGetInforomVersionInternal(nvmlDevice device, nvmlInforomObject IRobject, byte[] version, uint length);

        /// <summary>
        /// Retrieves the version information for the device's infoROM object.
        /// For all products with an inforom.
        /// Fermi and higher parts have non-volatile on-board memory for persisting device info, such as aggregate 
        /// ECC counts. The version of the data structures in this memory may change from time to time. It will not
        /// exceed 16 characters in length (including the NULL terminator).
        /// See \ref nvmlConstants::NVML_DEVICE_INFOROM_VERSION_BUFFER_SIZE.
        /// See \ref nvmlInforomObject for details on the available infoROM objects.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="IRobject">The target infoROM object</param>
        /// <param name="version">Reference in which to return the infoROM version</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a version has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a version is NULL
        ///         - \ref NVML_ERROR_INSUFFICIENT_SIZE if \a length is too small 
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not have an infoROM
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlDeviceGetInforomImageVersion
        /// </returns>
        internal static nvmlReturn nvmlDeviceGetInforomVersion(nvmlDevice device, nvmlInforomObject IRobject, out string version) {
            byte[] temp = new byte[NvmlConstant.DeviceInformVersionBufferSize];
            nvmlReturn ret = nvmlDeviceGetInforomVersionInternal(device, IRobject, temp, NvmlConstant.DeviceInformVersionBufferSize);
            version = string.Empty;
            if (ret == nvmlReturn.Success) {
                version = ASCIIEncoding.ASCII.GetString(temp).Replace("\0", "");
            }
            return ret;
        }


        [DllImport(NVML_API_DLL_NAME, EntryPoint = "nvmlDeviceGetInforomImageVersion")]
        private static extern nvmlReturn nvmlDeviceGetInforomImageVersionInternal(nvmlDevice device, byte[] version, uint length);

        /// <summary>
        /// Retrieves the global infoROM image version
        /// For all products with an inforom.
        /// Image version just like VBIOS version uniquely describes the exact version of the infoROM flashed on the board 
        /// in contrast to infoROM object version which is only an indicator of supported features.
        /// Version string will not exceed 16 characters in length (including the NULL terminator).
        /// See \ref nvmlConstants::NVML_DEVICE_INFOROM_VERSION_BUFFER_SIZE.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="version">Reference in which to return the infoROM image version</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a version has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a version is NULL
        ///         - \ref NVML_ERROR_INSUFFICIENT_SIZE if \a length is too small 
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not have an infoROM
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlDeviceGetInforomVersion
        /// </returns>
        internal static nvmlReturn nvmlDeviceGetInforomImageVersion(nvmlDevice device, out string version) {
            byte[] temp = new byte[NvmlConstant.DeviceInformVersionBufferSize];
            nvmlReturn ret = nvmlDeviceGetInforomImageVersionInternal(device, temp, NvmlConstant.DeviceInformVersionBufferSize);
            version = string.Empty;
            if (ret == nvmlReturn.Success) {
                version = ASCIIEncoding.ASCII.GetString(temp).Replace("\0", "");
            }
            return ret;
        }

        /// <summary>
        /// Retrieves the checksum of the configuration stored in the device's infoROM.
        /// For all products with an inforom.
        /// Can be used to make sure that two GPUs have the exact same configuration.
        /// Current checksum takes into account configuration stored in PWR and ECC infoROM objects.
        /// Checksum can change between driver releases or when user changes configuration (e.g. disable/enable ECC)
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="checksum">Reference in which to return the infoROM configuration checksum</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a checksum has been set
        ///         - \ref NVML_ERROR_CORRUPTED_INFOROM if the device's checksum couldn't be retrieved due to infoROM corruption
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a checksum is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error 
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetInforomConfigurationChecksum(nvmlDevice device, ref uint checksum);


        /// <summary>
        /// Reads the infoROM from the flash and verifies the checksums.
        /// For all products with an inforom.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if infoROM is not corrupted
        ///         - \ref NVML_ERROR_CORRUPTED_INFOROM if the device's infoROM is corrupted
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error 
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceValidateInforom(nvmlDevice device);


        /// <summary>
        /// Retrieves the display mode for the device.
        /// For all products.
        /// This method indicates whether a physical display (e.g. monitor) is currently connected to
        /// any of the device's connectors.
        /// See \ref nvmlEnableState for details on allowed modes.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="display">Reference in which to return the display mode</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a display has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a display is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetDisplayMode(nvmlDevice device, ref nvmlEnableState display);


        /// <summary>
        /// Retrieves the display active state for the device.
        /// For all products.
        /// This method indicates whether a display is initialized on the device.
        /// For example whether X Server is attached to this device and has allocated memory for the screen.
        /// Display can be active even when no monitor is physically attached.
        /// See \ref nvmlEnableState for details on allowed modes.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="isActive">Reference in which to return the display active state</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a isActive has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a isActive is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetDisplayActive(nvmlDevice device, ref nvmlEnableState isActive);


        /// <summary>
        /// Retrieves the persistence mode associated with this device.
        /// For all products.
        /// For Linux only.
        /// When driver persistence mode is enabled the driver software state is not torn down when the last 
        /// client disconnects. By default this feature is disabled. 
        /// See \ref nvmlEnableState for details on allowed modes.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="mode">Reference in which to return the current driver persistence mode</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a mode has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a mode is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlDeviceSetPersistenceMode()
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetPersistenceMode(nvmlDevice device, ref nvmlEnableState mode);


        /// <summary>
        /// Retrieves the PCI attributes of this device.
        /// 
        /// For all products.
        /// See \ref nvmlPciInfo for details on the available PCI info.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="pci">Reference in which to return the PCI info</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a pci has been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a pci is NULL
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME, EntryPoint = "nvmlDeviceGetPciInfo_v2")]
        internal static extern nvmlReturn nvmlDeviceGetPciInfo(nvmlDevice device, ref nvmlPciInfo pci);


        /// <summary>
        /// Retrieves the maximum PCIe link generation possible with this device and system
        /// I.E. for a generation 2 PCIe device attached to a generation 1 PCIe bus the max link generation this function will
        /// report is generation 1.
        /// 
        /// For Fermi or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="maxLinkGen">Reference in which to return the max PCIe link generation</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a maxLinkGen has been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a maxLinkGen is null
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if PCIe link information is not available
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetMaxPcieLinkGeneration(nvmlDevice device, ref uint maxLinkGen);


        /// <summary>
        /// Retrieves the maximum PCIe link width possible with this device and system
        /// I.E. for a device with a 16x PCIe bus width attached to a 8x PCIe system bus this function will report
        /// a max link width of 8.
        /// 
        /// For Fermi or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="maxLinkWidth">Reference in which to return the max PCIe link generation</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a maxLinkWidth has been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a maxLinkWidth is null
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if PCIe link information is not available
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetMaxPcieLinkWidth(nvmlDevice device, ref uint maxLinkWidth);


        /// <summary>
        /// Retrieves the current PCIe link generation
        /// 
        /// For Fermi or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="currLinkGen">Reference in which to return the current PCIe link generation</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a currLinkGen has been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a currLinkGen is null
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if PCIe link information is not available
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetCurrPcieLinkGeneration(nvmlDevice device, ref uint currLinkGen);


        /// <summary>
        /// Retrieves the current PCIe link width
        /// 
        /// For Fermi or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="currLinkWidth">Reference in which to return the current PCIe link generation</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a currLinkWidth has been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a currLinkWidth is null
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if PCIe link information is not available
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetCurrPcieLinkWidth(nvmlDevice device, ref uint currLinkWidth);


        /// <summary>
        /// Retrieve PCIe utilization information.
        /// This function is querying a byte counter over a 20ms interval and thus is the 
        ///   PCIe throughput over that interval.
        /// For Maxwell or newer fully supported devices.
        /// This method is not supported on virtualized GPU environments.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="counter">The specific counter that should be queried \ref nvmlPcieUtilCounter</param>
        /// <param name="value">Reference in which to return throughput in KB/s</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a value has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device or \a counter is invalid, or \a value is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetPcieThroughput(nvmlDevice device, nvmlPcieUtilCounter counter, ref uint value);


        /// <summary>
        /// Retrieve the PCIe replay counter.
        /// For Kepler or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="value">Reference in which to return the counter's value</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a value and \a rollover have been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, or \a value or \a rollover are NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetPcieReplayCounter(nvmlDevice device, ref uint value);


        /// <summary>
        /// Retrieves the current clock speeds for the device.
        /// For Fermi or newer fully supported devices.
        /// See \ref nvmlClockType for details on available clock information.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="type">Identify which clock domain to query</param>
        /// <param name="clock">Reference in which to return the clock speed in MHz</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a clock has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a clock is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device cannot report the specified clock
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetClockInfo(nvmlDevice device, nvmlClockType type, ref uint clock);


        /// <summary>
        /// Retrieves the maximum clock speeds for the device.
        /// For Fermi or newer fully supported devices.
        /// See \ref nvmlClockType for details on available clock information.
        /// \note On GPUs from Fermi family current P0 clocks (reported by \ref nvmlDeviceGetClockInfo) can differ from max clocks
        ///       by few MHz.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="type">Identify which clock domain to query</param>
        /// <param name="clock">Reference in which to return the clock speed in MHz</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a clock has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a clock is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device cannot report the specified clock
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetMaxClockInfo(nvmlDevice device, nvmlClockType type, ref uint clock);


        /// <summary>
        /// Retrieves the current setting of a clock that applications will use unless an overspec situation occurs.
        /// Can be changed using \ref nvmlDeviceSetApplicationsClocks.
        /// For Kepler or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="clockType">Identify which clock domain to query</param>
        /// <param name="clockMHz">Reference in which to return the clock in MHz</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a clockMHz has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a clockMHz is NULL or \a clockType is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetApplicationsClock(nvmlDevice device, nvmlClockType clockType, ref uint clockMHz);


        /// <summary>
        /// Retrieves the default applications clock that GPU boots with or 
        /// defaults to after \ref nvmlDeviceResetApplicationsClocks call.
        /// For Kepler or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="clockType">Identify which clock domain to query</param>
        /// <param name="clockMHz">Reference in which to return the default clock in MHz</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a clockMHz has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a clockMHz is NULL or \a clockType is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// \see nvmlDeviceGetApplicationsClock
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetDefaultApplicationsClock(nvmlDevice device, nvmlClockType clockType, ref uint clockMHz);


        /// <summary>
        /// Resets the application clock to the default value
        /// This is the applications clock that will be used after system reboot or driver reload.
        /// Default value is constant, but the current value an be changed using \ref nvmlDeviceSetApplicationsClocks.
        /// @see nvmlDeviceGetApplicationsClock
        /// @see nvmlDeviceSetApplicationsClocks
        /// For Fermi or newer non-GeForce fully supported devices and Maxwell or newer GeForce devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if new settings were successfully set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceResetApplicationsClocks(nvmlDevice device);


        /// <summary>
        /// Retrieves the clock speed for the clock specified by the clock type and clock ID.
        /// For Kepler or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="clockType">Identify which clock domain to query</param>
        /// <param name="clockId">Identify which clock in the domain to query</param>
        /// <param name="clockMHz">Reference in which to return the clock in MHz</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a clockMHz has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a clockMHz is NULL or \a clockType is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetClock(nvmlDevice device, nvmlClockType clockType, nvmlClockId clockId, ref uint clockMHz);


        /// <summary>
        /// Retrieves the customer defined maximum boost clock speed specified by the given clock type.
        /// For newer than Maxwell fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="clockType">Identify which clock domain to query</param>
        /// <param name="clockMHz">Reference in which to return the clock in MHz</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a clockMHz has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a clockMHz is NULL or \a clockType is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device or the \a clockType on this device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetMaxCustomerBoostClock(nvmlDevice device, nvmlClockType clockType, ref uint clockMHz);


        /// <summary>
        /// Retrieves the list of possible memory clocks that can be used as an argument for \ref nvmlDeviceSetApplicationsClocks.
        /// For Kepler or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="count">Reference in which to provide the \a clocksMHz array size, and</param>
        /// <param name="clocksMHz">Reference in which to return the clock in MHz</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a count and \a clocksMHz have been populated 
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a count is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_INSUFFICIENT_SIZE if \a count is too small (\a count is set to the number of
        ///                                                required elements)
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlDeviceSetApplicationsClocks
        /// @see nvmlDeviceGetSupportedGraphicsClocks
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetSupportedMemoryClocks(nvmlDevice device, ref uint count, uint[] clocksMHz);


        /// <summary>
        /// Retrieves the list of possible graphics clocks that can be used as an argument for \ref nvmlDeviceSetApplicationsClocks.
        /// For Kepler or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="memoryClockMHz">Memory clock for which to return possible graphics clocks</param>
        /// <param name="count">Reference in which to provide the \a clocksMHz array size, and</param>
        /// <param name="clocksMHz">Reference in which to return the clocks in MHz</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a count and \a clocksMHz have been populated 
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_NOT_FOUND         if the specified \a memoryClockMHz is not a supported frequency
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a clock is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_INSUFFICIENT_SIZE if \a count is too small 
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlDeviceSetApplicationsClocks
        /// @see nvmlDeviceGetSupportedMemoryClocks
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetSupportedGraphicsClocks(nvmlDevice device, uint memoryClockMHz, ref uint count, uint[] clocksMHz);


        /// <summary>
        /// Retrieve the current state of auto boosted clocks on a device and store it in \a isEnabled
        /// For Kepler or newer fully supported devices.
        /// Auto boosted clocks are enabled by default on some hardware, allowing the GPU to run at higher clock rates
        /// to maximize performance as thermal limits allow.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="isEnabled">Where to store the current state of auto boosted clocks of the target device</param>
        /// <param name="defaultIsEnabled">Where to store the default auto boosted clocks behavior of the target device that the device will</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 If \a isEnabled has been been set with the auto boosted clocks state of \a device
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a isEnabled is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support auto boosted clocks
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetAutoBoostedClocksEnabled(nvmlDevice device, ref nvmlEnableState isEnabled, ref nvmlEnableState defaultIsEnabled);


        /// <summary>
        /// Try to set the current state of auto boosted clocks on a device.
        /// For Kepler or newer fully supported devices.
        /// Auto boosted clocks are enabled by default on some hardware, allowing the GPU to run at higher clock rates
        /// to maximize performance as thermal limits allow. Auto boosted clocks should be disabled if fixed clock
        /// rates are desired.
        /// Non-root users may use this API by default but can be restricted by root from using this API by calling
        /// \ref nvmlDeviceSetAPIRestriction with apiType=NVML_RESTRICTED_API_SET_AUTO_BOOSTED_CLOCKS.
        /// Note: Persistence Mode is required to modify current Auto boost settings, therefore, it must be enabled.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="enabled">What state to try to set auto boosted clocks of the target device to</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 If the auto boosted clocks were successfully set to the state specified by \a enabled
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support auto boosted clocks
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceSetAutoBoostedClocksEnabled(nvmlDevice device, nvmlEnableState enabled);


        /// <summary>
        /// Try to set the default state of auto boosted clocks on a device. This is the default state that auto boosted clocks will
        /// return to when no compute running processes (e.g. CUDA application which have an active context) are running
        /// For Kepler or newer non-GeForce fully supported devices and Maxwell or newer GeForce devices.
        /// Requires root/admin permissions.
        /// Auto boosted clocks are enabled by default on some hardware, allowing the GPU to run at higher clock rates
        /// to maximize performance as thermal limits allow. Auto boosted clocks should be disabled if fixed clock
        /// rates are desired.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="enabled">What state to try to set default auto boosted clocks of the target device to</param>
        /// <param name="flags">Flags that change the default behavior. Currently Unused.</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 If the auto boosted clock's default state was successfully set to the state specified by \a enabled
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_NO_PERMISSION     If the calling user does not have permission to change auto boosted clock's default state.
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support auto boosted clocks
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceSetDefaultAutoBoostedClocksEnabled(nvmlDevice device, nvmlEnableState enabled, uint flags);


        /// <summary>
        /// Retrieves the intended operating speed of the device's fan.
        /// Note: The reported speed is the intended fan speed.  If the fan is physically blocked and unable to spin, the
        /// output will not match the actual fan speed.
        /// 
        /// For all discrete products with dedicated fans.
        /// The fan speed is expressed as a percent of the maximum, i.e. full speed is 100%.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="speed">Reference in which to return the fan speed percentage</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a speed has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a speed is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not have a fan
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetFanSpeed(nvmlDevice device, ref uint speed);


        /// <summary>
        /// Retrieves the current temperature readings for the device, in degrees C. 
        /// 
        /// For all products.
        /// See \ref nvmlTemperatureSensors for details on available temperature sensors.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="sensorType">Flag that indicates which sensor reading to retrieve</param>
        /// <param name="temp">Reference in which to return the temperature reading</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a temp has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, \a sensorType is invalid or \a temp is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not have the specified sensor
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetTemperature(nvmlDevice device, nvmlTemperatureSensors sensorType, ref uint temp);


        /// <summary>
        /// Retrieves the temperature threshold for the GPU with the specified threshold type in degrees C.
        /// For Kepler or newer fully supported devices.
        /// See \ref nvmlTemperatureThresholds for details on available temperature thresholds.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="thresholdType">The type of threshold value queried</param>
        /// <param name="temp">Reference in which to return the temperature reading</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a temp has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, \a thresholdType is invalid or \a temp is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not have a temperature sensor or is unsupported
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetTemperatureThreshold(nvmlDevice device, nvmlTemperatureThresholds thresholdType, ref uint temp);


        /// <summary>
        /// Retrieves the current performance state for the device. 
        /// For Fermi or newer fully supported devices.
        /// See \ref nvmlPstates for details on allowed performance states.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="pState">Reference in which to return the performance state reading</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a pState has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a pState is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetPerformanceState(nvmlDevice device, ref nvmlPstates pState);


        /// <summary>
        /// Retrieves current clocks throttling reasons.
        /// For all fully supported products.
        /// \note More than one bit can be enabled at the same time. Multiple reasons can be affecting clocks at once.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="clocksThrottleReasons">Reference in which to return bitmask of active clocks throttle</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a clocksThrottleReasons has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a clocksThrottleReasons is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlClocksThrottleReasons
        /// @see nvmlDeviceGetSupportedClocksThrottleReasons
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetCurrentClocksThrottleReasons(nvmlDevice device, ref ulong clocksThrottleReasons);


        /// <summary>
        /// Retrieves bitmask of supported clocks throttle reasons that can be returned by 
        /// \ref nvmlDeviceGetCurrentClocksThrottleReasons
        /// For all fully supported products.
        /// This method is not supported on virtualized GPU environments.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="supportedClocksThrottleReasons">Reference in which to return bitmask of supported</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a supportedClocksThrottleReasons has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a supportedClocksThrottleReasons is NULL
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlClocksThrottleReasons
        /// @see nvmlDeviceGetCurrentClocksThrottleReasons
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetSupportedClocksThrottleReasons(nvmlDevice device, ref ulong supportedClocksThrottleReasons);


        /// <summary>
        /// Retrieves the power management limit associated with this device.
        /// For Fermi or newer fully supported devices.
        /// The power limit defines the upper boundary for the card's power draw. If
        /// the card's total power draw reaches this limit the power management algorithm kicks in.
        /// This reading is only available if power management mode is supported. 
        /// See \ref nvmlDeviceGetPowerManagementMode.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="limit">Reference in which to return the power management limit in milliwatts</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a limit has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a limit is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetPowerManagementLimit(nvmlDevice device, ref uint limit);


        /// <summary>
        /// Retrieves information about possible values of power management limits on this device.
        /// For Kepler or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="minLimit">Reference in which to return the minimum power management limit in milliwatts</param>
        /// <param name="maxLimit">Reference in which to return the maximum power management limit in milliwatts</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a minLimit and \a maxLimit have been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a minLimit or \a maxLimit is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlDeviceSetPowerManagementLimit
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetPowerManagementLimitConstraints(nvmlDevice device, ref uint minLimit, ref uint maxLimit);


        /// <summary>
        /// Retrieves default power management limit on this device, in milliwatts.
        /// Default power management limit is a power management limit that the device boots with.
        /// For Kepler or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="defaultLimit">Reference in which to return the default power management limit in milliwatts</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a defaultLimit has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a defaultLimit is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetPowerManagementDefaultLimit(nvmlDevice device, ref uint defaultLimit);


        /// <summary>
        /// Retrieves power usage for this GPU in milliwatts and its associated circuitry (e.g. memory)
        /// For Fermi or newer fully supported devices.
        /// On Fermi and Kepler GPUs the reading is accurate to within +/- 5% of current power draw.
        /// It is only available if power management mode is supported. See \ref nvmlDeviceGetPowerManagementMode.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="power">Reference in which to return the power usage information</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a power has been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a power is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support power readings
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetPowerUsage(nvmlDevice device, ref uint power);


        /// <summary>
        /// Get the effective power limit that the driver enforces after taking into account all limiters
        /// Note: This can be different from the \ref nvmlDeviceGetPowerManagementLimit if other limits are set elsewhere
        /// This includes the out of band power limit interface
        /// For Kepler or newer fully supported devices.
        /// </summary>
        /// <param name="device">The device to communicate with</param>
        /// <param name="limit">Reference in which to return the power management limit in milliwatts</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a limit has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a limit is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetEnforcedPowerLimit(nvmlDevice device, ref uint limit);


        /// <summary>
        /// Retrieves the current GOM and pending GOM (the one that GPU will switch to after reboot).
        /// For GK110 M-class and X-class Tesla products from the Kepler family.
        /// Modes \ref NVML_GOM_LOW_DP and \ref NVML_GOM_ALL_ON are supported on fully supported GeForce products.
        /// Not supported on Quadro and Tesla C-class products.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="current">Reference in which to return the current GOM</param>
        /// <param name="pending">Reference in which to return the pending GOM</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a mode has been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a current or \a pending is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlGpuOperationMode
        /// @see nvmlDeviceSetGpuOperationMode
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetGpuOperationMode(nvmlDevice device, ref nvmlGpuOperationMode current, ref nvmlGpuOperationMode pending);


        /// <summary>
        /// Retrieves the amount of used, free and total memory available on the device, in bytes.
        /// 
        /// For all products.
        /// Enabling ECC reduces the amount of total available memory, due to the extra required parity bits.
        /// Under WDDM most device memory is allocated and managed on startup by Windows.
        /// Under Linux and Windows TCC, the reported amount of used memory is equal to the sum of memory allocated 
        /// by all active channels on the device.
        /// See \ref nvmlMemory for details on available memory info.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="memory">Reference in which to return the memory information</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a memory has been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a memory is NULL
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetMemoryInfo(nvmlDevice device, ref nvmlMemory memory);


        /// <summary>
        /// Retrieves the current compute mode for the device.
        /// For all products.
        /// See \ref nvmlComputeMode for details on allowed compute modes.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="mode">Reference in which to return the current compute mode</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a mode has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a mode is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlDeviceSetComputeMode()
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetComputeMode(nvmlDevice device, ref nvmlComputeMode mode);


        /// <summary>
        /// Retrieves the current and pending ECC modes for the device.
        /// For Fermi or newer fully supported devices.
        /// Only applicable to devices with ECC.
        /// Requires \a NVML_INFOROM_ECC version 1.0 or higher.
        /// Changing ECC modes requires a reboot. The "pending" ECC mode refers to the target mode following
        /// the next reboot.
        /// See \ref nvmlEnableState for details on allowed modes.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="current">Reference in which to return the current ECC mode</param>
        /// <param name="pending">Reference in which to return the pending ECC mode</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a current and \a pending have been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or either \a current or \a pending is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlDeviceSetEccMode()
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetEccMode(nvmlDevice device, ref nvmlEnableState current, ref nvmlEnableState pending);


        /// <summary>
        /// Retrieves the device boardId from 0-N.
        /// Devices with the same boardId indicate GPUs connected to the same PLX.  Use in conjunction with 
        ///  \ref nvmlDeviceGetMultiGpuBoard() to decide if they are on the same board as well.
        ///  The boardId returned is a unique ID for the current configuration.  Uniqueness and ordering across 
        ///  reboots and system configurations is not guaranteed (i.e. if a Tesla K40c returns 0x100 and
        ///  the two GPUs on a Tesla K10 in the same system returns 0x200 it is not guaranteed they will 
        ///  always return those values but they will always be different from each other).
        ///  
        /// For Fermi or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="boardId">Reference in which to return the device's board ID</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a boardId has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a boardId is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetBoardId(nvmlDevice device, ref uint boardId);


        /// <summary>
        /// Retrieves whether the device is on a Multi-GPU Board
        /// Devices that are on multi-GPU boards will set \a multiGpuBool to a non-zero value.
        /// For Fermi or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="multiGpuBool">Reference in which to return a zero or non-zero value</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a multiGpuBool has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a multiGpuBool is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetMultiGpuBoard(nvmlDevice device, ref uint multiGpuBool);


        /// <summary>
        /// Retrieves the total ECC error counts for the device.
        /// For Fermi or newer fully supported devices.
        /// Only applicable to devices with ECC.
        /// Requires \a NVML_INFOROM_ECC version 1.0 or higher.
        /// Requires ECC Mode to be enabled.
        /// The total error count is the sum of errors across each of the separate memory systems, i.e. the total set of 
        /// errors across the entire device.
        /// See \ref nvmlMemoryErrorType for a description of available error types.\n
        /// See \ref nvmlEccCounterType for a description of available counter types.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="errorType">Flag that specifies the type of the errors. </param>
        /// <param name="counterType">Flag that specifies the counter-type of the errors. </param>
        /// <param name="eccCounts">Reference in which to return the specified ECC errors</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a eccCounts has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device, \a errorType or \a counterType is invalid, or \a eccCounts is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlDeviceClearEccErrorCounts()
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetTotalEccErrors(nvmlDevice device, nvmlMemoryErrorType errorType, nvmlEccCounterType counterType, ref ulong eccCounts);



        /// <summary>
        /// Retrieves the requested memory error counter for the device.
        /// For Fermi or newer fully supported devices.
        /// Requires \a NVML_INFOROM_ECC version 2.0 or higher to report aggregate location-based memory error counts.
        /// Requires \a NVML_INFOROM_ECC version 1.0 or higher to report all other memory error counts.
        /// Only applicable to devices with ECC.
        /// Requires ECC Mode to be enabled.
        /// See \ref nvmlMemoryErrorType for a description of available memory error types.\n
        /// See \ref nvmlEccCounterType for a description of available counter types.\n
        /// See \ref nvmlMemoryLocation for a description of available counter locations.\n
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="errorType">Flag that specifies the type of error.</param>
        /// <param name="counterType">Flag that specifies the counter-type of the errors. </param>
        /// <param name="locationType">Specifies the location of the counter. </param>
        /// <param name="count">Reference in which to return the ECC counter</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a count has been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device, \a bitTyp,e \a counterType or \a locationType is
        ///                                             invalid, or \a count is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support ECC error reporting in the specified memory
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetMemoryErrorCounter(nvmlDevice device, nvmlMemoryErrorType errorType, nvmlEccCounterType counterType, nvmlMemoryLocation locationType, ref ulong count);


        /// <summary>
        /// Retrieves the current utilization rates for the device's major subsystems.
        /// For Fermi or newer fully supported devices.
        /// See \ref nvmlUtilization for details on available utilization rates.
        /// \note During driver initialization when ECC is enabled one can see high GPU and Memory Utilization readings.
        ///       This is caused by ECC Memory Scrubbing mechanism that is performed during driver initialization.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="utilization">Reference in which to return the utilization information</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a utilization has been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a utilization is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetUtilizationRates(nvmlDevice device, ref nvmlUtilization utilization);


        /// <summary>
        /// Retrieves the current utilization and sampling size in microseconds for the Encoder
        /// For Kepler or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="utilization">Reference to an uint for encoder utilization info</param>
        /// <param name="samplingPeriodUs">Reference to an uint for the sampling period in US</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a utilization has been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, \a utilization is NULL, or \a samplingPeriodUs is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetEncoderUtilization(nvmlDevice device, ref uint utilization, ref uint samplingPeriodUs);


        /// <summary>
        /// Retrieves the current utilization and sampling size in microseconds for the Decoder
        /// For Kepler or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="utilization">Reference to an uint for decoder utilization info</param>
        /// <param name="samplingPeriodUs">Reference to an uint for the sampling period in US</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a utilization has been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, \a utilization is NULL, or \a samplingPeriodUs is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetDecoderUtilization(nvmlDevice device, ref uint utilization, ref uint samplingPeriodUs);


        /// <summary>
        /// Retrieves the current and pending driver model for the device.
        /// For Fermi or newer fully supported devices.
        /// For windows only.
        /// On Windows platforms the device driver can run in either WDDM or WDM (TCC) mode. If a display is attached
        /// to the device it must run in WDDM mode. TCC mode is preferred if a display is not attached.
        /// See \ref nvmlDriverModel for details on available driver models.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="current">Reference in which to return the current driver model</param>
        /// <param name="pending">Reference in which to return the pending driver model</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if either \a current and/or \a pending have been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or both \a current and \a pending are NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the platform is not windows
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// 
        /// @see nvmlDeviceSetDriverModel()
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetDriverModel(nvmlDevice device, ref nvmlDriverModel current, ref nvmlDriverModel pending);


        [DllImport(NVML_API_DLL_NAME, EntryPoint = "nvmlDeviceGetVbiosVersion")]
        private static extern nvmlReturn nvmlDeviceGetVbiosVersionInternal(nvmlDevice device, byte[] version, uint length);

        /// <summary>
        /// Get VBIOS version of the device.
        /// For all products.
        /// The VBIOS version may change from time to time. It will not exceed 32 characters in length 
        /// (including the NULL terminator).  See \ref nvmlConstants::NVML_DEVICE_VBIOS_VERSION_BUFFER_SIZE.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="version">Reference to which to return the VBIOS version</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a version has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, or \a version is NULL
        ///         - \ref NVML_ERROR_INSUFFICIENT_SIZE if \a length is too small 
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        internal static nvmlReturn nvmlDeviceGetVbiosVersion(nvmlDevice device, out string version) {
            byte[] temp = new byte[NvmlConstant.DeviceVBIOSVersionBufferSize];
            nvmlReturn ret = nvmlDeviceGetVbiosVersionInternal(device, temp, NvmlConstant.DeviceVBIOSVersionBufferSize);
            version = string.Empty;
            if (ret == nvmlReturn.Success) {
                version = ASCIIEncoding.ASCII.GetString(temp).Replace("\0", "");
            }
            return ret;
        }


        /// <summary>
        /// Get Bridge Chip Information for all the bridge chips on the board.
        /// 
        /// For all fully supported products.
        /// Only applicable to multi-GPU products.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="bridgeHierarchy">Reference to the returned bridge chip Hierarchy</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if bridge chip exists
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, or \a bridgeInfo is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if bridge chip not supported on the device
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// 
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetBridgeChipInfo(nvmlDevice device, ref nvmlBridgeChipHierarchy bridgeHierarchy);


        /// <summary>
        /// Get information about processes with a compute context on a device
        /// For Fermi or newer fully supported devices.
        /// This function returns information only about compute running processes (e.g. CUDA application which have
        /// active context). Any graphics applications (e.g. using OpenGL, DirectX) won't be listed by this function.
        /// To query the current number of running compute processes, call this function with *infoCount = 0. The
        /// return code will be NVML_ERROR_INSUFFICIENT_SIZE, or NVML_SUCCESS if none are running. For this call
        /// \a infos is allowed to be NULL.
        /// The usedGpuMemory field returned is all of the memory used by the application.
        /// Keep in mind that information returned by this call is dynamic and the number of elements might change in
        /// time. Allocate more space for \a infos table in case new compute processes are spawned.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="infoCount">Reference in which to provide the \a infos array size, and</param>
        /// <param name="infos">Reference in which to return the process information</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a infoCount and \a infos have been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INSUFFICIENT_SIZE if \a infoCount indicates that the \a infos array is too small
        ///                                             \a infoCount will contain minimal amount of space necessary for
        ///                                             the call to complete
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, either of \a infoCount or \a infos is NULL
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see \ref nvmlSystemGetProcessName
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetComputeRunningProcesses(nvmlDevice device, ref uint infoCount, nvmlProcessInfo[] infos);


        /// <summary>
        /// Get information about processes with a graphics context on a device
        /// For Kepler or newer fully supported devices.
        /// This function returns information only about graphics based processes 
        /// (eg. applications using OpenGL, DirectX)
        /// To query the current number of running graphics processes, call this function with *infoCount = 0. The
        /// return code will be NVML_ERROR_INSUFFICIENT_SIZE, or NVML_SUCCESS if none are running. For this call
        /// \a infos is allowed to be NULL.
        /// The usedGpuMemory field returned is all of the memory used by the application.
        /// Keep in mind that information returned by this call is dynamic and the number of elements might change in
        /// time. Allocate more space for \a infos table in case new graphics processes are spawned.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="infoCount">Reference in which to provide the \a infos array size, and</param>
        /// <param name="infos">Reference in which to return the process information</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a infoCount and \a infos have been populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INSUFFICIENT_SIZE if \a infoCount indicates that the \a infos array is too small
        ///                                             \a infoCount will contain minimal amount of space necessary for
        ///                                             the call to complete
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, either of \a infoCount or \a infos is NULL
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see \ref nvmlSystemGetProcessName
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetGraphicsRunningProcesses(nvmlDevice device, ref uint infoCount, nvmlProcessInfo[] infos);


        /// <summary>
        /// Check if the GPU devices are on the same physical board.
        /// For all fully supported products.
        /// </summary>
        /// <param name="device1">The first GPU device</param>
        /// <param name="device2">The second GPU device</param>
        /// <param name="onSameBoard">Reference in which to return the status.</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a onSameBoard has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a dev1 or \a dev2 are invalid or \a onSameBoard is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if this check is not supported by the device
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the either GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceOnSameBoard(nvmlDevice device1, nvmlDevice device2, ref int onSameBoard);


        /// <summary>
        /// Retrieves the root/admin permissions on the target API. See \a nvmlRestrictedAPI for the list of supported APIs.
        /// If an API is restricted only root users can call that API. See \a nvmlDeviceSetAPIRestriction to change current permissions.
        /// For all fully supported products.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="apiType">Target API type for this operation</param>
        /// <param name="isRestricted">Reference in which to return the current restriction </param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a isRestricted has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, \a apiType incorrect or \a isRestricted is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if this query is not supported by the device or the device does not support
        ///                                                 the feature that is being queried (E.G. Enabling/disabling auto boosted clocks is
        ///                                                 not supported by the device)
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlRestrictedAPI
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetAPIRestriction(nvmlDevice device, nvmlRestrictedAPI apiType, ref nvmlEnableState isRestricted);


        /// <summary>
        /// Gets recent samples for the GPU.
        /// 
        /// For Kepler or newer fully supported devices.
        /// 
        /// Based on type, this method can be used to fetch the power, utilization or clock samples maintained in the buffer by 
        /// the driver.
        /// 
        /// Power, Utilization and Clock samples are returned as type "uint" for the union nvmlValue.
        /// 
        /// To get the size of samples that user needs to allocate, the method is invoked with samples set to NULL. 
        /// The returned samplesCount will provide the number of samples that can be queried. The user needs to 
        /// allocate the buffer with size as samplesCount		/// sizeof(nvmlSample).
        /// 
        /// lastSeenTimeStamp represents CPU timestamp in microseconds. Set it to 0 to fetch all the samples maintained by the 
        /// underlying buffer. Set lastSeenTimeStamp to one of the timeStamps retrieved from the date of the previous query 
        /// to get more recent samples.
        /// 
        /// This method fetches the number of entries which can be accommodated in the provided samples array, and the 
        /// reference samplesCount is updated to indicate how many samples were actually retrieved. The advantage of using this 
        /// method for samples in contrast to polling via existing methods is to get get higher frequency data at lower polling cost.
        /// </summary>
        /// <param name="device">The identifier for the target device</param>
        /// <param name="type">Type of sampling event</param>
        /// <param name="lastSeenTimeStamp">Return only samples with timestamp greater than lastSeenTimeStamp. </param>
        /// <param name="sampleValType">Output parameter to represent the type of sample value as described in nvmlSampleVal</param>
        /// <param name="sampleCount">Reference to provide the number of elements which can be queried in samples array</param>
        /// <param name="samples">Reference in which samples are returned</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if samples are successfully retrieved
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, \a samplesCount is NULL or 
        ///                                             reference to \a sampleCount is 0 for non null \a samples
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if this query is not supported by the device
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_NOT_FOUND         if sample entries are not found
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetSamples(nvmlDevice device, nvmlSamplingType type, ulong lastSeenTimeStamp, ref nvmlValueType sampleValType, ref uint sampleCount, nvmlSample[] samples);


        /// <summary>
        /// Gets Total, Available and Used size of BAR1 memory.
        /// 
        /// BAR1 is used to map the FB (device memory) so that it can be directly accessed by the CPU or by 3rd party 
        /// devices (peer-to-peer on the PCIE bus). 
        /// 
        /// For Kepler or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="bar1Memory">Reference in which BAR1 memory</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if BAR1 memory is successfully retrieved
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, \a bar1Memory is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if this query is not supported by the device
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetBAR1MemoryInfo(nvmlDevice device, ref nvmlBAR1Memory bar1Memory);


        /// <summary>
        /// Gets the duration of time during which the device was throttled (lower than requested clocks) due to power 
        /// or thermal constraints.
        /// The method is important to users who are tying to understand if their GPUs throttle at any point during their applications. The
        /// difference in violation times at two different reference times gives the indication of GPU throttling event. 
        /// Violation for thermal capping is not supported at this time.
        /// 
        /// For Kepler or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="perfPolicyType">Represents Performance policy which can trigger GPU throttling</param>
        /// <param name="violTime">Reference to which violation time related information is returned </param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if violation time is successfully retrieved
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, \a perfPolicyType is invalid, or \a violTime is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if this query is not supported by the device
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetViolationStatus(nvmlDevice device, nvmlPerfPolicyType perfPolicyType, ref nvmlViolationTime violTime);


        /// <summary>
        /// Queries the state of per process accounting mode.
        /// For Kepler or newer fully supported devices.
        /// See \ref nvmlDeviceGetAccountingStats for more details.
        /// See \ref nvmlDeviceSetAccountingMode
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="mode">Reference in which to return the current accounting mode</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the mode has been successfully retrieved 
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a mode are NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetAccountingMode(nvmlDevice device, ref nvmlEnableState mode);


        /// <summary>
        /// Queries process's accounting stats.
        /// For Kepler or newer fully supported devices.
        /// 
        /// Accounting stats capture GPU utilization and other statistics across the lifetime of a process.
        /// Accounting stats can be queried during life time of the process and after its termination.
        /// The time field in \ref nvmlAccountingStats is reported as 0 during the lifetime of the process and 
        /// updated to actual running time after its termination.
        /// Accounting stats are kept in a circular buffer, newly created processes overwrite information about old
        /// processes.
        /// See \ref nvmlAccountingStats for description of each returned metric.
        /// List of processes that can be queried can be retrieved from \ref nvmlDeviceGetAccountingPids.
        /// @note Accounting Mode needs to be on. See \ref nvmlDeviceGetAccountingMode.
        /// @note Only compute and graphics applications stats can be queried. Monitoring applications stats can't be
        ///         queried since they don't contribute to GPU utilization.
        /// @note In case of pid collision stats of only the latest process (that terminated last) will be reported
        /// @warning On Kepler devices per process statistics are accurate only if there's one process running on a GPU.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="pid">Process Id of the target process to query stats for</param>
        /// <param name="stats">Reference in which to return the process's accounting stats</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if stats have been successfully retrieved
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a stats are NULL
        ///         - \ref NVML_ERROR_NOT_FOUND         if process stats were not found
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature or accounting mode is disabled
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlDeviceGetAccountingBufferSize
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetAccountingStats(nvmlDevice device, uint pid, ref nvmlAccountingStats stats);


        /// <summary>
        /// Queries list of processes that can be queried for accounting stats. The list of processes returned 
        /// can be in running or terminated state.
        /// For Kepler or newer fully supported devices.
        /// To just query the number of processes ready to be queried, call this function with *count = 0 and
        /// pids=NULL. The return code will be NVML_ERROR_INSUFFICIENT_SIZE, or NVML_SUCCESS if list is empty.
        /// 
        /// For more details see \ref nvmlDeviceGetAccountingStats.
        /// @note In case of PID collision some processes might not be accessible before the circular buffer is full.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="count">Reference in which to provide the \a pids array size, and</param>
        /// <param name="pids">Reference in which to return list of process ids</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if pids were successfully retrieved
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a count is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature or accounting mode is disabled
        ///         - \ref NVML_ERROR_INSUFFICIENT_SIZE if \a count is too small (\a count is set to
        ///                                                 expected value)
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlDeviceGetAccountingBufferSize
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetAccountingPids(nvmlDevice device, ref uint count, uint[] pids);


        /// <summary>
        /// Returns the number of processes that the circular buffer with accounting pids can hold.
        /// For Kepler or newer fully supported devices.
        /// This is the maximum number of processes that accounting information will be stored for before information
        /// about oldest processes will get overwritten by information about new processes.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="bufferSize">Reference in which to provide the size (in number of elements)</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if buffer size was successfully retrieved
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a bufferSize is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature or accounting mode is disabled
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// 
        /// @see nvmlDeviceGetAccountingStats
        /// @see nvmlDeviceGetAccountingPids
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetAccountingBufferSize(nvmlDevice device, ref uint bufferSize);


        /// <summary>
        /// Returns the list of retired pages by source, including pages that are pending retirement
        /// The address information provided from this API is the hardware address of the page that was retired.  Note
        /// that this does not match the virtual address used in CUDA, but will match the address information in XID 63
        /// 
        /// For Kepler or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="cause">Filter page addresses by cause of retirement</param>
        /// <param name="pageCount">Reference in which to provide the \a addresses buffer size, and</param>
        /// <param name="addresses">Buffer to write the page addresses into</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a pageCount was populated and \a addresses was filled
        ///         - \ref NVML_ERROR_INSUFFICIENT_SIZE if \a pageCount indicates the buffer is not large enough to store all the
        ///                                             matching page addresses.  \a pageCount is set to the needed size.
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid, \a pageCount is NULL, \a cause is invalid, or 
        ///                                             \a addresses is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetRetiredPages(nvmlDevice device, nvmlPageRetirementCause cause, ref uint pageCount, ulong[] addresses);


        /// <summary>
        /// Check if any pages are pending retirement and need a reboot to fully retire.
        /// For Kepler or newer fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="isPending">Reference in which to return the pending status</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a isPending was populated
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a isPending is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetRetiredPagesPendingStatus(nvmlDevice device, ref nvmlEnableState isPending);


        /// <summary>
        /// Set the LED state for the unit. The LED can be either green (0) or amber (1).
        /// For S-class products.
        /// Requires root/admin permissions.
        /// This operation takes effect immediately.
        /// 
        /// <b>Current S-Class products don't provide unique LEDs for each unit. As such, both front 
        /// and back LEDs will be toggled in unison regardless of which unit is specified with this command.</b>
        /// See \ref nvmlLedColor for available colors.
        /// </summary>
        /// <param name="unit">The identifier of the target unit</param>
        /// <param name="color">The target LED color</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the LED color has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a unit or \a color is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if this is not an S-class product
        ///         - \ref NVML_ERROR_NO_PERMISSION     if the user doesn't have permission to perform this operation
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// 
        /// @see nvmlUnitGetLedState()
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlUnitSetLedState(nvmlUnit unit, nvmlLedColor color);


        /// <summary>
        /// Set the persistence mode for the device.
        /// For all products.
        /// For Linux only.
        /// Requires root/admin permissions.
        /// The persistence mode determines whether the GPU driver software is torn down after the last client
        /// exits.
        /// This operation takes effect immediately. It is not persistent across reboots. After each reboot the
        /// persistence mode is reset to "Disabled".
        /// See \ref nvmlEnableState for available modes.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="mode">The target persistence mode</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the persistence mode was set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a mode is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_NO_PERMISSION     if the user doesn't have permission to perform this operation
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlDeviceGetPersistenceMode()
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceSetPersistenceMode(nvmlDevice device, nvmlEnableState mode);


        /// <summary>
        /// Set the compute mode for the device.
        /// For all products.
        /// Requires root/admin permissions.
        /// The compute mode determines whether a GPU can be used for compute operations and whether it can
        /// be shared across contexts.
        /// This operation takes effect immediately. Under Linux it is not persistent across reboots and
        /// always resets to "Default". Under windows it is persistent.
        /// Under windows compute mode may only be set to DEFAULT when running in WDDM
        /// See \ref nvmlComputeMode for details on available compute modes.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="mode">The target compute mode</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the compute mode was set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a mode is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_NO_PERMISSION     if the user doesn't have permission to perform this operation
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlDeviceGetComputeMode()
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceSetComputeMode(nvmlDevice device, nvmlComputeMode mode);


        /// <summary>
        /// Set the ECC mode for the device.
        /// For Kepler or newer fully supported devices.
        /// Only applicable to devices with ECC.
        /// Requires \a NVML_INFOROM_ECC version 1.0 or higher.
        /// Requires root/admin permissions.
        /// The ECC mode determines whether the GPU enables its ECC support.
        /// This operation takes effect after the next reboot.
        /// See \ref nvmlEnableState for details on available modes.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="ecc">The target ECC mode</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the ECC mode was set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a ecc is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_NO_PERMISSION     if the user doesn't have permission to perform this operation
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlDeviceGetEccMode()
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceSetEccMode(nvmlDevice device, nvmlEnableState ecc);


        /// <summary>
        /// Clear the ECC error and other memory error counts for the device.
        /// For Kepler or newer fully supported devices.
        /// Only applicable to devices with ECC.
        /// Requires \a NVML_INFOROM_ECC version 2.0 or higher to clear aggregate location-based ECC counts.
        /// Requires \a NVML_INFOROM_ECC version 1.0 or higher to clear all other ECC counts.
        /// Requires root/admin permissions.
        /// Requires ECC Mode to be enabled.
        /// Sets all of the specified ECC counters to 0, including both detailed and total counts.
        /// This operation takes effect immediately.
        /// See \ref nvmlMemoryErrorType for details on available counter types.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="counterType">Flag that indicates which type of errors should be cleared.</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the error counts were cleared
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a counterType is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_NO_PERMISSION     if the user doesn't have permission to perform this operation
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see 
        ///      - nvmlDeviceGetDetailedEccErrors()
        ///      - nvmlDeviceGetTotalEccErrors()
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceClearEccErrorCounts(nvmlDevice device, nvmlEccCounterType counterType);


        /// <summary>
        /// Set the driver model for the device.
        /// For Fermi or newer fully supported devices.
        /// For windows only.
        /// Requires root/admin permissions.
        /// On Windows platforms the device driver can run in either WDDM or WDM (TCC) mode. If a display is attached
        /// to the device it must run in WDDM mode.  
        /// It is possible to force the change to WDM (TCC) while the display is still attached with a force flag (nvmlFlagForce).
        /// This should only be done if the host is subsequently powered down and the display is detached from the device
        /// before the next reboot. 
        /// This operation takes effect after the next reboot.
        /// 
        /// Windows driver model may only be set to WDDM when running in DEFAULT compute mode.
        /// Change driver model to WDDM is not supported when GPU doesn't support graphics acceleration or 
        /// will not support it after reboot. See \ref nvmlDeviceSetGpuOperationMode.
        /// See \ref nvmlDriverModel for details on available driver models.
        /// See \ref nvmlFlagDefault and \ref nvmlFlagForce
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="driverModel">The target driver model</param>
        /// <param name="flags">Flags that change the default behavior</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the driver model has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a driverModel is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the platform is not windows or the device does not support this feature
        ///         - \ref NVML_ERROR_NO_PERMISSION     if the user doesn't have permission to perform this operation
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// 
        /// @see nvmlDeviceGetDriverModel()
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceSetDriverModel(nvmlDevice device, nvmlDriverModel driverModel, uint flags);


        /// <summary>
        /// Set clocks that applications will lock to.
        /// Sets the clocks that compute and graphics applications will be running at.
        /// e.g. CUDA driver requests these clocks during context creation which means this property 
        /// defines clocks at which CUDA applications will be running unless some overspec event
        /// occurs (e.g. over power, over thermal or external HW brake).
        /// Can be used as a setting to request constant performance.
        /// For Kepler or newer non-GeForce fully supported devices and Maxwell or newer GeForce devices.
        /// Requires root/admin permissions. 
        /// See \ref nvmlDeviceGetSupportedMemoryClocks and \ref nvmlDeviceGetSupportedGraphicsClocks 
        /// for details on how to list available clocks combinations.
        /// After system reboot or driver reload applications clocks go back to their default value.
        /// See \ref nvmlDeviceResetApplicationsClocks.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="memClockMHz">Requested memory clock in MHz</param>
        /// <param name="graphicsClockMHz">Requested graphics clock in MHz</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if new settings were successfully set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a memClockMHz and \a graphicsClockMHz 
        ///                                                 is not a valid clock combination
        ///         - \ref NVML_ERROR_NO_PERMISSION     if the user doesn't have permission to perform this operation 
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceSetApplicationsClocks(nvmlDevice device, uint memClockMHz, uint graphicsClockMHz);


        /// <summary>
        /// Set new power limit of this device.
        /// 
        /// For Kepler or newer fully supported devices.
        /// Requires root/admin permissions.
        /// See \ref nvmlDeviceGetPowerManagementLimitConstraints to check the allowed ranges of values.
        /// \note Limit is not persistent across reboots or driver unloads.
        /// Enable persistent mode to prevent driver from unloading when no application is using the device.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="limit">Power management limit in milliwatts to set</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a limit has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a defaultLimit is out of range
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support this feature
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlDeviceGetPowerManagementLimitConstraints
        /// @see nvmlDeviceGetPowerManagementDefaultLimit
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceSetPowerManagementLimit(nvmlDevice device, uint limit);


        /// <summary>
        /// Sets new GOM. See \a nvmlGpuOperationMode for details.
        /// For GK110 M-class and X-class Tesla products from the Kepler family.
        /// Modes \ref NVML_GOM_LOW_DP and \ref NVML_GOM_ALL_ON are supported on fully supported GeForce products.
        /// Not supported on Quadro and Tesla C-class products.
        /// Requires root/admin permissions.
        /// 
        /// Changing GOMs requires a reboot. 
        /// The reboot requirement might be removed in the future.
        /// Compute only GOMs don't support graphics acceleration. Under windows switching to these GOMs when
        /// pending driver model is WDDM is not supported. See \ref nvmlDeviceSetDriverModel.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="mode">Target GOM</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a mode has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a mode incorrect
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support GOM or specific mode
        ///         - \ref NVML_ERROR_NO_PERMISSION     if the user doesn't have permission to perform this operation
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlGpuOperationMode
        /// @see nvmlDeviceGetGpuOperationMode
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceSetGpuOperationMode(nvmlDevice device, nvmlGpuOperationMode mode);


        /// <summary>
        /// Changes the root/admin restructions on certain APIs. See \a nvmlRestrictedAPI for the list of supported APIs.
        /// This method can be used by a root/admin user to give non-root/admin access to certain otherwise-restricted APIs.
        /// The new setting lasts for the lifetime of the NVIDIA driver it is not persistent. See \a nvmlDeviceGetAPIRestriction
        /// to query the current restriction settings.
        /// 
        /// For Kepler or newer fully supported devices.
        /// Requires root/admin permissions.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="apiType">Target API type for this operation</param>
        /// <param name="isRestricted">The target restriction</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a isRestricted has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device is invalid or \a apiType incorrect
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device does not support changing API restrictions or the device does not support
        ///                                                 the feature that api restrictions are being set for (E.G. Enabling/disabling auto 
        ///                                                 boosted clocks is not supported by the device)
        ///         - \ref NVML_ERROR_NO_PERMISSION     if the user doesn't have permission to perform this operation
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// @see nvmlRestrictedAPI
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceSetAPIRestriction(nvmlDevice device, nvmlRestrictedAPI apiType, nvmlEnableState isRestricted);


        /// <summary>
        /// Enables or disables per process accounting.
        /// For Kepler or newer fully supported devices.
        /// Requires root/admin permissions.
        /// @note This setting is not persistent and will default to disabled after driver unloads.
        ///       Enable persistence mode to be sure the setting doesn't switch off to disabled.
        /// 
        /// @note Enabling accounting mode has no negative impact on the GPU performance.
        /// @note Disabling accounting clears all accounting pids information.
        /// See \ref nvmlDeviceGetAccountingMode
        /// See \ref nvmlDeviceGetAccountingStats
        /// See \ref nvmlDeviceClearAccountingPids
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="mode">The target accounting mode</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the new mode has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device or \a mode are invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_NO_PERMISSION     if the user doesn't have permission to perform this operation
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceSetAccountingMode(nvmlDevice device, nvmlEnableState mode);


        /// <summary>
        /// Clears accounting information about all processes that have already terminated.
        /// For Kepler or newer fully supported devices.
        /// Requires root/admin permissions.
        /// See \ref nvmlDeviceGetAccountingMode
        /// See \ref nvmlDeviceGetAccountingStats
        /// See \ref nvmlDeviceSetAccountingMode
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if accounting information has been cleared 
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device are invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_NO_PERMISSION     if the user doesn't have permission to perform this operation
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceClearAccountingPids(nvmlDevice device);


        /// <summary>
        /// Retrieves the state of the device's NvLink for the link specified
        /// For newer than Maxwell fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="link">Specifies the NvLink link to be queried</param>
        /// <param name="isActive">\a nvmlEnableState where NVML_FEATURE_ENABLED indicates that</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a isActive has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device or \a link is invalid or \a isActive is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetNvLinkState(nvmlDevice device, uint link, ref nvmlEnableState isActive);


        /// <summary>
        /// Retrieves the version of the device's NvLink for the link specified
        /// For newer than Maxwell fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="link">Specifies the NvLink link to be queried</param>
        /// <param name="version">Requested NvLink version</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a version has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device or \a link is invalid or \a version is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetNvLinkVersion(nvmlDevice device, uint link, ref uint version);


        /// <summary>
        /// Retrieves the requested capability from the device's NvLink for the link specified
        /// Please refer to the \a nvmlNvLinkCapability structure for the specific caps that can be queried
        /// The return value should be treated as a boolean.
        /// For newer than Maxwell fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="link">Specifies the NvLink link to be queried</param>
        /// <param name="capability">Specifies the \a nvmlNvLinkCapability to be queried</param>
        /// <param name="capResult">A boolean for the queried capability indicating that feature is available</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a capResult has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device, \a link, or \a capability is invalid or \a capResult is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetNvLinkCapability(nvmlDevice device, uint link, nvmlNvLinkCapability capability, ref uint capResult);


        /// <summary>
        /// Retrieves the PCI information for the remote node on a NvLink link 
        /// Note: pciSubSystemId is not filled in this function and is indeterminate
        /// For newer than Maxwell fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="link">Specifies the NvLink link to be queried</param>
        /// <param name="pci">\a nvmlPciInfo of the remote node for the specified link                            </param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a pci has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device or \a link is invalid or \a pci is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetNvLinkRemotePciInfo(nvmlDevice device, uint link, ref nvmlPciInfo pci);


        /// <summary>
        /// Retrieves the specified error counter value
        /// Please refer to \a nvmlNvLinkErrorCounter for error counters that are available
        /// For newer than Maxwell fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="link">Specifies the NvLink link to be queried</param>
        /// <param name="counter">Specifies the NvLink counter to be queried</param>
        /// <param name="counterValue">Returned counter value</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a counter has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device, \a link, or \a counter is invalid or \a counterValue is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetNvLinkErrorCounter(nvmlDevice device, uint link, nvmlNvLinkErrorCounter counter, ref ulong counterValue);


        /// <summary>
        /// Resets all error counters to zero
        /// Please refer to \a nvmlNvLinkErrorCounter for the list of error counters that are reset
        /// For newer than Maxwell fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="link">Specifies the NvLink link to be queried</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the reset is successful
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device or \a link is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceResetNvLinkErrorCounters(nvmlDevice device, uint link);


        /// <summary>
        /// Set the NVLINK utilization counter control information for the specified counter, 0 or 1.
        /// Please refer to \a nvmlNvLinkUtilizationControl for the structure definition.  Performs a reset
        /// of the counters if the reset parameter is non-zero.
        /// For newer than Maxwell fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="counter">Specifies the counter that should be set (0 or 1).</param>
        /// <param name="link">Specifies the NvLink link to be queried</param>
        /// <param name="control">A reference to the \a nvmlNvLinkUtilizationControl to set</param>
        /// <param name="reset">Resets the counters on set if non-zero</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the control has been set successfully
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device, \a counter, \a link, or \a control is invalid 
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceSetNvLinkUtilizationControl(nvmlDevice device, uint link, uint counter, ref nvmlNvLinkUtilizationControl control, uint reset);


        /// <summary>
        /// Get the NVLINK utilization counter control information for the specified counter, 0 or 1.
        /// Please refer to \a nvmlNvLinkUtilizationControl for the structure definition
        /// For newer than Maxwell fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="counter">Specifies the counter that should be set (0 or 1).</param>
        /// <param name="link">Specifies the NvLink link to be queried</param>
        /// <param name="control">A reference to the \a nvmlNvLinkUtilizationControl to place information</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the control has been set successfully
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device, \a counter, \a link, or \a control is invalid 
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetNvLinkUtilizationControl(nvmlDevice device, uint link, uint counter, ref nvmlNvLinkUtilizationControl control);


        /// <summary>
        /// Retrieve the NVLINK utilization counter based on the current control for a specified counter.
        /// In general it is good practice to use \a nvmlDeviceSetNvLinkUtilizationControl
        ///  before reading the utilization counters as they have no default state
        /// For newer than Maxwell fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="link">Specifies the NvLink link to be queried</param>
        /// <param name="counter">Specifies the counter that should be read (0 or 1).</param>
        /// <param name="rxcounter">Receive counter return value</param>
        /// <param name="txcounter">Transmit counter return value</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if \a rxcounter and \a txcounter have been successfully set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device, \a counter, or \a link is invalid or \a rxcounter or \a txcounter are NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetNvLinkUtilizationCounter(nvmlDevice device, uint link, uint counter, ref ulong rxcounter, ref ulong txcounter);


        /// <summary>
        /// Freeze the NVLINK utilization counters 
        /// Both the receive and transmit counters are operated on by this function
        /// For newer than Maxwell fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="link">Specifies the NvLink link to be queried</param>
        /// <param name="counter">Specifies the counter that should be frozen (0 or 1).</param>
        /// <param name="freeze">NVML_FEATURE_ENABLED = freeze the receive and transmit counters</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if counters were successfully frozen or unfrozen
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device, \a link, \a counter, or \a freeze is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceFreezeNvLinkUtilizationCounter(nvmlDevice device, uint link, uint counter, nvmlEnableState freeze);


        /// <summary>
        /// Reset the NVLINK utilization counters 
        /// Both the receive and transmit counters are operated on by this function
        /// For newer than Maxwell fully supported devices.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="link">Specifies the NvLink link to be reset</param>
        /// <param name="counter">Specifies the counter that should be reset (0 or 1)</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if counters were successfully reset
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a device, \a link, or \a counter is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceResetNvLinkUtilizationCounter(nvmlDevice device, uint link, uint counter);


        /// <summary>
        /// Create an empty set of events.
        /// Event set should be freed by \ref nvmlEventSetFree
        /// For Fermi or newer fully supported devices.
        /// </summary>
        /// <param name="set">Reference in which to return the event handle</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the event has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a set is NULL
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// 
        /// @see nvmlEventSetFree
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlEventSetCreate(ref nvmlEventSet set);


        /// <summary>
        /// Starts recording of events on a specified devices and add the events to specified \ref nvmlEventSet
        /// For Fermi or newer fully supported devices.
        /// Ecc events are available only on ECC enabled devices (see \ref nvmlDeviceGetTotalEccErrors)
        /// Power capping events are available only on Power Management enabled devices (see \ref nvmlDeviceGetPowerManagementMode)
        /// For Linux only.
        /// \b IMPORTANT: Operations on \a set are not thread safe
        /// This call starts recording of events on specific device.
        /// All events that occurred before this call are not recorded.
        /// Checking if some event occurred can be done with \ref nvmlEventSetWait
        /// If function reports NVML_ERROR_UNKNOWN, event set is in undefined state and should be freed.
        /// If function reports NVML_ERROR_NOT_SUPPORTED, event set can still be used. None of the requested eventTypes
        ///     are registered in that case.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="eventTypes">Bitmask of \ref nvmlEventType to record</param>
        /// <param name="set">Set to which add new event types</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the event has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a eventTypes is invalid or \a set is NULL
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the platform does not support this feature or some of requested event types
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// 
        /// @see nvmlEventType
        /// @see nvmlDeviceGetSupportedEventTypes
        /// @see nvmlEventSetWait
        /// @see nvmlEventSetFree
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceRegisterEvents(nvmlDevice device, ulong eventTypes, nvmlEventSet set);


        /// <summary>
        /// Returns information about events supported on device
        /// For Fermi or newer fully supported devices.
        /// Events are not supported on Windows. So this function returns an empty mask in \a eventTypes on Windows.
        /// </summary>
        /// <param name="device">The identifier of the target device</param>
        /// <param name="eventTypes">Reference in which to return bitmask of supported events</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the eventTypes has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a eventType is NULL
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if the target GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// 
        /// @see nvmlEventType
        /// @see nvmlDeviceRegisterEvents
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceGetSupportedEventTypes(nvmlDevice device, ref ulong eventTypes);


        /// <summary>
        /// Waits on events and delivers events
        /// For Fermi or newer fully supported devices.
        /// If some events are ready to be delivered at the time of the call, function returns immediately.
        /// If there are no events ready to be delivered, function sleeps till event arrives 
        /// but not longer than specified timeout. This function in certain conditions can return before
        /// specified timeout passes (e.g. when interrupt arrives)
        /// 
        /// In case of xid error, the function returns the most recent xid error type seen by the system. If there are multiple
        /// xid errors generated before nvmlEventSetWait is invoked then the last seen xid error type is returned for all
        /// xid error events.
        /// </summary>
        /// <param name="set">Reference to set of events to wait on</param>
        /// <param name="data">Reference in which to return event data</param>
        /// <param name="timeoutms">Maximum amount of wait time in milliseconds for registered event</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the data has been set
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a data is NULL
        ///         - \ref NVML_ERRORIMEOUT           if no event arrived in specified timeout or interrupt arrived
        ///         - \ref NVML_ERROR_GPU_IS_LOST       if a GPU has fallen off the bus or is otherwise inaccessible
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// 
        /// @see nvmlEventType
        /// @see nvmlDeviceRegisterEvents
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlEventSetWait(nvmlEventSet set, ref nvmlEventData data, uint timeoutms);


        /// <summary>
        /// Releases events in the set
        /// For Fermi or newer fully supported devices.
        /// </summary>
        /// <param name="set">Reference to events to be released </param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if the event has been successfully released
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// 
        /// @see nvmlDeviceRegisterEvents
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlEventSetFree(nvmlEventSet set);


        /// <summary>
        /// Modify the drain state of a GPU.  This method forces a GPU to no longer accept new incoming requests.
        /// Any new NVML process will see a gap in the enumeration where this GPU should exist as any call to that
        /// GPU outside of the drain state APIs will fail.
        /// Must be called as administrator.
        /// For Linux only.
        /// 
        /// For newer than Maxwell fully supported devices.
        /// Some Kepler devices supported.
        /// </summary>
        /// <param name="nvmlIndex">The ID of the target device</param>
        /// <param name="newState">The drain state that should be entered, see \ref nvmlEnableState</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if counters were successfully reset
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a nvmlIndex or \a newState is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_NO_PERMISSION     if the calling process has insufficient permissions to perform operation
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceModifyDrainState(uint nvmlIndex, nvmlEnableState newState);


        /// <summary>
        /// Query the drain state of a GPU.  This method is used to check if a GPU is in a currently draining
        /// state.
        /// For Linux only.
        /// 
        /// For newer than Maxwell fully supported devices.
        /// Some Kepler devices supported.
        /// </summary>
        /// <param name="nvmlIndex">The ID of the target device</param>
        /// <param name="currentState">The current drain state for this GPU, see \ref nvmlEnableState</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if counters were successfully reset
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a nvmlIndex or \a currentState is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceQueryDrainState(uint nvmlIndex, ref nvmlEnableState currentState);


        /// <summary>
        /// This method will remove the specified GPU from the view of both NVML and the NVIDIA kernel driver
        /// as long as no other processes are attached. If other processes are attached, this call will return
        /// NVML_ERROR_IN_USE and the GPU will be returned to its original "draining" state. Note: the
        /// only situation where a process can still be attached after nvmlDeviceModifyDrainState() is called
        /// to initiate the draining state is if that process was using, and is still using, a GPU before the 
        /// call was made. Also note, persistence mode counts as an attachment to the GPU thus it must be disabled
        /// prior to this call.
        /// For long-running NVML processes please note that this will change the enumeration of current GPUs.
        /// For example, if there are four GPUs present and GPU1 is removed, the new enumeration will be 0-2.
        /// Also, device handles after the removed GPU will not be valid and must be re-established.
        /// Must be run as administrator. 
        /// For Linux only.
        /// For newer than Maxwell fully supported devices.
        /// Some Kepler devices supported.
        /// </summary>
        /// <param name="nvmlIndex">The ID of the target device</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if counters were successfully reset
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a nvmlIndex is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the device doesn't support this feature
        ///         - \ref NVML_ERROR_IN_USE            if the device is still in use and cannot be removed
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceRemoveGpu(uint nvmlIndex);


        /// <summary>
        /// Request the OS and the NVIDIA kernel driver to rediscover a portion of the PCI subsystem looking for GPUs that
        /// were previously removed. The portion of the PCI tree can be narrowed by specifying a domain, bus, and device.  
        /// If all are zeroes then the entire PCI tree will be searched.  Please note that for long-running NVML processes
        /// the enumeration will change based on how many GPUs are discovered and where they are inserted in bus order.
        /// In addition, all newly discovered GPUs will be initialized and their ECC scrubbed which may take several seconds
        /// per GPU. Also, all device handles are no longer guaranteed to be valid post discovery.
        /// Must be run as administrator.
        /// For Linux only.
        /// 
        /// For newer than Maxwell fully supported devices.
        /// Some Kepler devices supported.
        /// </summary>
        /// <param name="pciInfo">The PCI tree to be searched.  Only the domain, bus, and device</param>
        /// <returns>
        /// 
        ///         - \ref NVML_SUCCESS                 if counters were successfully reset
        ///         - \ref NVML_ERROR_UNINITIALIZED     if the library has not been successfully initialized
        ///         - \ref NVML_ERROR_INVALID_ARGUMENT  if \a pciInfo is invalid
        ///         - \ref NVML_ERROR_NOT_SUPPORTED     if the operating system does not support this feature
        ///         - \ref NVML_ERROR_OPERATING_SYSTEM  if the operating system is denying this feature
        ///         - \ref NVML_ERROR_NO_PERMISSION     if the calling process has insufficient permissions to perform operation
        ///         - \ref NVML_ERROR_UNKNOWN           on any unexpected error
        /// </returns>
        [DllImport(NVML_API_DLL_NAME)]
        internal static extern nvmlReturn nvmlDeviceDiscoverGpus(ref nvmlPciInfo spciInfo);
    }
}

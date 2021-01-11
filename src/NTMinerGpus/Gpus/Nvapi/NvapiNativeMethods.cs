using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NTMiner.Gpus.Nvapi {
    public static class NvapiNativeMethods {
        [AttributeUsage(AttributeTargets.Property)]
        public class IdAttribute : Attribute {
            public IdAttribute(uint id) {
                this.Id = id;
            }

            public uint Id { get; private set; }
        }

        private delegate IntPtr nvapi_QueryInterfaceDelegate(uint id);
        private delegate NvStatus NvInitializeDelegate();

        internal delegate NvStatus NvEnumPhysicalGPUsDelegate([Out] NvPhysicalGpuHandle[] physicalGpus, out int gpuCount);
        internal delegate NvStatus NvEnumTCCPhysicalGPUsDelegate([Out] NvPhysicalGpuHandle[] physicalGpus, out int gpuCount);
        internal delegate NvStatus NvGetBusIdDelegate(NvPhysicalGpuHandle physicalGpu, out int busID);

        internal delegate NvStatus NvPowerPoliciesGetStatusDelegate(NvPhysicalGpuHandle physicalGpu, ref NvGpuPowerStatus status);
        internal delegate NvStatus NvPowerPoliciesSetStatusDelegate(NvPhysicalGpuHandle physicalGpu, ref NvGpuPowerStatus status);

        internal delegate NvStatus NvGetPStateV2Delegate(NvPhysicalGpuHandle physicalGpu, ref NvGpuPerfPStates20InfoV2 pstate);
        internal delegate NvStatus NvSetPStateV2Delegate(NvPhysicalGpuHandle physicalGpu, ref NvGpuPerfPStates20InfoV2 pstate);

        internal delegate NvStatus NvThermalPoliciesGetInfoDelegate(NvPhysicalGpuHandle physicalGpu, ref NvGpuThermalInfo outThermalInfo);
        internal delegate NvStatus NvThermalPoliciesGetSetLimitDelegate(NvPhysicalGpuHandle physicalGpu, ref NvGpuThermalLimit outThermalLimit);

        internal delegate NvStatus NvPowerPoliciesGetInfoDelegate(NvPhysicalGpuHandle physicalGpu, ref NvGpuPowerInfo outPowerInfo);

        internal delegate NvStatus NvGetCoolerSettingsDelegate(NvPhysicalGpuHandle physicalGpu, NvCoolerTarget targetId, ref NvCoolerSettings outCoolerInfo);
        internal delegate NvStatus NvSetCoolerLevelsDelegate(NvPhysicalGpuHandle physicalGpu, NvCoolerTarget coolerIndex, ref NvCoolerLevel level);

        internal delegate NvStatus NvFanCoolersGetStatusDelegate(NvPhysicalGpuHandle physicalGpu, ref PrivateFanCoolersStatusV1 status);
        internal delegate NvStatus NvFanCoolersGetControlDelegate(NvPhysicalGpuHandle physicalGpu, ref PrivateFanCoolersControlV1 control);
        internal delegate NvStatus NvFanCoolersSetControlDelegate(NvPhysicalGpuHandle physicalGpu, ref PrivateFanCoolersControlV1 control);

        private static nvapi_QueryInterfaceDelegate nvapi_QueryInterface { get; set; }
        private static NvInitializeDelegate NvInitialize { get; set; }

        #region 
        // 以下属性要求必须是外部可见的static，不能是private的
        [Id(0xE5AC921F)]
        internal static NvEnumPhysicalGPUsDelegate NvEnumPhysicalGPUs { get; private set; }
        [Id(0xD9930B07)]
        internal static NvEnumTCCPhysicalGPUsDelegate NvEnumTCCPhysicalGPUs { get; private set; }
        [Id(0x1BE0B8E5)]
        internal static NvGetBusIdDelegate NvGetBusID { get; private set; }
        [Id(0x70916171)]
        internal static NvPowerPoliciesGetStatusDelegate NvPowerPoliciesGetStatus { get; private set; }
        [Id(0xAD95F5ED)]
        internal static NvPowerPoliciesSetStatusDelegate NvPowerPoliciesSetStatus { get; private set; }
        [Id(0x6FF81213)]
        internal static NvGetPStateV2Delegate NvGetPStateV2 { get; private set; }
        [Id(0x0F4DAE6B)]
        internal static NvSetPStateV2Delegate NvSetPStateV2 { get; private set; }
        [Id(0x0D258BB5)]
        internal static NvThermalPoliciesGetInfoDelegate NvThermalPoliciesGetInfo { get; private set; }
        [Id(0xE9C425A1)]
        internal static NvThermalPoliciesGetSetLimitDelegate NvThermalPoliciesGetLimit { get; private set; }
        [Id(0x34C0B13D)]
        internal static NvThermalPoliciesGetSetLimitDelegate NvThermalPoliciesSetLimit { get; private set; }
        [Id(0x34206D86)]
        internal static NvPowerPoliciesGetInfoDelegate NvPowerPoliciesGetInfo { get; private set; }
        [Id(0xDA141340)]
        internal static NvGetCoolerSettingsDelegate NvGetCoolerSettings { get; private set; }
        [Id(0x891FA0AE)]
        internal static NvSetCoolerLevelsDelegate NvSetCoolerLevels { get; private set; }
        [Id(0x35AED5E8)]
        internal static NvFanCoolersGetStatusDelegate NvFanCoolersGetStatus { get; private set; }
        [Id(0x814B209F)]
        internal static NvFanCoolersGetControlDelegate NvFanCoolersGetControl { get; private set; }
        [Id(0xA58971A5)]
        internal static NvFanCoolersSetControlDelegate NvFanCoolersSetControl { get; private set; }

        #endregion

        private static void SetDelegate(PropertyInfo property, uint id) {
            IntPtr ptr = nvapi_QueryInterface(id);
            if (ptr != IntPtr.Zero) {
                var newDelegate = Marshal.GetDelegateForFunctionPointer(ptr, property.PropertyType);
                property.SetValue(null, newDelegate, null);
            }
        }

        static NvapiNativeMethods() {
            DllImportAttribute attribute = new DllImportAttribute("nvapi64.dll") {
                CallingConvention = CallingConvention.Cdecl,
                PreserveSig = true,
                EntryPoint = "nvapi_QueryInterface"
            };
            PInvokeDelegateFactory.CreateDelegate(attribute, typeof(nvapi_QueryInterfaceDelegate), out object newDelegate);
            nvapi_QueryInterface = (nvapi_QueryInterfaceDelegate)newDelegate;

            try {
                IntPtr ptr = nvapi_QueryInterface(0x0150E828);
                NvInitialize = (NvInitializeDelegate)Marshal.GetDelegateForFunctionPointer(ptr, typeof(NvInitializeDelegate));
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }

            if (NvInitialize() == NvStatus.NVAPI_OK) {
                Type t = typeof(NvapiNativeMethods);
                var properties = t.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetProperty);
                foreach (var property in properties) {
                    var attrs = property.GetCustomAttributes(typeof(IdAttribute), inherit: false);
                    if (attrs.Length == 0) {
                        continue;
                    }
                    var id = ((IdAttribute)attrs[0]).Id;
                    SetDelegate(property, id);
                }
            }
        }
    }
}

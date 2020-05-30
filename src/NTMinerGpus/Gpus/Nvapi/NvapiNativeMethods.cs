using System;
using System.Runtime.InteropServices;

namespace NTMiner.Gpus.Nvapi {
    public static class NvapiNativeMethods {
        private delegate IntPtr NvQueryInterfaceDelegate(uint id);
        private delegate NvStatus NvInitializeDelegate();

        private static readonly NvQueryInterfaceDelegate NvQueryInterface;
        private static readonly NvInitializeDelegate NvInitialize;

        #region 
        internal static NvDelegates.NvEnumPhysicalGPUsDelegate NvEnumPhysicalGPUs { get; private set; }
        internal static NvDelegates.NvEnumTCCPhysicalGPUsDelegate NvEnumTCCPhysicalGPUs { get; private set; }
        internal static NvDelegates.NvGetBusIdDelegate NvGetBusID { get; private set; }
        internal static NvDelegates.NvGetTachReadingDelegate NvGetTachReading { get; private set; }
        internal static NvDelegates.NvGetPStatesDelegate NvGetPStates { get; private set; }

        internal static NvDelegates.NvPowerPoliciesGetStatusDelegate NvPowerPoliciesGetStatus { get; private set; }
        internal static NvDelegates.NvPowerPoliciesSetStatusDelegate NvPowerPoliciesSetStatus { get; private set; }

        internal static NvDelegates.NvGetPStateV1Delegate NvGetPStateV1 { get; private set; }
        internal static NvDelegates.NvGetPStateV2Delegate NvGetPStateV2 { get; private set; }
        internal static NvDelegates.NvSetPStateV1Delegate NvSetPStateV1 { get; private set; }
        internal static NvDelegates.NvSetPStateV2Delegate NvSetPStateV2 { get; private set; }
        internal static NvDelegates.NvGetAllClockFrequenciesV2Delegate NvGetAllClockFrequenciesV2 { get; private set; }

        internal static NvDelegates.NvThermalPoliciesGetInfoDelegate NvThermalPoliciesGetInfo { get; private set; }
        internal static NvDelegates.NvThermalPoliciesGetSetLimitDelegate NvThermalPoliciesGetLimit { get; private set; }
        internal static NvDelegates.NvThermalPoliciesGetSetLimitDelegate NvThermalPoliciesSetLimit { get; private set; }

        internal static NvDelegates.NvPowerPoliciesGetInfoDelegate NvPowerPoliciesGetInfo { get; private set; }

        internal static NvDelegates.NvGetCoolerSettingsDelegate NvGetCoolerSettings { get; private set; }
        internal static NvDelegates.NvSetCoolerLevelsDelegate NvSetCoolerLevels { get; private set; }
        internal static NvDelegates.NvRestoreCoolerSettingsDelegate NvRestoreCoolerSettings { get; private set; }

        internal static NvDelegates.NvFanCoolersGetInfoDelegate NvFanCoolersGetInfo { get; private set; }
        internal static NvDelegates.NvFanCoolersGetStatusDelegate NvFanCoolersGetStatus { get; private set; }
        internal static NvDelegates.NvFanCoolersGetControlDelegate NvFanCoolersGetControl { get; private set; }
        internal static NvDelegates.NvFanCoolersSetControlDelegate NvFanCoolersSetControl { get; private set; }

        #endregion

        private static T GetDelegate<T>(uint id)
            where T : class {
            IntPtr ptr = NvQueryInterface(id);
            if (ptr != IntPtr.Zero) {
                return Marshal.GetDelegateForFunctionPointer(ptr, typeof(T)) as T;
            }
            else {
                return null;
            }
        }

        static NvapiNativeMethods() {
            DllImportAttribute attribute = new DllImportAttribute("nvapi64.dll");
            attribute.CallingConvention = CallingConvention.Cdecl;
            attribute.PreserveSig = true;
            attribute.EntryPoint = "nvapi_QueryInterface";
            PInvokeDelegateFactory.CreateDelegate(attribute, out NvQueryInterface);

            try {
                NvInitialize = GetDelegate<NvInitializeDelegate>(0x0150E828);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }

            if (NvInitialize() == NvStatus.NVAPI_OK) {
                NvGetTachReading = GetDelegate<NvDelegates.NvGetTachReadingDelegate>(0x5F608315);
                NvGetPStates = GetDelegate<NvDelegates.NvGetPStatesDelegate>(0x60DED2ED);
                NvEnumPhysicalGPUs = GetDelegate<NvDelegates.NvEnumPhysicalGPUsDelegate>(0xE5AC921F);
                NvEnumTCCPhysicalGPUs = GetDelegate<NvDelegates.NvEnumTCCPhysicalGPUsDelegate>(0xD9930B07);
                NvGetBusID = GetDelegate<NvDelegates.NvGetBusIdDelegate>(0x1BE0B8E5);

                NvGetPStateV1 = GetDelegate<NvDelegates.NvGetPStateV1Delegate>(0x6FF81213);
                NvGetPStateV2 = GetDelegate<NvDelegates.NvGetPStateV2Delegate>(0x6FF81213);
                NvSetPStateV1 = GetDelegate<NvDelegates.NvSetPStateV1Delegate>(0x0F4DAE6B);
                NvSetPStateV2 = GetDelegate<NvDelegates.NvSetPStateV2Delegate>(0x0F4DAE6B);
                NvGetAllClockFrequenciesV2 = GetDelegate<NvDelegates.NvGetAllClockFrequenciesV2Delegate>(0xDCB616C3);

                NvThermalPoliciesGetInfo = GetDelegate<NvDelegates.NvThermalPoliciesGetInfoDelegate>(0x0D258BB5);
                NvThermalPoliciesGetLimit = GetDelegate<NvDelegates.NvThermalPoliciesGetSetLimitDelegate>(0xE9C425A1);
                NvThermalPoliciesSetLimit = GetDelegate<NvDelegates.NvThermalPoliciesGetSetLimitDelegate>(0x34C0B13D);

                NvPowerPoliciesGetStatus = GetDelegate<NvDelegates.NvPowerPoliciesGetStatusDelegate>(0x70916171);
                NvPowerPoliciesSetStatus = GetDelegate<NvDelegates.NvPowerPoliciesSetStatusDelegate>(0xAD95F5ED);
                NvPowerPoliciesGetInfo = GetDelegate<NvDelegates.NvPowerPoliciesGetInfoDelegate>(0x34206D86);

                NvGetCoolerSettings = GetDelegate<NvDelegates.NvGetCoolerSettingsDelegate>(0xDA141340);
                NvSetCoolerLevels = GetDelegate<NvDelegates.NvSetCoolerLevelsDelegate>(0x891FA0AE);
                NvRestoreCoolerSettings = GetDelegate<NvDelegates.NvRestoreCoolerSettingsDelegate>(0x8F6ED0FB);

                NvFanCoolersGetInfo = GetDelegate<NvDelegates.NvFanCoolersGetInfoDelegate>(0xFB85B01E);
                NvFanCoolersGetStatus = GetDelegate<NvDelegates.NvFanCoolersGetStatusDelegate>(0x35AED5E8);
                NvFanCoolersGetControl = GetDelegate<NvDelegates.NvFanCoolersGetControlDelegate>(0x814B209F);
                NvFanCoolersSetControl = GetDelegate<NvDelegates.NvFanCoolersSetControlDelegate>(0xA58971A5);
            }
        }
    }
}

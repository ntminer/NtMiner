using System;
using System.Runtime.InteropServices;

namespace NTMiner.Gpus.Nvapi {
    public static class NvapiNativeMethods {
        #region 
        private static readonly NvDelegates.NvQueryInterfaceDelegate NvQueryInterface;
        private static readonly NvDelegates.NvInitializeDelegate NvInitialize;

        internal static readonly NvDelegates.NvEnumPhysicalGPUsDelegate NvEnumPhysicalGPUs;
        internal static readonly NvDelegates.NvEnumTCCPhysicalGPUsDelegate NvEnumTCCPhysicalGPUs;
        internal static readonly NvDelegates.NvGetBusIdDelegate NvGetBusID;
        internal static readonly NvDelegates.NvGetTachReadingDelegate NvGetTachReading;
        internal static readonly NvDelegates.NvGetPStatesDelegate NvGetPStates;

        internal static readonly NvDelegates.NvPowerPoliciesGetStatusDelegate NvPowerPoliciesGetStatus;
        internal static readonly NvDelegates.NvPowerPoliciesSetStatusDelegate NvPowerPoliciesSetStatus;

        internal static readonly NvDelegates.NvGetPStateV1Delegate NvGetPStateV1;
        internal static readonly NvDelegates.NvGetPStateV2Delegate NvGetPStateV2;
        internal static readonly NvDelegates.NvSetPStateV1Delegate NvSetPStateV1;
        internal static readonly NvDelegates.NvSetPStateV2Delegate NvSetPStateV2;
        internal static readonly NvDelegates.NvGetAllClockFrequenciesV2Delegate NvGetAllClockFrequenciesV2;

        internal static readonly NvDelegates.NvThermalPoliciesGetInfoDelegate NvThermalPoliciesGetInfo;
        internal static readonly NvDelegates.NvThermalPoliciesGetSetLimitDelegate NvThermalPoliciesGetLimit;
        internal static readonly NvDelegates.NvThermalPoliciesGetSetLimitDelegate NvThermalPoliciesSetLimit;

        internal static readonly NvDelegates.NvPowerPoliciesGetInfoDelegate NvPowerPoliciesGetInfo;

        internal static readonly NvDelegates.NvGetCoolerSettingsDelegate NvGetCoolerSettings;
        internal static readonly NvDelegates.NvSetCoolerLevelsDelegate NvSetCoolerLevels;
        internal static readonly NvDelegates.NvRestoreCoolerSettingsDelegate NvRestoreCoolerSettings;

        internal static readonly NvDelegates.NvFanCoolersGetInfoDelegate NvFanCoolersGetInfo;
        internal static readonly NvDelegates.NvFanCoolersGetStatusDelegate NvFanCoolersGetStatus;
        internal static readonly NvDelegates.NvFanCoolersGetControlDelegate NvFanCoolersGetControl;
        internal static readonly NvDelegates.NvFanCoolersSetControlDelegate NvFanCoolersSetControl;

        #endregion

        private static void GetDelegate<T>(uint id, out T newDelegate)
            where T : class {
            IntPtr ptr = NvQueryInterface(id);
            if (ptr != IntPtr.Zero) {
                newDelegate = Marshal.GetDelegateForFunctionPointer(ptr, typeof(T)) as T;
            }
            else {
                newDelegate = null;
            }
        }

        static NvapiNativeMethods() {
            DllImportAttribute attribute = new DllImportAttribute("nvapi64.dll");
            attribute.CallingConvention = CallingConvention.Cdecl;
            attribute.PreserveSig = true;
            attribute.EntryPoint = "nvapi_QueryInterface";
            PInvokeDelegateFactory.CreateDelegate(attribute, out NvQueryInterface);

            try {
                GetDelegate(0x0150E828, out NvInitialize);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }

            if (NvInitialize() == NvStatus.NVAPI_OK) {
                GetDelegate(0x5F608315, out NvGetTachReading);
                GetDelegate(0x60DED2ED, out NvGetPStates);
                GetDelegate(0xE5AC921F, out NvEnumPhysicalGPUs);
                GetDelegate(0xD9930B07, out NvEnumTCCPhysicalGPUs);
                GetDelegate(0x1BE0B8E5, out NvGetBusID);

                GetDelegate(0x6FF81213, out NvGetPStateV1);
                GetDelegate(0x6FF81213, out NvGetPStateV2);
                GetDelegate(0x0F4DAE6B, out NvSetPStateV1);
                GetDelegate(0x0F4DAE6B, out NvSetPStateV2);
                GetDelegate(0xDCB616C3, out NvGetAllClockFrequenciesV2);

                GetDelegate(0x0D258BB5, out NvThermalPoliciesGetInfo);
                GetDelegate(0xE9C425A1, out NvThermalPoliciesGetLimit);
                GetDelegate(0x34C0B13D, out NvThermalPoliciesSetLimit);

                GetDelegate(0x70916171, out NvPowerPoliciesGetStatus);
                GetDelegate(0xAD95F5ED, out NvPowerPoliciesSetStatus);
                GetDelegate(0x34206D86, out NvPowerPoliciesGetInfo);

                GetDelegate(0xDA141340, out NvGetCoolerSettings);
                GetDelegate(0x891FA0AE, out NvSetCoolerLevels);
                GetDelegate(0x8F6ED0FB, out NvRestoreCoolerSettings);

                GetDelegate(0xFB85B01E, out NvFanCoolersGetInfo);
                GetDelegate(0x35AED5E8, out NvFanCoolersGetStatus);
                GetDelegate(0x814B209F, out NvFanCoolersGetControl);
                GetDelegate(0xA58971A5, out NvFanCoolersSetControl);
            }
        }
    }
}

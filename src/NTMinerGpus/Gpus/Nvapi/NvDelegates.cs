using System;
using System.Runtime.InteropServices;

namespace NTMiner.Gpus.Nvapi {
    public static class NvDelegates {
        internal delegate IntPtr NvQueryInterfaceDelegate(uint id);
        internal delegate NvStatus NvInitializeDelegate();

        internal delegate NvStatus NvEnumPhysicalGPUsDelegate([Out] NvPhysicalGpuHandle[] physicalGpus, out int gpuCount);
        internal delegate NvStatus NvEnumTCCPhysicalGPUsDelegate([Out] NvPhysicalGpuHandle[] physicalGpus, out int gpuCount);
        internal delegate NvStatus NvGetBusIdDelegate(NvPhysicalGpuHandle physicalGpu, out int busID);
        internal delegate NvStatus NvGetTachReadingDelegate(NvPhysicalGpuHandle physicalGpu, out int value);
        internal delegate NvStatus NvGetPStatesDelegate(NvPhysicalGpuHandle physicalGpu, ref NvPStates nvPStates);

        internal delegate NvStatus NvPowerPoliciesGetStatusDelegate(NvPhysicalGpuHandle physicalGpu, ref NvGpuPowerStatus status);
        internal delegate NvStatus NvPowerPoliciesSetStatusDelegate(NvPhysicalGpuHandle physicalGpu, ref NvGpuPowerStatus status);

        internal delegate NvStatus NvGetPStateV1Delegate(NvPhysicalGpuHandle physicalGpu, ref NvGpuPerfPStates20InfoV1 pstate);
        internal delegate NvStatus NvGetPStateV2Delegate(NvPhysicalGpuHandle physicalGpu, ref NvGpuPerfPStates20InfoV2 pstate);
        internal delegate NvStatus NvSetPStateV1Delegate(NvPhysicalGpuHandle physicalGpu, ref NvGpuPerfPStates20InfoV1 pstate);
        internal delegate NvStatus NvSetPStateV2Delegate(NvPhysicalGpuHandle physicalGpu, ref NvGpuPerfPStates20InfoV2 pstate);
        internal delegate NvStatus NvGetAllClockFrequenciesV2Delegate(NvPhysicalGpuHandle physicalGpu, ref NvGpuClockFrequenciesV2 freq);

        internal delegate NvStatus NvThermalPoliciesGetInfoDelegate(NvPhysicalGpuHandle physicalGpu, ref NvGpuThermalInfo outThermalInfo);
        internal delegate NvStatus NvThermalPoliciesGetSetLimitDelegate(NvPhysicalGpuHandle physicalGpu, ref NvGpuThermalLimit outThermalLimit);

        internal delegate NvStatus NvPowerPoliciesGetInfoDelegate(NvPhysicalGpuHandle physicalGpu, ref NvGpuPowerInfo outPowerInfo);

        internal delegate NvStatus NvGetCoolerSettingsDelegate(NvPhysicalGpuHandle physicalGpu, NvCoolerTarget targetId, ref NvCoolerSettings outCoolerInfo);
        internal delegate NvStatus NvRestoreCoolerSettingsDelegate(NvPhysicalGpuHandle physicalGpu, IntPtr pCoolerIndex, NvCoolerTarget targetId);
        internal delegate NvStatus NvSetCoolerLevelsDelegate(NvPhysicalGpuHandle physicalGpu, NvCoolerTarget coolerIndex, ref NvCoolerLevel level);

        internal delegate NvStatus NvFanCoolersGetInfoDelegate(NvPhysicalGpuHandle physicalGpu, ref PrivateFanCoolersInfoV1 info);
        internal delegate NvStatus NvFanCoolersGetStatusDelegate(NvPhysicalGpuHandle physicalGpu, ref PrivateFanCoolersStatusV1 status);
        internal delegate NvStatus NvFanCoolersGetControlDelegate(NvPhysicalGpuHandle physicalGpu, ref PrivateFanCoolersControlV1 control);
        internal delegate NvStatus NvFanCoolersSetControlDelegate(NvPhysicalGpuHandle physicalGpu, ref PrivateFanCoolersControlV1 control);
    }
}

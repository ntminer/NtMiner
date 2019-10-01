namespace HardwareProviders.CPU.Ryzen {
    static class RyzenCostants {
        public const uint PERF_CTL_0 = 0xC0010000;
        public const uint PERF_CTR_0 = 0xC0010004;
        public const uint HWCR = 0xC0010015;

        public const uint MSR_PSTATE_L = 0xC0010061;
        public const uint MSR_PSTATE_C = 0xC0010062;
        public const uint MSR_PSTATE_S = 0xC0010063;
        public const uint MSR_PSTATE_0 = 0xC0010064;

        public const uint MSR_PWR_UNIT = 0xC0010299;
        public const uint MSR_CORE_ENERGY_STAT = 0xC001029A;
        public const uint MSR_PKG_ENERGY_STAT = 0xC001029B;
        public const uint MSR_HARDWARE_PSTATE_STATUS = 0xC0010293;
        public const uint COFVID_STATUS = 0xC0010071;
        public const uint FAMILY_17H_PCI_CONTROL_REGISTER = 0x60;
        public const uint FAMILY_17H_MODEL_01_MISC_CONTROL_DEVICE_ID = 0x1463;
        public const uint F17H_M01H_THM_TCON_CUR_TMP = 0x00059800;
        public const uint F17H_M01H_SVI = 0x0005A000;

        public const uint F17H_TEMP_OFFSET_FLAG = 0x80000;
    }
}

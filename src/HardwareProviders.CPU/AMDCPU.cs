namespace HardwareProviders.CPU {
    public abstract class AmdCpu : Cpu {
        private const byte PCI_BUS = 0;
        private const byte PCI_BASE_DEVICE = 0x18;
        private const byte DEVICE_VENDOR_ID_REGISTER = 0;
        private const ushort AMD_VENDOR_ID = 0x1022;

        protected AmdCpu(int processorIndex, CpuId[][] cpuId)
            : base(processorIndex, cpuId) {
        }

        protected uint GetPciAddress(byte function, ushort deviceId) {
            // assemble the pci address
            var address = Ring0.GetPciAddress(PCI_BUS,
                (byte)(PCI_BASE_DEVICE + processorIndex), function);

            // verify that we have the correct bus, device and function
            if (!Ring0.ReadPciConfig(
                address, DEVICE_VENDOR_ID_REGISTER, out var deviceVendor))
                return Ring0.InvalidPciAddress;

            return deviceVendor != ((deviceId << 16) | AMD_VENDOR_ID) ? Ring0.InvalidPciAddress : address;
        }
    }
}
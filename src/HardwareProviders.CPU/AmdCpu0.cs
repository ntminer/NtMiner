using System.Threading;

namespace HardwareProviders.CPU {
    public sealed class AmdCpu0 : AmdCpu {
        private const uint FIDVID_STATUS = 0xC0010042;

        private const byte MISCELLANEOUS_CONTROL_FUNCTION = 3;
        private const ushort MISCELLANEOUS_CONTROL_DEVICE_ID = 0x1103;
        private const uint THERMTRIP_STATUS_REGISTER = 0xE4;

        private readonly uint miscellaneousControlAddress;

        private readonly byte thermSenseCoreSelCPU0;
        private readonly byte thermSenseCoreSelCPU1;

        public AmdCpu0(int processorIndex, CpuId[][] cpuId)
            : base(processorIndex, cpuId) {
            var offset = -49.0f;

            // AM2+ 65nm +21 offset
            var model = cpuId[0][0].Model;
            if (model >= 0x69 && model != 0xc1 && model != 0x6c && model != 0x7c)
                offset += 21;

            if (model < 40) {
                // AMD Athlon 64 Processors
                thermSenseCoreSelCPU0 = 0x0;
                thermSenseCoreSelCPU1 = 0x4;
            }
            else {
                // AMD NPT Family 0Fh Revision F, G have the core selection swapped
                thermSenseCoreSelCPU0 = 0x4;
                thermSenseCoreSelCPU1 = 0x0;
            }

            // check if processor supports a digital thermal sensor 
            if (cpuId[0][0].ExtData.GetLength(0) > 7 &&
                (cpuId[0][0].ExtData[7, 3] & 1) != 0) {
                CoreTemperatures = new Sensor[CoreCount];
                for (var i = 0; i < CoreCount; i++)
                    CoreTemperatures[i] =
                        new Sensor("Core #" + (i + 1), i, SensorType.Temperature,
                            this, new[]
                            {
                                new Parameter("Offset [°C]",
                                    "Temperature offset of the thermal sensor.\n" +
                                    "Temperature = Value + Offset.", offset)
                            });
            }
            else {
                CoreTemperatures = new Sensor[0];
            }

            miscellaneousControlAddress = GetPciAddress(
                MISCELLANEOUS_CONTROL_FUNCTION, MISCELLANEOUS_CONTROL_DEVICE_ID);

            BusClock = new Sensor("Bus Speed", 0, SensorType.Clock, this);
            CoreClocks = new Sensor[CoreCount];
            for (var i = 0; i < CoreClocks.Length; i++) {
                CoreClocks[i] = new Sensor(CoreString(i), i + 1, SensorType.Clock,
                    this);
                if (HasTimeStampCounter)
                    ActivateSensor(CoreClocks[i]);
            }

            Update();
        }

        protected override uint[] GetMSRs() {
            return new[] { FIDVID_STATUS };
        }

        public override void Update() {
            base.Update();

            if (miscellaneousControlAddress != Ring0.InvalidPciAddress)
                for (uint i = 0; i < CoreTemperatures.Length; i++)
                    if (Ring0.WritePciConfig(
                        miscellaneousControlAddress, THERMTRIP_STATUS_REGISTER,
                        i > 0 ? thermSenseCoreSelCPU1 : thermSenseCoreSelCPU0)) {
                        uint value;
                        if (Ring0.ReadPciConfig(
                            miscellaneousControlAddress, THERMTRIP_STATUS_REGISTER,
                            out value)) {
                            CoreTemperatures[i].Value = ((value >> 16) & 0xFF) +
                                                        CoreTemperatures[i].Parameters[0].Value;
                            ActivateSensor(CoreTemperatures[i]);
                        }
                        else {
                            DeactivateSensor(CoreTemperatures[i]);
                        }
                    }

            if (HasTimeStampCounter) {
                double newBusClock = 0;

                for (var i = 0; i < CoreClocks.Length; i++) {
                    Thread.Sleep(1);

                    if (Ring0.RdmsrTx(FIDVID_STATUS, out var eax, out _,
                        1UL << CpuId[i][0].Thread)) {
                        // CurrFID can be found in eax bits 0-5, MaxFID in 16-21
                        // 8-13 hold StartFID, we don't use that here.
                        var curMP = 0.5 * ((eax & 0x3F) + 8);
                        var maxMP = 0.5 * (((eax >> 16) & 0x3F) + 8);
                        CoreClocks[i].Value =
                            (float)(curMP * TimeStampCounterFrequency / maxMP);
                        newBusClock = (float)(TimeStampCounterFrequency / maxMP);
                    }
                    else {
                        // Fail-safe value - if the code above fails, we'll use this instead
                        CoreClocks[i].Value = (float)TimeStampCounterFrequency;
                    }
                }

                if (newBusClock > 0) {
                    BusClock.Value = (float)newBusClock;
                    ActivateSensor(BusClock);
                }
            }
        }
    }
}
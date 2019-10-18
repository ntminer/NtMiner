using System;
using System.Threading;

namespace HardwareProviders.CPU {
    public sealed class IntelCpu : Cpu {
        private const uint IA32_THERM_STATUS_MSR = 0x019C;
        private const uint IA32_TEMPERATURE_TARGET = 0x01A2;
        private const uint IA32_PERF_STATUS = 0x0198;
        private const uint MSR_PLATFORM_INFO = 0xCE;
        private const uint IA32_PACKAGE_THERM_STATUS = 0x1B1;
        private const uint MSR_RAPL_POWER_UNIT = 0x606;
        private const uint MSR_PKG_ENERY_STATUS = 0x611;
        private const uint MSR_DRAM_ENERGY_STATUS = 0x619;
        private const uint MSR_PP0_ENERY_STATUS = 0x639;
        private const uint MSR_PP1_ENERY_STATUS = 0x641;

        public IntelMicroarchitecture IntelMicroarchitecture { get; }

        private readonly uint[] energyStatusMSRs =
        {
            MSR_PKG_ENERY_STATUS,
            MSR_PP0_ENERY_STATUS, MSR_PP1_ENERY_STATUS, MSR_DRAM_ENERGY_STATUS
        };

        private readonly float energyUnitMultiplier;
        private readonly uint[] lastEnergyConsumed;
        private readonly DateTime[] lastEnergyTime;

        private readonly string[] powerSensorLabels =
            {"CPU Package", "CPU Cores", "CPU Graphics", "CPU DRAM"};


        private readonly double timeStampCounterMultiplier;

        public IntelCpu(int processorIndex, CpuId[][] cpuId)
            : base(processorIndex, cpuId) {
            // set tjMax
            float[] tjMax;
            switch (family) {
                case 0x06: {
                        switch (model) {
                            case 0x0F: // Intel Core 2 (65nm)
                                IntelMicroarchitecture = IntelMicroarchitecture.Core;
                                switch (stepping) {
                                    case 0x06: // B2
                                        tjMax = Floats(80 + 10);
                                        break;
                                    case 0x0B: // G0
                                        tjMax = Floats(90 + 10);
                                        break;
                                    case 0x0D: // M0
                                        tjMax = Floats(85 + 10);
                                        break;
                                    default:
                                        tjMax = Floats(85 + 10);
                                        break;
                                }

                                break;
                            case 0x17: // Intel Core 2 (45nm)
                                IntelMicroarchitecture = IntelMicroarchitecture.Core;
                                tjMax = Floats(100);
                                break;
                            case 0x1C: // Intel Atom (45nm)
                                IntelMicroarchitecture = IntelMicroarchitecture.Atom;
                                switch (stepping) {
                                    case 0x02: // C0
                                        tjMax = Floats(90);
                                        break;
                                    case 0x0A: // A0, B0
                                        tjMax = Floats(100);
                                        break;
                                    default:
                                        tjMax = Floats(90);
                                        break;
                                }

                                break;
                            case 0x1A: // Intel Core i7 LGA1366 (45nm)
                            case 0x1E: // Intel Core i5, i7 LGA1156 (45nm)
                            case 0x1F: // Intel Core i5, i7 
                            case 0x25: // Intel Core i3, i5, i7 LGA1156 (32nm)
                            case 0x2C: // Intel Core i7 LGA1366 (32nm) 6 Core
                            case 0x2E: // Intel Xeon Processor 7500 series (45nm)
                            case 0x2F: // Intel Xeon Processor (32nm)
                                IntelMicroarchitecture = IntelMicroarchitecture.Nehalem;
                                tjMax = GetTjMaxFromMSR();
                                break;
                            case 0x2A: // Intel Core i5, i7 2xxx LGA1155 (32nm)
                            case 0x2D: // Next Generation Intel Xeon, i7 3xxx LGA2011 (32nm)
                                IntelMicroarchitecture = IntelMicroarchitecture.SandyBridge;
                                tjMax = GetTjMaxFromMSR();
                                break;
                            case 0x3A: // Intel Core i5, i7 3xxx LGA1155 (22nm)
                            case 0x3E: // Intel Core i7 4xxx LGA2011 (22nm)
                                IntelMicroarchitecture = IntelMicroarchitecture.IvyBridge;
                                tjMax = GetTjMaxFromMSR();
                                break;
                            case 0x3C: // Intel Core i5, i7 4xxx LGA1150 (22nm)              
                            case 0x3F: // Intel Xeon E5-2600/1600 v3, Core i7-59xx
                                       // LGA2011-v3, Haswell-E (22nm)
                            case 0x45: // Intel Core i5, i7 4xxxU (22nm)
                            case 0x46:
                                IntelMicroarchitecture = IntelMicroarchitecture.Haswell;
                                tjMax = GetTjMaxFromMSR();
                                break;
                            case 0x3D: // Intel Core M-5xxx (14nm)
                            case 0x47: // Intel i5, i7 5xxx, Xeon E3-1200 v4 (14nm)
                            case 0x4F: // Intel Xeon E5-26xx v4
                            case 0x56: // Intel Xeon D-15xx
                                IntelMicroarchitecture = IntelMicroarchitecture.Broadwell;
                                tjMax = GetTjMaxFromMSR();
                                break;
                            case 0x36: // Intel Atom S1xxx, D2xxx, N2xxx (32nm)
                                IntelMicroarchitecture = IntelMicroarchitecture.Atom;
                                tjMax = GetTjMaxFromMSR();
                                break;
                            case 0x37: // Intel Atom E3xxx, Z3xxx (22nm)
                            case 0x4A:
                            case 0x4D: // Intel Atom C2xxx (22nm)
                            case 0x5A:
                            case 0x5D:
                                IntelMicroarchitecture = IntelMicroarchitecture.Silvermont;
                                tjMax = GetTjMaxFromMSR();
                                break;
                            case 0x4E:
                            case 0x5E: // Intel Core i5, i7 6xxxx LGA1151 (14nm)
                            case 0x55: // Intel Core X i7, i9 7xxx LGA2066 (14nm)
                                IntelMicroarchitecture = IntelMicroarchitecture.Skylake;
                                tjMax = GetTjMaxFromMSR();
                                break;
                            case 0x4C:
                                IntelMicroarchitecture = IntelMicroarchitecture.Airmont;
                                tjMax = GetTjMaxFromMSR();
                                break;
                            case 0x8E:
                            case 0x9E: // Intel Core i5, i7 7xxxx (14nm)
                                IntelMicroarchitecture = IntelMicroarchitecture.KabyLake;
                                tjMax = GetTjMaxFromMSR();
                                break;
                            case 0x5C: // Intel ApolloLake
                                IntelMicroarchitecture = IntelMicroarchitecture.ApolloLake;
                                tjMax = GetTjMaxFromMSR();
                                break;
                            case 0xAE: // Intel Core i5, i7 8xxxx (14nm++)
                                IntelMicroarchitecture = IntelMicroarchitecture.CoffeeLake;
                                tjMax = GetTjMaxFromMSR();
                                break;
                            default:
                                IntelMicroarchitecture = IntelMicroarchitecture.Unknown;
                                tjMax = Floats(100);
                                break;
                        }
                    }
                    break;
                case 0x0F: {
                        switch (model) {
                            case 0x00: // Pentium 4 (180nm)
                            case 0x01: // Pentium 4 (130nm)
                            case 0x02: // Pentium 4 (130nm)
                            case 0x03: // Pentium 4, Celeron D (90nm)
                            case 0x04: // Pentium 4, Pentium D, Celeron D (90nm)
                            case 0x06: // Pentium 4, Pentium D, Celeron D (65nm)
                                IntelMicroarchitecture = IntelMicroarchitecture.NetBurst;
                                tjMax = Floats(100);
                                break;
                            default:
                                IntelMicroarchitecture = IntelMicroarchitecture.Unknown;
                                tjMax = Floats(100);
                                break;
                        }
                    }
                    break;
                default:
                    IntelMicroarchitecture = IntelMicroarchitecture.Unknown;
                    tjMax = Floats(100);
                    break;
            }

            // set timeStampCounterMultiplier
            switch (IntelMicroarchitecture) {
                case IntelMicroarchitecture.NetBurst:
                case IntelMicroarchitecture.Atom:
                case IntelMicroarchitecture.Core: {
                        if (Ring0.Rdmsr(IA32_PERF_STATUS, out _, out var edx))
                            timeStampCounterMultiplier =
                                ((edx >> 8) & 0x1f) + 0.5 * ((edx >> 14) & 1);
                    }
                    break;
                case IntelMicroarchitecture.Nehalem:
                case IntelMicroarchitecture.SandyBridge:
                case IntelMicroarchitecture.IvyBridge:
                case IntelMicroarchitecture.Haswell:
                case IntelMicroarchitecture.Broadwell:
                case IntelMicroarchitecture.Silvermont:
                case IntelMicroarchitecture.Skylake:
                case IntelMicroarchitecture.Airmont:
                case IntelMicroarchitecture.ApolloLake:
                case IntelMicroarchitecture.KabyLake:
                case IntelMicroarchitecture.CoffeeLake: {
                        if (Ring0.Rdmsr(MSR_PLATFORM_INFO, out var eax, out _))
                            timeStampCounterMultiplier = (eax >> 8) & 0xff;
                    }
                    break;
                default:
                    timeStampCounterMultiplier = 0;
                    break;
            }

            // check if processor supports a digital thermal sensor at core level
            if (cpuId[0][0].Data.GetLength(0) > 6 &&
                (cpuId[0][0].Data[6, 0] & 1) != 0 &&
                IntelMicroarchitecture != IntelMicroarchitecture.Unknown) {
                CoreTemperatures = new Sensor[CoreCount];
                for (var i = 0; i < CoreTemperatures.Length; i++) {
                    CoreTemperatures[i] = new Sensor(CoreString(i), i,
                        SensorType.Temperature, this, new[]
                        {
                            new Parameter(
                                "TjMax [°C]", "TjMax temperature of the core sensor.\n" +
                                              "Temperature = TjMax - TSlope * Value.", tjMax[i]),
                            new Parameter("TSlope [°C]",
                                "Temperature slope of the digital thermal sensor.\n" +
                                "Temperature = TjMax - TSlope * Value.", 1)
                        });
                    ActivateSensor(CoreTemperatures[i]);
                }
            }
            else {
                CoreTemperatures = new Sensor[0];
            }

            // check if processor supports a digital thermal sensor at package level
            if (cpuId[0][0].Data.GetLength(0) > 6 &&
                (cpuId[0][0].Data[6, 0] & 0x40) != 0 &&
                IntelMicroarchitecture != IntelMicroarchitecture.Unknown) {
                PackageTemperature = new Sensor("CPU Package",
                    CoreTemperatures.Length, SensorType.Temperature, this, new[]
                    {
                        new Parameter(
                            "TjMax [°C]", "TjMax temperature of the package sensor.\n" +
                                          "Temperature = TjMax - TSlope * Value.", tjMax[0]),
                        new Parameter("TSlope [°C]",
                            "Temperature slope of the digital thermal sensor.\n" +
                            "Temperature = TjMax - TSlope * Value.", 1)
                    });
                ActivateSensor(PackageTemperature);
            }

            BusClock = new Sensor("Bus Speed", 0, SensorType.Clock, this);
            CoreClocks = new Sensor[CoreCount];
            for (var i = 0; i < CoreClocks.Length; i++) {
                CoreClocks[i] =
                    new Sensor(CoreString(i), i + 1, SensorType.Clock, this);
                if (HasTimeStampCounter && IntelMicroarchitecture != IntelMicroarchitecture.Unknown)
                    ActivateSensor(CoreClocks[i]);
            }

            if (IntelMicroarchitecture == IntelMicroarchitecture.SandyBridge ||
                IntelMicroarchitecture == IntelMicroarchitecture.IvyBridge ||
                IntelMicroarchitecture == IntelMicroarchitecture.Haswell ||
                IntelMicroarchitecture == IntelMicroarchitecture.Broadwell ||
                IntelMicroarchitecture == IntelMicroarchitecture.Skylake ||
                IntelMicroarchitecture == IntelMicroarchitecture.Silvermont ||
                IntelMicroarchitecture == IntelMicroarchitecture.Airmont ||
                IntelMicroarchitecture == IntelMicroarchitecture.KabyLake ||
                IntelMicroarchitecture == IntelMicroarchitecture.ApolloLake) {
                CorePowers = new Sensor[energyStatusMSRs.Length];
                lastEnergyTime = new DateTime[energyStatusMSRs.Length];
                lastEnergyConsumed = new uint[energyStatusMSRs.Length];

                if (Ring0.Rdmsr(MSR_RAPL_POWER_UNIT, out var eax, out _))
                    switch (IntelMicroarchitecture) {
                        case IntelMicroarchitecture.Silvermont:
                        case IntelMicroarchitecture.Airmont:
                            energyUnitMultiplier = 1.0e-6f * (1 << (int)((eax >> 8) & 0x1F));
                            break;
                        default:
                            energyUnitMultiplier = 1.0f / (1 << (int)((eax >> 8) & 0x1F));
                            break;
                    }
                if (energyUnitMultiplier != 0)
                    for (var i = 0; i < energyStatusMSRs.Length; i++) {
                        if (!Ring0.Rdmsr(energyStatusMSRs[i], out eax, out _))
                            continue;

                        lastEnergyTime[i] = DateTime.UtcNow;
                        lastEnergyConsumed[i] = eax;
                        CorePowers[i] = new Sensor(powerSensorLabels[i], i,
                            SensorType.Power, this);
                        ActivateSensor(CorePowers[i]);
                    }
            }

            Update();
        }


        private float[] Floats(float f) {
            var result = new float[CoreCount];
            for (var i = 0; i < CoreCount; i++)
                result[i] = f;
            return result;
        }

        private float[] GetTjMaxFromMSR() {
            var result = new float[CoreCount];
            for (var i = 0; i < CoreCount; i++)
                if (Ring0.RdmsrTx(IA32_TEMPERATURE_TARGET, out var eax,
                    out _, 1UL << CpuId[i][0].Thread))
                    result[i] = (eax >> 16) & 0xFF;
                else
                    result[i] = 100;
            return result;
        }

        protected override uint[] GetMSRs() {
            return new[]
            {
                MSR_PLATFORM_INFO,
                IA32_PERF_STATUS,
                IA32_THERM_STATUS_MSR,
                IA32_TEMPERATURE_TARGET,
                IA32_PACKAGE_THERM_STATUS,
                MSR_RAPL_POWER_UNIT,
                MSR_PKG_ENERY_STATUS,
                MSR_DRAM_ENERGY_STATUS,
                MSR_PP0_ENERY_STATUS,
                MSR_PP1_ENERY_STATUS
            };
        }

        public override void Update() {
            base.Update();

            for (var i = 0; i < CoreTemperatures.Length; i++) {
                // if reading is valid
                if (Ring0.RdmsrTx(IA32_THERM_STATUS_MSR, out var eax, out _,
                        1UL << CpuId[i][0].Thread) && (eax & 0x80000000) != 0) {
                    // get the dist from tjMax from bits 22:16
                    float deltaT = (eax & 0x007F0000) >> 16;
                    var tjMax = CoreTemperatures[i].Parameters[0].Value;
                    var tSlope = CoreTemperatures[i].Parameters[1].Value;
                    CoreTemperatures[i].Value = tjMax - tSlope * deltaT;
                }
                else {
                    CoreTemperatures[i].Value = null;
                }
            }

            if (PackageTemperature != null) {
                // if reading is valid
                if (Ring0.RdmsrTx(IA32_PACKAGE_THERM_STATUS, out var eax, out _,
                        1UL << CpuId[0][0].Thread) && (eax & 0x80000000) != 0) {
                    // get the dist from tjMax from bits 22:16
                    float deltaT = (eax & 0x007F0000) >> 16;
                    var tjMax = PackageTemperature.Parameters[0].Value;
                    var tSlope = PackageTemperature.Parameters[1].Value;
                    PackageTemperature.Value = tjMax - tSlope * deltaT;
                }
                else {
                    PackageTemperature.Value = null;
                }
            }

            if (HasTimeStampCounter && timeStampCounterMultiplier > 0) {
                double newBusClock = 0;
                for (var i = 0; i < CoreClocks.Length; i++) {
                    Thread.Sleep(1);
                    if (Ring0.RdmsrTx(IA32_PERF_STATUS, out var eax, out _,
                        1UL << CpuId[i][0].Thread)) {
                        newBusClock =
                            TimeStampCounterFrequency / timeStampCounterMultiplier;
                        switch (IntelMicroarchitecture) {
                            case IntelMicroarchitecture.Nehalem: {
                                    var multiplier = eax & 0xff;
                                    CoreClocks[i].Value = (float)(multiplier * newBusClock);
                                }
                                break;
                            case IntelMicroarchitecture.SandyBridge:
                            case IntelMicroarchitecture.IvyBridge:
                            case IntelMicroarchitecture.Haswell:
                            case IntelMicroarchitecture.Broadwell:
                            case IntelMicroarchitecture.Silvermont:
                            case IntelMicroarchitecture.Skylake:
                            case IntelMicroarchitecture.ApolloLake:
                            case IntelMicroarchitecture.KabyLake:
                            case IntelMicroarchitecture.CoffeeLake: {
                                    var multiplier = (eax >> 8) & 0xff;
                                    CoreClocks[i].Value = (float)(multiplier * newBusClock);
                                }
                                break;
                            default: {
                                    var multiplier =
                                        ((eax >> 8) & 0x1f) + 0.5 * ((eax >> 14) & 1);
                                    CoreClocks[i].Value = (float)(multiplier * newBusClock);
                                }
                                break;
                        }
                    }
                    else {
                        // if IA32_PERF_STATUS is not available, assume TSC frequency
                        CoreClocks[i].Value = (float)TimeStampCounterFrequency;
                    }
                }

                if (newBusClock > 0) {
                    BusClock.Value = (float)newBusClock;
                    ActivateSensor(BusClock);
                }
            }

            if (CorePowers != null)
                foreach (var sensor in CorePowers) {
                    if (sensor == null)
                        continue;

                    if (!Ring0.Rdmsr(energyStatusMSRs[sensor.Index], out var eax, out _))
                        continue;

                    var time = DateTime.UtcNow;
                    var energyConsumed = eax;
                    var deltaTime =
                        (float)(time - lastEnergyTime[sensor.Index]).TotalSeconds;
                    if (deltaTime < 0.01)
                        continue;

                    sensor.Value = energyUnitMultiplier * unchecked(
                                       energyConsumed - lastEnergyConsumed[sensor.Index]) / deltaTime;
                    lastEnergyTime[sensor.Index] = time;
                    lastEnergyConsumed[sensor.Index] = energyConsumed;
                }
        }
    }
}
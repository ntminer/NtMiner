using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HardwareProviders.CPU {
    public class Cpu : Hardware {
        public int CoreCount { get; }

        public CpuLoad CpuLoad { get; }
        public Sensor[] CoreLoads { get; private set; }

        public Sensor TotalLoad { get; }
        public Vendor Vendor { get; }

        public Sensor BusClock { get; protected set; }
        public virtual Sensor PackageTemperature { get; protected set; }

        public virtual Sensor[] CoreClocks { get; protected set; } = new Sensor[0];
        public virtual Sensor[] CoreTemperatures { get; protected set; } = new Sensor[0];
        public virtual Sensor[] CorePowers { get; protected set; } = new Sensor[0];
        public virtual Sensor[] CoreVoltages { get; protected set; } = new Sensor[0];

        public double EstimatedTimeStampCounterFrequency { get; set; }
        public double EstimatedTimeStampCounterFrequencyError { get; set; }

        public bool HasModelSpecificRegisters { get; }

        public bool HasTimeStampCounter { get; }

        public double TimeStampCounterFrequency { get; private set; }

        protected readonly CpuId[][] CpuId;
        private long lastTime;
        private ulong lastTimeStampCount;
        protected readonly uint family;

        private readonly bool isInvariantTimeStampCounter;
        protected readonly uint model;

        protected readonly int processorIndex;
        protected readonly uint stepping;

        protected Cpu(int processorIndex, CpuId[][] cpuId) : base(cpuId[0][0].Name) {
            this.CpuId = cpuId;

            Vendor = cpuId[0][0].Vendor;
            Identifier = $"{Vendor.ToString()}/{processorIndex.ToString()}";

            family = cpuId[0][0].Family;
            model = cpuId[0][0].Model;
            stepping = cpuId[0][0].Stepping;

            this.processorIndex = processorIndex;
            CoreCount = cpuId.Length;

            // check if processor has MSRs
            if (cpuId[0][0].Data.GetLength(0) > 1
                && (cpuId[0][0].Data[1, 3] & 0x20) != 0)
                HasModelSpecificRegisters = true;
            else
                HasModelSpecificRegisters = false;

            // check if processor has a TSC
            if (cpuId[0][0].Data.GetLength(0) > 1
                && (cpuId[0][0].Data[1, 3] & 0x10) != 0)
                HasTimeStampCounter = true;
            else
                HasTimeStampCounter = false;

            // check if processor supports an invariant TSC 
            if (cpuId[0][0].ExtData.GetLength(0) > 7
                && (cpuId[0][0].ExtData[7, 3] & 0x100) != 0)
                isInvariantTimeStampCounter = true;
            else
                isInvariantTimeStampCounter = false;

            TotalLoad = CoreCount > 1 ? new Sensor("CPU Total", 0, SensorType.Load, this) : null;
            CoreLoads = new Sensor[CoreCount];
            for (var i = 0; i < CoreLoads.Length; i++)
                CoreLoads[i] = new Sensor(CoreString(i), i + 1,
                    SensorType.Load, this);
            CpuLoad = new CpuLoad(cpuId);
            if (CpuLoad.IsAvailable) {
                foreach (var sensor in CoreLoads)
                    ActivateSensor(sensor);
                if (TotalLoad != null)
                    ActivateSensor(TotalLoad);
            }

            if (HasTimeStampCounter) {
                var mask = ThreadAffinity.Set(1UL << cpuId[0][0].Thread);

                EstimateTimeStampCounterFrequency();

                ThreadAffinity.Set(mask);
            }
            else {
                EstimatedTimeStampCounterFrequency = 0;
            }

            TimeStampCounterFrequency = EstimatedTimeStampCounterFrequency;
        }

        public static Cpu[] Discover() {
            EnsureHook();

            var cpus = new List<Cpu>();
            var processorThreads = GetCpuId();
            var threads = new CpuId[processorThreads.Length][][];

            var index = 0;
            foreach (var cpuids in processorThreads) {
                if (cpuids.Length == 0)
                    continue;

                var coreThreads = GroupThreadsByCore(cpuids);

                threads[index] = coreThreads;

                switch (cpuids[0].Vendor) {
                    case Vendor.Intel:
                        cpus.Add(new IntelCpu(index, coreThreads));
                        break;
                    case Vendor.AMD:
                        switch (cpuids[0].Family) {
                            case 0x0F:
                                cpus.Add(new AmdCpu0(index, coreThreads));
                                break;
                            case 0x10:
                            case 0x11:
                            case 0x12:
                            case 0x14:
                            case 0x15:
                            case 0x16:
                                cpus.Add(new AmdCpu10(index, coreThreads));
                                break;
                            case 0x17:
                                cpus.Add(new AmdCpu17(index, coreThreads));
                                break;
                            default:
                                cpus.Add(new Cpu(index, coreThreads));
                                break;
                        }

                        break;
                    default:
                        cpus.Add(new Cpu(index, coreThreads));
                        break;
                }

                index++;
            }

            return cpus.ToArray();
        }

        private static CpuId[][] GetCpuId() {
            var threads = new List<CpuId>();
            for (var i = 0; i < 64; i++)
                try {
                    threads.Add(new CpuId(i));
                }
                catch (ArgumentOutOfRangeException) {
                    break;
                }

            var processors =
                new SortedDictionary<uint, List<CpuId>>();
            foreach (var thread in threads) {
                processors.TryGetValue(thread.ProcessorId, out var list);
                if (list == null) {
                    list = new List<CpuId>();
                    processors.Add(thread.ProcessorId, list);
                }

                list.Add(thread);
            }

            var processorThreads = new CpuId[processors.Count][];
            var index = 0;
            foreach (var list in processors.Values) {
                processorThreads[index] = list.ToArray();
                index++;
            }

            return processorThreads;
        }

        private static CpuId[][] GroupThreadsByCore(IEnumerable<CpuId> threads) {
            var cores =
                new SortedDictionary<uint, List<CpuId>>();
            foreach (var thread in threads) {
                cores.TryGetValue(thread.CoreId, out var coreList);
                if (coreList == null) {
                    coreList = new List<CpuId>();
                    cores.Add(thread.CoreId, coreList);
                }

                coreList.Add(thread);
            }

            var coreThreads = new CpuId[cores.Count][];
            var index = 0;
            foreach (var list in cores.Values) {
                coreThreads[index] = list.ToArray();
                index++;
            }

            return coreThreads;
        }


        protected string CoreString(int i) {
            if (CoreCount == 1)
                return "CPU Core";
            return "CPU Core #" + (i + 1);
        }

        private void EstimateTimeStampCounterFrequency() {
            // preload the function
            EstimateTimeStampCounterFrequency(0, out var f, out var e);
            EstimateTimeStampCounterFrequency(0, out f, out e);

            // estimate the frequency
            EstimatedTimeStampCounterFrequencyError = double.MaxValue;
            EstimatedTimeStampCounterFrequency = 0;
            for (var i = 0; i < 5; i++) {
                EstimateTimeStampCounterFrequency(0.025, out f, out e);
                if (e < EstimatedTimeStampCounterFrequencyError) {
                    EstimatedTimeStampCounterFrequencyError = e;
                    EstimatedTimeStampCounterFrequency = f;
                }

                if (EstimatedTimeStampCounterFrequencyError < 1e-4)
                    break;
            }
        }

        private static void EstimateTimeStampCounterFrequency(double timeWindow,
            out double frequency, out double error) {
            var ticks = (long)(timeWindow * Stopwatch.Frequency);

            var timeBegin = Stopwatch.GetTimestamp() +
                            (long)Math.Ceiling(0.001 * ticks);
            var timeEnd = timeBegin + ticks;

            while (Stopwatch.GetTimestamp() < timeBegin) {
            }

            var countBegin = Opcode.Rdtsc();
            var afterBegin = Stopwatch.GetTimestamp();

            while (Stopwatch.GetTimestamp() < timeEnd) {
            }

            var countEnd = Opcode.Rdtsc();
            var afterEnd = Stopwatch.GetTimestamp();

            double delta = timeEnd - timeBegin;
            frequency = 1e-6 *
                        ((double)(countEnd - countBegin) * Stopwatch.Frequency) / delta;

            var beginError = (afterBegin - timeBegin) / delta;
            var endError = (afterEnd - timeEnd) / delta;
            error = beginError + endError;
        }

        protected virtual uint[] GetMSRs() {
            return null;
        }

        public override void Update() {
            if (CoreClocks.Any(x => x == null)) {
                CoreClocks = CoreClocks.Where(x => x != null).ToArray();
            }
            if (CoreLoads.Any(x => x == null)) {
                CoreLoads = CoreLoads.Where(x => x != null).ToArray();
            }
            if (CorePowers.Any(x => x == null)) {
                CorePowers = CorePowers.Where(x => x != null).ToArray();
            }
            if (CoreTemperatures.Any(x => x == null)) {
                CoreTemperatures = CoreTemperatures.Where(x => x != null).ToArray();
            }
            if (CoreVoltages.Any(x => x == null)) {
                CoreVoltages = CoreVoltages.Where(x => x != null).ToArray();
            }


            if (HasTimeStampCounter && isInvariantTimeStampCounter) {
                // make sure always the same thread is used
                var mask = ThreadAffinity.Set(1UL << CpuId[0][0].Thread);

                // read time before and after getting the TSC to estimate the error
                var firstTime = Stopwatch.GetTimestamp();
                var timeStampCount = Opcode.Rdtsc();
                var time = Stopwatch.GetTimestamp();

                // restore the thread affinity mask
                ThreadAffinity.Set(mask);

                var delta = (double)(time - lastTime) / Stopwatch.Frequency;
                var error = (double)(time - firstTime) / Stopwatch.Frequency;

                // only use data if they are measured accuarte enough (max 0.1ms delay)
                if (error < 0.0001) {
                    // ignore the first reading because there are no initial values 
                    // ignore readings with too large or too small time window
                    if (lastTime != 0 && delta > 0.5 && delta < 2)
                        TimeStampCounterFrequency =
                            (timeStampCount - lastTimeStampCount) / (1e6 * delta);

                    lastTimeStampCount = timeStampCount;
                    lastTime = time;
                }
            }

            if (CpuLoad.IsAvailable) {
                CpuLoad.Update();
                for (var i = 0; i < CoreLoads.Length; i++)
                    CoreLoads[i].Value = CpuLoad.GetCoreLoad(i);
                if (TotalLoad != null)
                    TotalLoad.Value = CpuLoad.GetTotalLoad();
            }
        }
    }
}
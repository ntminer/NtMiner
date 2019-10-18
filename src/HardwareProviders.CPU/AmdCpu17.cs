using System.Linq;
using HardwareProviders.CPU.Ryzen;

namespace HardwareProviders.CPU {
    public sealed class AmdCpu17 : AmdCpu {
        // register index names for CPUID[] 
        private const int EBX = 1;
        private const int ECX = 2;

        internal int SensorClock { get; set; }
        internal int SensorMultipliers { get; set; }
        internal int SensorPower { get; set; }
        internal int SensorTemperatures { get; set; }
        internal int SensorVoltage { get; set; }

        public RyzenProcessor Processor { get; }
        public int NodesPerProcessor { get; }

        public override Sensor[] CoreClocks => Processor.Nodes.SelectMany(node => node.Cores.Select(core => core.Clock)).ToArray();

        public override Sensor[] CorePowers => Processor.Nodes.SelectMany(node => node.Cores.Select(core => core.Power)).ToArray();

        public override Sensor[] CoreVoltages => Processor.Nodes.SelectMany(node => node.Cores.Select(core => core.Voltage)).ToArray();

        public AmdCpu17(int processorIndex, CpuId[][] cpuId)
            : base(processorIndex, cpuId) {
            // add all numa nodes 
            // Register ..1E_ECX, [10:8] + 1 
            Processor = new RyzenProcessor(this);
            NodesPerProcessor = 1 + (int)((cpuId[0][0].ExtData[0x1e, ECX] >> 8) & 0x7);

            // add all numa nodes
            foreach (var cpu in cpuId) {
                var thread = cpu[0];

                // coreID 
                // Register ..1E_EBX, [7:0] 
                var core_id = (int)(thread.ExtData[0x1e, EBX] & 0xff);

                // nodeID 
                // Register ..1E_ECX, [7:0] 
                var node_id = (int)(thread.ExtData[0x1e, ECX] & 0xff);

                Processor.AppendThread(null, node_id, core_id);
            }

            // add all threads to numa nodes and specific core 
            foreach (var cpu in cpuId) {
                var thread = cpu[0];

                // coreID 
                // Register ..1E_EBX, [7:0] 
                var core_id = (int)(thread.ExtData[0x1e, EBX] & 0xff);

                // nodeID 
                // Register ..1E_ECX, [7:0] 
                var node_id = (int)(thread.ExtData[0x1e, ECX] & 0xff);

                Processor.AppendThread(thread, node_id, core_id);
            }

            Update();
        }

        protected override uint[] GetMSRs() {
            return new[] { RyzenCostants.PERF_CTL_0, RyzenCostants.PERF_CTR_0, RyzenCostants.HWCR, RyzenCostants.MSR_PSTATE_0, RyzenCostants.COFVID_STATUS };
        }

        public override void Update() {
            base.Update();

            Processor.UpdateSensors();
            foreach (var node in Processor.Nodes) {
                node.UpdateSensors();

                foreach (var c in node.Cores) c.UpdateSensors();
            }
        }
    }
}
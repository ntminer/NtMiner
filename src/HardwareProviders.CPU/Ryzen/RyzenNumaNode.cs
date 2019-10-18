using System.Collections.Generic;

namespace HardwareProviders.CPU.Ryzen {
    public class RyzenNumaNode {
        private readonly AmdCpu17 _cpu;

        public RyzenNumaNode(AmdCpu17 cpu, int id) {
            Cores = new List<RyzenCore>();
            NodeId = id;
            _cpu = cpu;
        }

        public int NodeId { get; }
        public List<RyzenCore> Cores { get; }

        public void AppendThread(CpuId thread, int core_id) {
            RyzenCore ryzenCore = null;
            foreach (var c in Cores)
                if (c.CoreId == core_id)
                    ryzenCore = c;
            if (ryzenCore == null) {
                ryzenCore = new RyzenCore(_cpu, core_id);
                Cores.Add(ryzenCore);
            }

            if (thread != null)
                ryzenCore.Threads.Add(thread);
        }

        #region UpdateSensors

        public void UpdateSensors() {
        }

        #endregion
    }
}
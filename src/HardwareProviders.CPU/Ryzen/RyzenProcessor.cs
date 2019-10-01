using System;
using System.Collections.Generic;

namespace HardwareProviders.CPU.Ryzen {
    public class RyzenProcessor {
        private readonly AmdCpu17 _cpu;
        private DateTime _lastPwrTime = new DateTime(0);
        private uint _lastPwrValue;

        public Sensor CoreTemperatureTctl { get; set; }
        public Sensor CoreTemperatureTdie { get; set; }
        public Sensor CoreVoltage { get; set; }
        public Sensor PackagePower { get; set; }
        public Sensor SocVoltage { get; set; }

        private readonly List<RyzenNumaNode> _nodes;

        public RyzenNumaNode[] Nodes => _nodes.ToArray();

        public RyzenProcessor(AmdCpu17 hw) {
            _cpu = hw;
            _nodes = new List<RyzenNumaNode>();

            PackagePower = new Sensor("Package Power", _cpu.SensorPower++, SensorType.Power, _cpu);
            CoreTemperatureTctl =
                new Sensor("Core (Tctl)", _cpu.SensorTemperatures++, SensorType.Temperature, _cpu);
            CoreTemperatureTdie =
                new Sensor("Core (Tdie)", _cpu.SensorTemperatures++, SensorType.Temperature, _cpu);
            CoreVoltage = new Sensor("Core (SVI2)", _cpu.SensorVoltage++, SensorType.Voltage, _cpu);
            SocVoltage = new Sensor("SoC (SVI2)", _cpu.SensorVoltage++, SensorType.Voltage, _cpu);

            _cpu.ActivateSensor(PackagePower);
            _cpu.ActivateSensor(CoreTemperatureTctl);
            _cpu.ActivateSensor(CoreTemperatureTdie);
            _cpu.ActivateSensor(CoreVoltage);
        }


        #region UpdateSensors

        public void UpdateSensors() {
            var node = _nodes[0];
            var core = node?.Cores[0];
            var cpu = core?.Threads[0];
            if (cpu == null)
                return;

            var mask = Ring0.ThreadAffinitySet(1UL << cpu.Thread);

            // MSRC001_0299 
            // TU [19:16] 
            // ESU [12:8] -> Unit 15.3 micro Joule per increment 
            // PU [3:0] 
            Ring0.Rdmsr(RyzenCostants.MSR_PWR_UNIT, out var eax, out _);

            // MSRC001_029B 
            // total_energy [31:0] 
            var sample_time = DateTime.Now;
            Ring0.Rdmsr(RyzenCostants.MSR_PKG_ENERGY_STAT, out eax, out _);
            var total_energy = eax;

            // THM_TCON_CUR_TMP 
            // CUR_TEMP [31:21] 
            Ring0.WritePciConfig(Ring0.GetPciAddress(0, 0, 0), RyzenCostants.FAMILY_17H_PCI_CONTROL_REGISTER, RyzenCostants.F17H_M01H_THM_TCON_CUR_TMP);
            Ring0.ReadPciConfig(Ring0.GetPciAddress(0, 0, 0), RyzenCostants.FAMILY_17H_PCI_CONTROL_REGISTER + 4, out var temperature);

            // SVI0_TFN_PLANE0 [0] 
            // SVI0_TFN_PLANE1 [1] 
            Ring0.WritePciConfig(Ring0.GetPciAddress(0, 0, 0), RyzenCostants.FAMILY_17H_PCI_CONTROL_REGISTER, RyzenCostants.F17H_M01H_SVI + 0x8);
            Ring0.ReadPciConfig(Ring0.GetPciAddress(0, 0, 0), RyzenCostants.FAMILY_17H_PCI_CONTROL_REGISTER + 4, out var smusvi0_tfn);

            // SVI0_PLANE0_VDDCOR [24:16] 
            // SVI0_PLANE0_IDDCOR [7:0] 
            Ring0.WritePciConfig(Ring0.GetPciAddress(0, 0, 0), RyzenCostants.FAMILY_17H_PCI_CONTROL_REGISTER, RyzenCostants.F17H_M01H_SVI + 0xc);
            Ring0.ReadPciConfig(Ring0.GetPciAddress(0, 0, 0), RyzenCostants.FAMILY_17H_PCI_CONTROL_REGISTER + 4, out var smusvi0_tel_plane0);

            // SVI0_PLANE1_VDDCOR [24:16] 
            // SVI0_PLANE1_IDDCOR [7:0] 
            Ring0.WritePciConfig(Ring0.GetPciAddress(0, 0, 0), RyzenCostants.FAMILY_17H_PCI_CONTROL_REGISTER, RyzenCostants.F17H_M01H_SVI + 0x10);
            Ring0.ReadPciConfig(Ring0.GetPciAddress(0, 0, 0), RyzenCostants.FAMILY_17H_PCI_CONTROL_REGISTER + 4, out var smusvi0_tel_plane1);

            Ring0.ThreadAffinitySet(mask);

            // power consumption 
            // power.Value = (float) ((double)pu * 0.125); 
            // esu = 15.3 micro Joule per increment 
            if (_lastPwrTime.Ticks == 0) {
                _lastPwrTime = sample_time;
                _lastPwrValue = total_energy;
            }

            // ticks diff 
            var time = sample_time - _lastPwrTime;
            long pwr;
            if (_lastPwrValue <= total_energy)
                pwr = total_energy - _lastPwrValue;
            else
                pwr = 0xffffffff - _lastPwrValue + total_energy;

            // update for next sample 
            _lastPwrTime = sample_time;
            _lastPwrValue = total_energy;

            var energy = 15.3e-6 * pwr;
            energy /= time.TotalSeconds;

            PackagePower.Value = (float)energy;

            // current temp Bit [31:21]
            //If bit 19 of the Temperature Control register is set, there is an additional offset of 49 degrees C.
            var tempOffsetFlag = (temperature & RyzenCostants.F17H_TEMP_OFFSET_FLAG) != 0;
            temperature = (temperature >> 21) * 125;

            var offset = 0.0f;
            if (cpu.Name != null && (cpu.Name.Contains("2600X") || cpu.Name.Contains("2700X")))
                offset = -10.0f;
            if (cpu.Name != null && (cpu.Name.Contains("1600X") || cpu.Name.Contains("1700X") ||
                                     cpu.Name.Contains("1800X")))
                offset = -20.0f;
            else if (cpu.Name != null && (cpu.Name.Contains("1920X") || cpu.Name.Contains("1950X") ||
                                          cpu.Name.Contains("1900X")))
                offset = -27.0f;
            else if (cpu.Name != null &&
                     (cpu.Name.Contains("1910") || cpu.Name.Contains("1920") || cpu.Name.Contains("1950")))
                offset = -10.0f;

            var t = temperature * 0.001f;
            if (tempOffsetFlag)
                t += -49.0f;

            CoreTemperatureTctl.Value = t;
            CoreTemperatureTdie.Value = t + offset;

            // voltage 
            var VIDStep = 0.00625;
            double vcc;
            uint svi0_plane_x_vddcor;

            //Core
            if ((smusvi0_tfn & 0x01) == 0) {
                svi0_plane_x_vddcor = (smusvi0_tel_plane0 >> 16) & 0xff;
                vcc = 1.550 - VIDStep * svi0_plane_x_vddcor;
                CoreVoltage.Value = (float)vcc;
            }

            // SoC 
            // not every zen cpu has this voltage 
            if ((smusvi0_tfn & 0x02) == 0) {
                svi0_plane_x_vddcor = (smusvi0_tel_plane1 >> 16) & 0xff;
                vcc = 1.550 - VIDStep * svi0_plane_x_vddcor;
                SocVoltage.Value = (float)vcc;
                _cpu.ActivateSensor(SocVoltage);
            }
        }

        #endregion

        public void AppendThread(CpuId thread, int numa_id, int core_id) {
            RyzenNumaNode node = null;
            foreach (var n in _nodes)
                if (n.NodeId == numa_id)
                    node = n;
            if (node == null) {
                node = new RyzenNumaNode(_cpu, numa_id);
                _nodes.Add(node);
            }

            if (thread != null)
                node.AppendThread(thread, core_id);
        }
    }
}
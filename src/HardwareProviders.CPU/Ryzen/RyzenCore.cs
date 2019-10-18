using System;
using System.Collections.Generic;

namespace HardwareProviders.CPU.Ryzen {
    public class RyzenCore {
        public Sensor Clock { get; }
        public Sensor Multiplier { get; }
        public Sensor Power { get; }
        public Sensor Voltage { get; }

        private DateTime _lastPwrTime = new DateTime(0);
        private uint _lastPwrValue;

        public RyzenCore(AmdCpu17 cpu, int id) {
            Threads = new List<CpuId>();
            CoreId = id;
            Clock = new Sensor("Core #" + CoreId, cpu.SensorClock++, SensorType.Clock, cpu);
            Multiplier = new Sensor("Core #" + CoreId, cpu.SensorMultipliers++, SensorType.Factor, cpu);
            Power = new Sensor("Core #" + CoreId + " (SMU)", cpu.SensorPower++, SensorType.Power, cpu);
            Voltage = new Sensor("Core #" + CoreId + " VID", cpu.SensorVoltage++, SensorType.Voltage, cpu);

            cpu.ActivateSensor(Clock);
            cpu.ActivateSensor(Multiplier);
            cpu.ActivateSensor(Power);
            cpu.ActivateSensor(Voltage);
        }

        public int CoreId { get; }
        public List<CpuId> Threads { get; }

        #region UpdateSensors

        public void UpdateSensors() {
            // CPUID cpu = threads.FirstOrDefault(); 
            var cpu = Threads[0];
            if (cpu == null)
                return;
            var mask = Ring0.ThreadAffinitySet(1UL << cpu.Thread);

            // MSRC001_0299 
            // TU [19:16] 
            // ESU [12:8] -> Unit 15.3 micro Joule per increment 
            // PU [3:0] 
            Ring0.Rdmsr(RyzenCostants.MSR_PWR_UNIT, out var eax, out _);

            // MSRC001_029A 
            // total_energy [31:0] 
            var sample_time = DateTime.Now;
            Ring0.Rdmsr(RyzenCostants.MSR_CORE_ENERGY_STAT, out eax, out _);
            var total_energy = eax;

            // MSRC001_0293 
            // CurHwPstate [24:22] 
            // CurCpuVid [21:14] 
            // CurCpuDfsId [13:8] 
            // CurCpuFid [7:0] 
            Ring0.Rdmsr(RyzenCostants.MSR_HARDWARE_PSTATE_STATUS, out eax, out _);
            var CurCpuVid = (int)((eax >> 14) & 0xff);
            var CurCpuDfsId = (int)((eax >> 8) & 0x3f);
            var CurCpuFid = (int)(eax & 0xff);

            // MSRC001_0064 + x 
            // IddDiv [31:30] 
            // IddValue [29:22] 
            // CpuVid [21:14] 
            // CpuDfsId [13:8] 
            // CpuFid [7:0] 
            // Ring0.Rdmsr(MSR_PSTATE_0 + (uint)CurHwPstate, out eax, out edx); 
            // int IddDiv = (int)((eax >> 30) & 0x03); 
            // int IddValue = (int)((eax >> 22) & 0xff); 
            // int CpuVid = (int)((eax >> 14) & 0xff); 
            Ring0.ThreadAffinitySet(mask);

            // clock 
            // CoreCOF is (Core::X86::Msr::PStateDef[CpuFid[7:0]] / Core::X86::Msr::PStateDef[CpuDfsId]) * 200 
            Clock.Value = (float)(CurCpuFid / (double)CurCpuDfsId * 200.0);

            // multiplier 
            Multiplier.Value = (float)(CurCpuFid / (double)CurCpuDfsId * 2.0);

            // Voltage 
            var VIDStep = 0.00625;
            var vcc = 1.550 - VIDStep * CurCpuVid;
            Voltage.Value = (float)vcc;

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

            Power.Value = (float)energy;
        }

        #endregion
    }
}
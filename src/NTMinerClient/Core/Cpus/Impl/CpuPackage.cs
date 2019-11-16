using NTMiner.Profile;
using System;
using System.Threading.Tasks;

namespace NTMiner.Core.Cpus.Impl {
    public class CpuPackage : ICpuPackage {
        private bool _isFirst = true;
        public CpuPackage(IMinerProfile minerProfile) {
            VirtualRoot.BuildEventPath<Per1SecondEvent>("周期更新CpuAll的状态", LogEnum.None,
                action: message => {
                    if (_isFirst) {
                        _isFirst = false;
                        Task.Factory.StartNew(() => {
                            // 因为初始化费时间所以第一次访问放在Task中
                            Update((int)Windows.Cpu.Instance.GetPerformance(), (int)Windows.Cpu.Instance.GetTemperature());
                        });
                    }
                    else {
                        Update((int)Windows.Cpu.Instance.GetPerformance(), (int)Windows.Cpu.Instance.GetTemperature());
                        if (minerProfile.IsAutoStopByCpu) {
                            if (NTMinerRoot.Instance.IsMining) {
                                NTMinerRoot.LowTemperatureCount = 0;
                                if (this.Temperature >= minerProfile.CpuStopTemperature) {
                                    NTMinerRoot.HighTemperatureCount++;
                                }
                                else {
                                    NTMinerRoot.HighTemperatureCount = 0;
                                }
                                if (NTMinerRoot.HighTemperatureCount >= minerProfile.CpuGETemperatureSeconds) {
                                    NTMinerRoot.HighTemperatureCount = 0;
                                    NTMinerRoot.Instance.StopMineAsync(StopMineReason.HighCpuTemperature);
                                    VirtualRoot.ThisLocalInfo(nameof(CpuPackage), $"自动停止挖矿，因为 CPU 温度连续{minerProfile.CpuGETemperatureSeconds}秒不低于{minerProfile.CpuStopTemperature}℃", toConsole: true);
                                }
                            }
                            else {
                                NTMinerRoot.HighTemperatureCount = 0;
                                if (minerProfile.IsAutoStartByCpu && NTMinerRoot.Instance.StopReason == StopMineReason.HighCpuTemperature) {
                                    if (this.Temperature <= minerProfile.CpuStartTemperature) {
                                        NTMinerRoot.LowTemperatureCount++;
                                    }
                                    else {
                                        NTMinerRoot.LowTemperatureCount = 0;
                                    }
                                    if (NTMinerRoot.LowTemperatureCount >= minerProfile.CpuLETemperatureSeconds) {
                                        NTMinerRoot.LowTemperatureCount = 0;
                                        VirtualRoot.ThisLocalInfo(nameof(CpuPackage), $"自动开始挖矿，因为 CPU 温度连续{minerProfile.CpuLETemperatureSeconds}秒不高于{minerProfile.CpuStartTemperature}℃", toConsole: true);
                                        NTMinerRoot.Instance.StartMine();
                                    }
                                }
                            }
                        }
                    }
                });
        }

        private void Update(int performance, int temperature) {
            bool isChanged = false;
            if (performance != this.Performance) {
                isChanged = true;
                this.Performance = performance;
            }
            if (temperature != this.Temperature) {
                isChanged = true;
                this.Temperature = temperature;
            }
            if (isChanged) {
                VirtualRoot.RaiseEvent(new CpuPackageStateChangedEvent());
            }
        }

        public int Performance { get; set; }

        public int Temperature { get; set; }

        public int HighCpuPercent { get; set; }

        public int HighCpuSeconds { get; set; }

        public DateTime LastLowCpuOn { get; set; }

        public void ResetCpu(int highCpuPercent, int highCpuSeconds) {
            this.HighCpuPercent = highCpuPercent;
            this.HighCpuSeconds = highCpuSeconds;
            this.LastLowCpuOn = DateTime.Now;
        }
    }
}

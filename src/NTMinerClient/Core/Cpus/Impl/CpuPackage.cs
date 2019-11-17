using NTMiner.Profile;
using System;
using System.Threading.Tasks;

namespace NTMiner.Core.Cpus.Impl {
    public class CpuPackage : ICpuPackage {
        private bool _isFirst = true;
        private readonly IMinerProfile _minerProfile;
        public CpuPackage(IMinerProfile minerProfile) {
            _minerProfile = minerProfile;
        }

        public void Start() {
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
                        if (_minerProfile.IsAutoStopByCpu) {
                            if (NTMinerRoot.Instance.IsMining) {
                                /* 挖矿中时周期更新最后一次温度低于挖矿停止温度的时刻，然后检查最后一次低于
                                 * 挖矿停止温度的时刻距离现在是否已经超过了设定的时常，如果超过了则自动停止挖矿*/
                                HighTemperatureOn = DateTime.MinValue;
                                // 如果当前温度低于挖矿停止温度则更新记录的低温时刻
                                if (this.Temperature < _minerProfile.CpuStopTemperature) {
                                    LowTemperatureOn = message.Timestamp;
                                }
                                if (LowTemperatureOn != DateTime.MinValue && (message.Timestamp - LowTemperatureOn).TotalSeconds >= _minerProfile.CpuGETemperatureSeconds) {
                                    LowTemperatureOn = message.Timestamp;
                                    NTMinerRoot.Instance.StopMineAsync(StopMineReason.HighCpuTemperature);
                                    VirtualRoot.ThisLocalInfo(nameof(CpuPackage), $"自动停止挖矿，因为 CPU 温度连续{_minerProfile.CpuGETemperatureSeconds}秒不低于{_minerProfile.CpuStopTemperature}℃", toConsole: true);
                                }
                            }
                            else {
                                /* 高温停止挖矿后周期更新最后一次温度高于挖矿停止温度的时刻，然后检查最后一次高于
                                 * 挖矿停止温度的时刻距离现在是否已经超过了设定的时常，如果超过了则自动开始挖矿*/
                                LowTemperatureOn = DateTime.MinValue;
                                if (_minerProfile.IsAutoStartByCpu && NTMinerRoot.Instance.StopReason == StopMineReason.HighCpuTemperature) {
                                    // 当前温度高于挖矿停止温度则更新记录的高温时刻
                                    if (this.Temperature > _minerProfile.CpuStartTemperature) {
                                        HighTemperatureOn = message.Timestamp;
                                    }
                                    if (HighTemperatureOn != DateTime.MinValue && (message.Timestamp - HighTemperatureOn).TotalSeconds >= _minerProfile.CpuLETemperatureSeconds) {
                                        HighTemperatureOn = message.Timestamp;
                                        VirtualRoot.ThisLocalInfo(nameof(CpuPackage), $"自动开始挖矿，因为 CPU 温度连续{_minerProfile.CpuLETemperatureSeconds}秒不高于{_minerProfile.CpuStartTemperature}℃", toConsole: true);
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

        public DateTime LowPerformanceOn { get; set; }

        public DateTime HighTemperatureOn { get; set; }
        public DateTime LowTemperatureOn { get; set; }
    }
}

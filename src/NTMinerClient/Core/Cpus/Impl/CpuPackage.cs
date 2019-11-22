using NTMiner.Profile;
using System;
using System.Threading.Tasks;

namespace NTMiner.Core.Cpus.Impl {
    public class CpuPackage : ICpuPackage {
        private bool _isFirst = true;
        private readonly IMinerProfile _minerProfile;
        public CpuPackage(IMinerProfile minerProfile) {
            _minerProfile = minerProfile;
            Reset();
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
                        #region CPU温度过高时自动停止挖矿和温度降低时自动开始挖矿
                        if (_minerProfile.IsAutoStopByCpu) {
                            if (NTMinerRoot.Instance.IsMining) {
                                /* 挖矿中时周期更新最后一次温度低于挖矿停止温度的时刻，然后检查最后一次低于
                                 * 挖矿停止温度的时刻距离现在是否已经超过了设定的时常，如果超过了则自动停止挖矿*/
                                HighTemperatureOn = message.Timestamp;
                                // 如果当前温度低于挖矿停止温度则更新记录的低温时刻
                                if (this.Temperature < _minerProfile.CpuStopTemperature) {
                                    LowTemperatureOn = message.Timestamp;
                                }
                                if ((message.Timestamp - LowTemperatureOn).TotalSeconds >= _minerProfile.CpuGETemperatureSeconds) {
                                    LowTemperatureOn = message.Timestamp;
                                    VirtualRoot.ThisLocalWarn(nameof(CpuPackage), $"自动停止挖矿，因为 CPU 温度连续{_minerProfile.CpuGETemperatureSeconds.ToString()}秒不低于{_minerProfile.CpuStopTemperature.ToString()}℃", toConsole: true);
                                    NTMinerRoot.Instance.StopMineAsync(StopMineReason.HighCpuTemperature);
                                }
                            }
                            else {
                                /* 高温停止挖矿后周期更新最后一次温度高于挖矿停止温度的时刻，然后检查最后一次高于
                                 * 挖矿停止温度的时刻距离现在是否已经超过了设定的时常，如果超过了则自动开始挖矿*/
                                LowTemperatureOn = message.Timestamp;
                                if (_minerProfile.IsAutoStartByCpu && NTMinerRoot.Instance.StopReason == StopMineReason.HighCpuTemperature) {
                                    // 当前温度高于挖矿停止温度则更新记录的高温时刻
                                    if (this.Temperature > _minerProfile.CpuStartTemperature) {
                                        HighTemperatureOn = message.Timestamp;
                                    }
                                    if ((message.Timestamp - HighTemperatureOn).TotalSeconds >= _minerProfile.CpuLETemperatureSeconds) {
                                        HighTemperatureOn = message.Timestamp;
                                        VirtualRoot.ThisLocalWarn(nameof(CpuPackage), $"自动开始挖矿，因为 CPU 温度连续{_minerProfile.CpuLETemperatureSeconds.ToString()}秒不高于{_minerProfile.CpuStartTemperature.ToString()}℃", toConsole: true);
                                        NTMinerRoot.Instance.StartMine();
                                    }
                                }
                            }
                        }
                        #endregion
                        if (_minerProfile.IsRaiseHighCpuEvent) {
                            if (this.Performance < _minerProfile.HighCpuBaseline) {
                                LowPerformanceOn = message.Timestamp;
                            }
                            if ((message.Timestamp - LowPerformanceOn).TotalSeconds >= _minerProfile.HighCpuSeconds) {
                                LowPerformanceOn = message.Timestamp;
                                VirtualRoot.ThisLocalWarn(nameof(CpuPackage), $"CPU使用率过高：连续{_minerProfile.HighCpuSeconds.ToString()}秒不低于{_minerProfile.HighCpuBaseline.ToString()}%");
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

        public void Reset() {
            DateTime now = DateTime.Now;
            this.LowTemperatureOn = DateTime.Now;
            this.LowPerformanceOn = now;
            this.HighTemperatureOn = now;
        }

        public int Performance { get; set; }

        public int Temperature { get; set; }

        public DateTime LowPerformanceOn { get; set; }

        public DateTime HighTemperatureOn { get; set; }
        public DateTime LowTemperatureOn { get; set; }
    }
}

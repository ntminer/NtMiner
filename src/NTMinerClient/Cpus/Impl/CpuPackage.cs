using NTMiner.Core.Profile;
using OpenHardwareMonitor.Hardware;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Cpus.Impl {
    public class CpuPackage : ICpuPackage {
        private readonly IMinerProfile _minerProfile;
        public CpuPackage(IMinerProfile minerProfile) {
            _minerProfile = minerProfile;
            Reset();
        }

        internal void Init() {
            if (ClientAppType.IsMinerClient) {
                Task.Factory.StartNew(() => {
                    // 注意：第一次GetTemperature请求约需要160毫秒，所以提前在非UI线程做第一次请求。
                    GetTemperature();
                    VirtualRoot.BuildEventPath<Per2SecondEvent>("周期更新CpuAll的状态", LogEnum.None, location: this.GetType(), PathPriority.Normal,
                        path: message => {
                            Task.Factory.StartNew(() => {
                                // 因为获取cpu温度的操作耗时100毫秒
                                Update();
                                #region CPU温度过高时自动停止挖矿和温度降低时自动开始挖矿
                                if (_minerProfile.IsAutoStopByCpu) {
                                    if (NTMinerContext.Instance.IsMining) {
                                        /* 挖矿中时周期更新最后一次温度低于挖矿停止温度的时刻，然后检查最后一次低于
                                         * 挖矿停止温度的时刻距离现在是否已经超过了设定的时常，如果超过了则自动停止挖矿*/
                                        HighTemperatureOn = message.BornOn;
                                        // 如果当前温度低于挖矿停止温度则更新记录的低温时刻
                                        if (this.Temperature < _minerProfile.CpuStopTemperature) {
                                            LowTemperatureOn = message.BornOn;
                                        }
                                        if ((message.BornOn - LowTemperatureOn).TotalSeconds >= _minerProfile.CpuGETemperatureSeconds) {
                                            LowTemperatureOn = message.BornOn;
                                            VirtualRoot.ThisLocalWarn(nameof(CpuPackage), $"自动停止挖矿，因为 CPU 温度连续{_minerProfile.CpuGETemperatureSeconds.ToString()}秒不低于{_minerProfile.CpuStopTemperature.ToString()}℃", toConsole: true);
                                            NTMinerContext.Instance.StopMineAsync(StopMineReason.HighCpuTemperature);
                                        }
                                    }
                                    else {
                                        /* 高温停止挖矿后周期更新最后一次温度高于挖矿停止温度的时刻，然后检查最后一次高于
                                         * 挖矿停止温度的时刻距离现在是否已经超过了设定的时常，如果超过了则自动开始挖矿*/
                                        LowTemperatureOn = message.BornOn;
                                        if (_minerProfile.IsAutoStartByCpu && NTMinerContext.Instance.StopReason == StopMineReason.HighCpuTemperature) {
                                            // 当前温度高于挖矿停止温度则更新记录的高温时刻
                                            if (this.Temperature > _minerProfile.CpuStartTemperature) {
                                                HighTemperatureOn = message.BornOn;
                                            }
                                            if ((message.BornOn - HighTemperatureOn).TotalSeconds >= _minerProfile.CpuLETemperatureSeconds) {
                                                HighTemperatureOn = message.BornOn;
                                                VirtualRoot.ThisLocalWarn(nameof(CpuPackage), $"自动开始挖矿，因为 CPU 温度连续{_minerProfile.CpuLETemperatureSeconds.ToString()}秒不高于{_minerProfile.CpuStartTemperature.ToString()}℃", toConsole: true);
                                                NTMinerContext.Instance.StartMine();
                                            }
                                        }
                                    }
                                }
                                #endregion
                                if (_minerProfile.IsRaiseHighCpuEvent) {
                                    if (this.Performance < _minerProfile.HighCpuBaseline) {
                                        LowPerformanceOn = message.BornOn;
                                    }
                                    if ((message.BornOn - LowPerformanceOn).TotalSeconds >= _minerProfile.HighCpuSeconds) {
                                        LowPerformanceOn = message.BornOn;
                                        VirtualRoot.ThisLocalWarn(nameof(CpuPackage), $"CPU使用率过高：连续{_minerProfile.HighCpuSeconds.ToString()}秒不低于{_minerProfile.HighCpuBaseline.ToString()}%，如果有杀毒软件需关闭或卸载杀毒软件。");
                                    }
                                }
                            });
                        });
                });
            }
        }

        private void Update() {
            bool isChanged = false;
            // 该操作约耗时100毫秒
            int temperature = (int)GetTemperature();
            double performance = Windows.Cpu.Instance.GetTotalCpuUsage();
            if (performance != this.Performance) {
                isChanged = true;
                this.Performance = (int)performance;
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

        public static float GetTemperature() {
            var computer = ComputerRoot.Computer;
            bool isSingleCore = computer.Hardware.Count(a => a.HardwareType == HardwareType.CPU) == 1;
            for (int i = 0; i < computer.Hardware.Length; i++) {
                var hardware = computer.Hardware[i];
                if (hardware.HardwareType == HardwareType.CPU) {
                    hardware.Update();
                    for (int j = 0; j < hardware.Sensors.Length; j++) {
                        switch (hardware.Sensors[j].SensorType) {
                            case SensorType.Voltage:
                                break;
                            case SensorType.Clock:
                                break;
                            case SensorType.Temperature:
                                if (isSingleCore || hardware.Sensors[j].Name == "CPU Package") {
                                    float? t = hardware.Sensors[j].Value;
                                    if (t.HasValue) {
                                        return t.Value;
                                    }
                                }
                                break;
                            case SensorType.Load:
                                break;
                            case SensorType.Fan:
                                break;
                            case SensorType.Flow:
                                break;
                            case SensorType.Control:
                                break;
                            case SensorType.Level:
                                break;
                            case SensorType.Factor:
                                break;
                            case SensorType.Power:
                                break;
                            case SensorType.Data:
                                break;
                            case SensorType.SmallData:
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return 0.0f;
        }
    }
}

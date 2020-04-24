using OpenHardwareMonitor.Hardware;

namespace NTMiner {
    public static class CpuUtil {
        private static readonly object _locker = new object();
        #region 电脑硬件
        private static Computer _computer = null;
        public static Computer Computer {
            get {
                if (_computer == null) {
                    lock (_locker) {
                        if (_computer == null) {
                            _computer = new Computer();
                            _computer.Open();
                            _computer.CPUEnabled = true;
                            VirtualRoot.AddEventPath<AppExitEvent>($"程序退出时关闭OpenHardwareMonitor", LogEnum.None,
                                message => {
                                    _computer?.Close();
                                }, typeof(VirtualRoot));
                        }
                    }
                }
                return _computer;
            }
        }
        #endregion

        public static void GetSensorValue(out float temperature, out double power) {
            temperature = 0.0f;
            power = 0.0f;
            var computer = Computer;
            for (int i = 0; i < computer.Hardware.Length; i++) {
                var hardware = computer.Hardware[i];
                if (hardware.HardwareType == HardwareType.CPU) {
                    hardware.Update();
                    bool isCPUPackageReaded = false;
                    bool isPowerReaded = false;
                    for (int j = 0; j < hardware.Sensors.Length; j++) {
                        switch (hardware.Sensors[j].SensorType) {
                            case SensorType.Voltage:
                                break;
                            case SensorType.Clock:
                                break;
                            case SensorType.Temperature:
                                if (!isCPUPackageReaded) {
                                    if (hardware.Sensors[j].Name == "CPU Package") {
                                        isCPUPackageReaded = true;
                                        float? t = hardware.Sensors[j].Value;
                                        if (t.HasValue) {
                                            temperature = t.Value;
                                        }
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
                                if (!isPowerReaded) {
                                    if (hardware.Sensors[j].Name == "CPU Package") {
                                        isPowerReaded = true;
                                        float? t = hardware.Sensors[j].Value;
                                        if (t.HasValue) {
                                            power = t.Value;
                                        }
                                    }
                                }
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
        }
    }
}

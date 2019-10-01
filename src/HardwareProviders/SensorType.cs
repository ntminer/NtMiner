namespace HardwareProviders {
    public enum SensorType {
        Voltage, // V
        Clock, // MHz
        Temperature, // °C
        Load, // %
        Frequency, // Hz
        Fan, // RPM
        Flow, // L/h
        Control, // %
        Level, // %
        Factor, // 1
        Power, // W
        Data, // GB = 2^30 Bytes    
        SmallData // MB = 2^20 Bytes
    }
}
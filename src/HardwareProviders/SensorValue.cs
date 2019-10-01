using System;

namespace HardwareProviders {
    public struct SensorValue {
        public SensorValue(float value, DateTime time) {
            Value = value;
            Time = time;
        }

        public float Value { get; }

        public DateTime Time { get; }
    }
}
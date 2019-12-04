using System;
using System.Collections.Generic;
using HardwareProviders.Internals;

namespace HardwareProviders {
    public class Sensor {
        static readonly Dictionary<SensorType, string> Units = new Dictionary<SensorType, string>
        {
            {SensorType.Voltage, "V"},
            {SensorType.Clock, "MHz"},
            {SensorType.Temperature, "°C"},
            {SensorType.Load, "%"},
            {SensorType.Fan, "RPM"},
            {SensorType.Flow, "L/h"},
            {SensorType.Control, "%"},
            {SensorType.Level, "%"},
            {SensorType.Factor, "1"},
            {SensorType.Power, "W"},
            {SensorType.Data, "GB"}
        };


        private readonly RingCollection<SensorValue> values = new RingCollection<SensorValue>();

        private int count;
        private float? currentValue;

        private float sum;

        public Sensor(string name, int index, SensorType sensorType, Hardware hardware) :
            this(name, index, sensorType, hardware, null) {
        }

        public Sensor(string name, int index, SensorType sensorType, Hardware hardware, Parameter[] parameterDescriptions) {
            Index = index;
            SensorType = sensorType;
            Parameters = parameterDescriptions;

            Name = name;
            Identifier = $"{hardware.Identifier}/{SensorType.ToString()}/{Index.ToString()}";
        }

        public SensorType SensorType { get; }

        public string Identifier { get; }

        public string Name { get; }

        public int Index { get; }

        public Parameter[] Parameters { get; }

        public override string ToString() => $"{Name} {(Value.HasValue?Value.Value.ToString() : string.Empty)} {Units[SensorType]}";

        public float? Value {
            get => currentValue;
            set {
                var now = DateTime.UtcNow;
                while (values.Count > 0 && (now - values.First.Time).TotalDays > 1)
                    values.Remove();

                if (value.HasValue) {
                    sum += value.Value;
                    count++;
                    if (count == 4) {
                        AppendValue(sum / count, now);
                        sum = 0;
                        count = 0;
                    }
                }

                currentValue = value;
                if (Min > value || !Min.HasValue)
                    Min = value;
                if (Max < value || !Max.HasValue)
                    Max = value;
            }
        }

        public float? Min { get; private set; }

        public float? Max { get; private set; }

        public IEnumerable<SensorValue> Values => values;

        private void AppendValue(float value, DateTime time) {
            if (values.Count >= 2 && values.Last.Value == value &&
                values[values.Count - 2].Value == value) {
                values.Last = new SensorValue(value, time);
                return;
            }

            values.Append(new SensorValue(value, time));
        }

        public void ResetMin() {
            Min = null;
        }

        public void ResetMax() {
            Max = null;
        }
    }
}
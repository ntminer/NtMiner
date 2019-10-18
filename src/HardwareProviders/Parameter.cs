namespace HardwareProviders {
    public class Parameter {
        public Parameter(string name, string description, float defaultValue) {
            Name = name;
            Description = description;
            Value = defaultValue;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public float Value { get; set; }
    }
}
namespace NTMiner.Core {
    public class EnvironmentVariable {
        public EnvironmentVariable() { }

        public EnvironmentVariable(EnvironmentVariable data) {
            this.Key = data.Key;
            this.Value = data.Value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}

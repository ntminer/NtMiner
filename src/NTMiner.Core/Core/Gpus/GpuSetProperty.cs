namespace NTMiner.Core.Gpus {
    public class GpuSetProperty {
        public GpuSetProperty(string code, string name, object value) {
            this.Code = code;
            this.Name = name;
            this.Value = value;
        }

        public string Code { get; private set; }
        public string Name { get; private set; }
        public object Value { get; private set; }
    }
}

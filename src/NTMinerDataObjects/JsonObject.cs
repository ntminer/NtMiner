namespace NTMiner {
    public class JsonObject<T> {
        public JsonObject() { }
        public JsonObject(T value) {
            Type = typeof(T).Name;
            Value = value;
        }

        public string Type { get; set; }
        public T Value { get; set; }
    }
}

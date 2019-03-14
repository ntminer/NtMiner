using Newtonsoft.Json;

namespace NTMiner.Serialization {
    public class ObjectJsonSerializer : IObjectSerializer {
        public ObjectJsonSerializer() {
        }

        public virtual string Serialize<TObject>(TObject obj) {
            Formatting formatting = Formatting.None;
#if DEBUG
            formatting = Formatting.Indented;
#endif
            return JsonConvert.SerializeObject(obj, formatting);
        }

        public virtual TObject Deserialize<TObject>(string json) {
            return JsonConvert.DeserializeObject<TObject>(json);
        }
    }
}

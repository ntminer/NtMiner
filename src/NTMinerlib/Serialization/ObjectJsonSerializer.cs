using Newtonsoft.Json;

namespace NTMiner.Serialization {
    public class ObjectJsonSerializer : IObjectSerializer {
        public ObjectJsonSerializer() {
        }

        public virtual string Serialize<TObject>(TObject obj) {
#if DEBUG
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings() {
                Formatting = Formatting.Indented
            });
#else
            return JsonConvert.SerializeObject(obj);
#endif
        }

        public virtual TObject Deserialize<TObject>(string json) {
            return JsonConvert.DeserializeObject<TObject>(json);
        }
    }
}

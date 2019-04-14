using Newtonsoft.Json;

namespace NTMiner.Serialization {
    public class ObjectJsonSerializer : IObjectSerializer {
        public ObjectJsonSerializer() {
        }

        public virtual string Serialize<TObject>(TObject obj) {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings() {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public virtual TObject Deserialize<TObject>(string json) {
            return JsonConvert.DeserializeObject<TObject>(json);
        }
    }
}

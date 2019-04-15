using Newtonsoft.Json;

namespace NTMiner.Serialization {
    public class ObjectJsonSerializer : IObjectSerializer {
        private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings() {
            MissingMemberHandling = MissingMemberHandling.Ignore,// 默认值也是Ignore
            NullValueHandling = NullValueHandling.Ignore
        };

        public ObjectJsonSerializer() {
        }

        public virtual string Serialize<TObject>(TObject obj) {
            return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
        }

        public virtual TObject Deserialize<TObject>(string json) {
            return JsonConvert.DeserializeObject<TObject>(json, jsonSerializerSettings);
        }
    }
}

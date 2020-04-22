using Newtonsoft.Json;

namespace NTMiner.Serialization {
    public class NTJsonSerializer : INTSerializer {
        public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings() {
            MissingMemberHandling = MissingMemberHandling.Ignore,// 默认值也是Ignore，复述一遍起文档作用
            NullValueHandling = NullValueHandling.Ignore,
            StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
        };

        public NTJsonSerializer() {
        }

        public virtual string Serialize<TObject>(TObject obj) {
            return JsonConvert.SerializeObject(obj, SerializerSettings);
        }

        public virtual TObject Deserialize<TObject>(string json) {
            try {
                return JsonConvert.DeserializeObject<TObject>(json, SerializerSettings);
            }
            catch {
                return default;
            }
        }
    }
}

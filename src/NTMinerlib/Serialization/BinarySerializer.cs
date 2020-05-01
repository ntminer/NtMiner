using System.Text;

namespace NTMiner.Serialization {
    // 不要使用.NET自带的二进制序列化，因为太慢且需要标记[Serializable]
    public class BinarySerializer : IBinarySerializer {
        private readonly IJsonSerializer _jsonSerializer;
        public BinarySerializer(IJsonSerializer jsonSerializer) {
            _jsonSerializer = jsonSerializer;
        }

        public byte[] Serialize<TObject>(TObject obj) {
            if (obj == null) {
                return new byte[0];
            }
            return Encoding.UTF8.GetBytes(_jsonSerializer.Serialize(obj));
        }

        public TObject Deserialize<TObject>(byte[] stream) {
            if (stream == null || stream.Length == 0) {
                return default;
            }
            return _jsonSerializer.Deserialize<TObject>(Encoding.UTF8.GetString(stream));
        }
    }
}

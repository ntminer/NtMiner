using System;
using System.Text;

namespace NTMiner.Serialization {
    // 不要使用.NET自带的二进制序列化，因为太慢且需要标记[Serializable]
    public class BinarySerializer : IBinarySerializer {
        private static readonly byte[] _head = Encoding.UTF8.GetBytes("gzipped");
        private readonly IJsonSerializer _jsonSerializer;
        public BinarySerializer(IJsonSerializer jsonSerializer) {
            _jsonSerializer = jsonSerializer;
        }

        public byte[] Serialize<TObject>(TObject obj) {
            if (obj == null) {
                return new byte[0];
            }
            byte[] data = Encoding.UTF8.GetBytes(_jsonSerializer.Serialize(obj));
            if (data.Length > NTKeyword.IntK) {// 因为设计了_heade，这个值是可以改的
                data = GZipUtil.Compress(data);
                byte[] array = new byte[_head.Length + data.Length];
                _head.CopyTo(array, 0);
                data.CopyTo(array, _head.Length);
                return array;
            }
            else {
                return data;
            }
        }

        public TObject Deserialize<TObject>(byte[] data) {
            if (data == null || data.Length == 0) {
                return default;
            }
            if (IsGZipped(data)) {
                byte[] array = new byte[data.Length - _head.Length];
                Array.Copy(data, _head.Length, array, 0, array.Length);
                array = GZipUtil.Decompress(array);
                return _jsonSerializer.Deserialize<TObject>(Encoding.UTF8.GetString(array));
            }
            else {
                return _jsonSerializer.Deserialize<TObject>(Encoding.UTF8.GetString(data));
            }
        }

        public bool IsGZipped(byte[] data) {
            if (data == null || data.Length < _head.Length) {
                return false;
            }
            bool isGZipped = true;
            for (int i = 0; i < _head.Length; i++) {
                if (data[i] != _head[i]) {
                    isGZipped = false;
                    break;
                }
            }
            return isGZipped;
        }
    }
}

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NTMiner.Serialization {
    public class BinarySerializer : IBinarySerializer {
        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        public BinarySerializer() { }

        public byte[] Serialize<TObject>(TObject obj) {
            if (obj == null) {
                return new byte[0];
            }
            byte[] ret = null;
            using (MemoryStream ms = new MemoryStream()) {
                binaryFormatter.Serialize(ms, obj);
                ret = ms.ToArray();
                ms.Close();
            }
            return ret;
        }

        public TObject Deserialize<TObject>(byte[] stream) {
            if (stream == null || stream.Length == 0) {
                return default;
            }
            try {
                using (MemoryStream ms = new MemoryStream(stream)) {
                    TObject ret = (TObject)binaryFormatter.Deserialize(ms);
                    ms.Close();
                    return ret;
                }
            }
            catch {
                return default;
            }
        }
    }
}

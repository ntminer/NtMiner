using RabbitMQ.Client;
using System;
using System.Text;

namespace NTMiner.Core.Mq.MqMessagePaths {
    public static class BasicPropertiesExtensions {
        public static string ReadHeaderString(this IBasicProperties basicProperties, string name) {
            if (string.IsNullOrEmpty(name)) {
                return string.Empty;
            }
            if (!basicProperties.IsHeadersPresent()) {
                return string.Empty;
            }
            if (basicProperties.Headers.TryGetValue(name, out object obj) && obj != null) {
                if (obj is byte[] data) {
                    return Encoding.UTF8.GetString(data);
                }
                return obj.ToString();
            }
            return string.Empty;
        }

        public static bool ReadHeaderGuid(this IBasicProperties basicProperties, string name, out Guid guid) {
            guid = Guid.Empty;
            string str = ReadHeaderString(basicProperties, name);
            if (string.IsNullOrEmpty(str)) {
                return false;
            }
            if (Guid.TryParse(str, out guid)) {
                return true;
            }
            return false;
        }
    }
}

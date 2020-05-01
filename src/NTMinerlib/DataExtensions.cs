using NTMiner.Ws;
using System;
using System.Collections.Generic;
using System.Text;

namespace NTMiner {
    public static class DataExtensions {
        public static byte[] SignToBytes(this WsMessage message, string password) {
            if (message == null) {
                throw new InvalidProgramException();
            }
            message.Sign = message.CalcSign(password);
            return ToBytes(message);
        }

        public static byte[] ToBytes(this WsMessage message) {
            if (message == null) {
                throw new InvalidProgramException();
            }
            return VirtualRoot.BinarySerializer.Serialize(message);
        }

        public static string SignToJson(this WsMessage message, string password) {
            if (message == null) {
                throw new InvalidProgramException();
            }
            message.Sign = message.CalcSign(password);
            return ToJson(message);
        }

        public static string ToJson(this WsMessage message) {
            if (message == null) {
                throw new InvalidProgramException();
            }
            return VirtualRoot.JsonSerializer.Serialize(message);
        }

        public static bool TryGetData<T>(this IData message, out T data) {
            if (message == null) {
                throw new InvalidProgramException();
            }
            if (message.Data == null) {
                data = default;
                return false;
            }
            try {
                if (message.Data is Newtonsoft.Json.Linq.JObject jObj) {
                    data = jObj.ToObject<T>();
                    return true;
                }
                else {
                    data = (T)message.Data;
                    return true;
                }
            }
            catch {
                data = default;
                return false;
            }
        }

        public static bool TryGetData<T>(this IData message, out List<T> data) {
            if (message == null) {
                throw new InvalidProgramException();
            }
            if (message.Data == null) {
                data = new List<T>();
                return false;
            }
            try {
                if (message.Data is Newtonsoft.Json.Linq.JArray jArray) {
                    data = new List<T>();
                    foreach (var item in jArray) {
                        data.Add(item.ToObject<T>());
                    }
                    return true;
                }
                else {
                    data = new List<T>();
                    return false;
                }
            }
            catch {
                data = new List<T>();
                return false;
            }
        }

        public static bool TryGetData(this IData message, out Guid guid) {
            guid = Guid.Empty;
            if (message == null) {
                throw new InvalidProgramException();
            }
            if (message.Data == null) {
                return false;
            }
            if (Guid.TryParse(message.Data.ToString(), out guid)) {
                return true;
            }
            return false;
        }

        public static string CalcSign(this WsMessage message, string password) {
            return HashUtil.Sha1(GetSignData(message).Append(password).ToString());
        }

        private static StringBuilder GetSignData(this WsMessage message) {
            if (message == null) {
                throw new InvalidProgramException();
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(message.Id)).Append(message.Id.ToString()).Append(nameof(message.Type)).Append(message.Type).Append(nameof(message.Timestamp)).Append(message.Timestamp.ToString());
            sb.Append(nameof(message.Data)).AppendData(message.Data);
            return sb;
        }

        private static StringBuilder AppendData(this StringBuilder sb, object data) {
            if (data == null) {
                return sb;
            }
            Type dataType = data.GetType();
            if (dataType.IsValueType || dataType == typeof(string)) {
                sb.Append(data.ToString());
                return sb;
            }
            sb.Append(VirtualRoot.JsonSerializer.Serialize(data));
            return sb;
        }
    }
}

using System;
using System.Collections.Generic;

namespace NTMiner {
    public static class DictionaryExtensions {
        public static Guid GetGuidValue(this Dictionary<string, object> dic, string key) {
            if (!dic.ContainsKey(key)) {
                return Guid.Empty;
            }
            var obj = dic[key];
            return ConvertToGuid(obj);
        }

        public static Guid ConvertToGuid(object obj) {
            if (obj == null) {
                return Guid.Empty;
            }
            if (obj is Guid) {
                return (Guid)obj;
            }
            if (obj is string s) {
                if (Guid.TryParse(s, out Guid guid)) {
                    return guid;
                }
            }
            return Guid.Empty;
        }
    }
}

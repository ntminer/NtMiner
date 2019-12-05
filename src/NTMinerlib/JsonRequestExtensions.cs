using System.Collections.Generic;

namespace NTMiner {
    public static class JsonRequestExtensions {
        public static Dictionary<string, object> Parse(this JsonRequest request) {
            Dictionary<string, object> data = VirtualRoot.JsonSerializer.Deserialize<Dictionary<string, object>>(request.json);
            if (data == null) {
                data = new Dictionary<string, object>();
            }
            data.Add("__action", request.action);
            return data;
        }
    }
}

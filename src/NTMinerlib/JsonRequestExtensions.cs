using System.Collections.Generic;

namespace NTMiner {
    public static class JsonRequestExtensions {
        public static Dictionary<string, object> Parse(this JsonRequest request, out string messageId) {
            Dictionary<string, object> data = VirtualRoot.JsonSerializer.Deserialize<Dictionary<string, object>>(request.json);
            if (data == null) {
                data = new Dictionary<string, object>();
            }
            messageId = string.Empty;
            if (data.TryGetValue("messageId", out object obj)) {
                data.Remove("messageId");
                if (obj != null) {
                    messageId = obj.ToString();
                }
            }
            data.Add("__action", request.action);
            return data;
        }
    }
}

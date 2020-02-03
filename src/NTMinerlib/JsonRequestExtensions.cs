using System;
using System.Collections.Generic;

namespace NTMiner {
    public static class JsonRequestExtensions {
        public static Dictionary<string, object> Parse(this JsonRequest request, out Guid messageId) {
            Dictionary<string, object> data;
            if (string.IsNullOrEmpty(request.json)) {
                data = new Dictionary<string, object>();
            }
            else {
                try {
                    data = VirtualRoot.JsonSerializer.Deserialize<Dictionary<string, object>>(request.json);
                    if (data == null) {
                        data = new Dictionary<string, object>();
                    }
                }
                catch (Exception e) {
                    Write.DevException(e);
                    data = new Dictionary<string, object>();
                }
            }
            messageId = Guid.Empty;
            if (data.TryGetValue("messageId", out object obj)) {
                data.Remove("messageId");
                if (obj != null) {
                    if (!Guid.TryParse(obj.ToString(), out messageId)) {
                        messageId = Guid.Empty;
                    }
                }
            }
            data.Add("__action", request.action);
            return data;
        }

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

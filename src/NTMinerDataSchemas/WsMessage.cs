using System;
using System.Collections.Generic;

namespace NTMiner {
    public class WsMessage : Dictionary<string, Object> {
        public WsMessage() { }

        public WsMessage SetAction(string value) {
            base["action"] = value;
            return this;
        }
        public string GetAction() {
            if (base.TryGetValue("action", out object obj) && obj != null) {
                return obj.ToString();
            }
            return string.Empty;
        }

        public WsMessage SetCode(int value) {
            base["code"] = value;
            return this;
        }
        public int GetCode() {
            if (base.TryGetValue("code", out object obj) && obj != null && int.TryParse(obj.ToString(), out int code)) {
                return code;
            }
            return 0;
        }

        public WsMessage SetPhrase(string value) {
            base["phrase"] = value;
            return this;
        }
        public string GetPhrase() {
            if (base.TryGetValue("phrase", out object obj) && obj != null) {
                return obj.ToString();
            }
            return string.Empty;
        }

        public WsMessage SetDes(string value) {
            base["des"] = value;
            return this;
        }
        public string GetDes() {
            if (base.TryGetValue("des", out object obj) && obj != null) {
                return obj.ToString();
            }
            return string.Empty;
        }

        public WsMessage SetData(object value) {
            base["data"] = value;
            return this;
        }
        public object GetData() {
            if (base.TryGetValue("data", out object obj)) {
                return obj;
            }
            return null;
        }
    }
}

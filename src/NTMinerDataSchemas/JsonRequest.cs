namespace NTMiner {
    public class JsonRequest {
        public JsonRequest() { }
        public JsonRequest(string action, string json) {
            this.action = action;
            this.json = json;
        }

        public string action { get; set; }
        public string json { get; set; }
    }
}

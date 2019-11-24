namespace NTMiner {
    public class JsonResponse {
        public JsonResponse() { }

        public string messageId { get; set; }
        public int code { get; set; }
        public string phrase { get; set; }
        public string des { get; set; }
        public string res { get; set; }
        public object data { get; set; }
    }
}

namespace NTMiner {
    public class JsonResponse {
        public JsonResponse() { }

        public string messageId { get; set; }
        public string action { get; set; }

        public int code { get; set; }
        public string phrase { get; set; }
        public string des { get; set; }

        public object data { get; set; }
    }
}

namespace NTMiner {
    public static class WsMessageExtension {
        public static string ToJson(this WsMessage request) {
            return VirtualRoot.JsonSerializer.Serialize(request);
        }
    }
}

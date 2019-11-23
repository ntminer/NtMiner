namespace NTMiner {
    public static class JsonObjectExtension {
        public static string ToJson(this JsonObject obj) {
            return VirtualRoot.JsonSerializer.Serialize(obj);
        }
    }
}

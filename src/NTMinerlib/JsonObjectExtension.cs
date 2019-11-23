namespace NTMiner {
    public static class JsonObjectExtension {
        public static string ToJson<T>(this JsonObject<T> obj) {
            return VirtualRoot.JsonSerializer.Serialize(obj);
        }
    }
}

namespace NTMiner {
    public static class JsonResponseExtension {
        public static string ToJson(this JsonResponse obj) {
            return VirtualRoot.JsonSerializer.Serialize(obj);
        }
    }
}

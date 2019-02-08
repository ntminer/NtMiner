namespace NTMiner {
    public static class ResponseBaseExtension {
        public static bool IsSuccess(this ResponseBase response) {
            if (response == null) {
                return false;
            }
            return response.StateCode == 200;
        }
    }
}

using System;

namespace NTMiner {
    public static class ResponseBaseExtension {
        public static bool IsSuccess(this ResponseBase response) {
            if (response == null) {
                return false;
            }
            return response.StateCode == 200;
        }

        public static string ReadMessage(this ResponseBase response, Exception e) {
            if (response != null && !string.IsNullOrEmpty(response.Description)) {
                return response.Description;
            }

            if (e != null) {
                return e.Message;
            }

            return string.Empty;
        }
    }
}

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
                return GetInnerException(e).Message;
            }

            return string.Empty;
        }

        // 记录异常时不应直接记录InnerException，因为InnerException的StackTrace上缺少行号
        private static Exception GetInnerException(Exception e) {
            if (e == null) {
                return null;
            }
            while (e.InnerException != null) {
                e = e.InnerException;
            }
            return e;
        }
    }
}

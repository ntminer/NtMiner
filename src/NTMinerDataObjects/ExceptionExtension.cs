using System;

namespace NTMiner {
    public static class ExceptionExtension {
        public static string GetInnerMessage(this Exception e) {
            if (e == null) {
                return string.Empty;
            }
            return GetInnerException(e).Message;
        }

        // 记录异常时不应直接记录InnerException，因为InnerException的StackTrace上缺少行号
        private static Exception GetInnerException(this Exception e) {
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

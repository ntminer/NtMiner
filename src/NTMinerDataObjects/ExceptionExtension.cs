using System;

namespace NTMiner {
    public static class ExceptionExtension {
        public static Exception GetInnerException(this Exception e) {
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

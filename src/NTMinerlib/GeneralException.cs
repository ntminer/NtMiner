
namespace NTMiner {
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class GeneralException : Exception {
        public GeneralException() : base() { }

        public GeneralException(string message) : base(message) { }

        public GeneralException(string message, Exception innerException) : base(message, innerException) { }

        public GeneralException(string format, params object[] args) : base(string.Format(format, args)) { }

        protected GeneralException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected Exception Cause => InnerException;
    }
}

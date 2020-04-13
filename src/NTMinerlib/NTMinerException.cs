
namespace NTMiner {
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class NTMinerException : Exception {
        public NTMinerException() : base() { }

        public NTMinerException(string message) : base(message) { }

        public NTMinerException(string message, Exception innerException) : base(message, innerException) { }

        public NTMinerException(string format, params object[] args) : base(string.Format(format, args)) { }

        protected NTMinerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

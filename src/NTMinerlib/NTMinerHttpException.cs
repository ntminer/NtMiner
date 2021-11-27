
namespace NTMiner {
    using System;
    using System.Net;
    using System.Runtime.Serialization;

    public class NTMinerHttpException : Exception {
        public NTMinerHttpException() : base() { }

        public NTMinerHttpException(string message) : base(message) { }

        public NTMinerHttpException(string message, Exception innerException) : base(message, innerException) { }

        public NTMinerHttpException(string format, params object[] args) : base(string.Format(format, args)) { }

        protected NTMinerHttpException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public HttpStatusCode StatusCode { get; set; }

        public string ReasonPhrase { get; set; }
    }
}

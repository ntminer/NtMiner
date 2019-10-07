using System;

namespace NTWebSocket {
    public class ConnectionNotAvailableException : Exception {
        public ConnectionNotAvailableException() : base() {
        }

        public ConnectionNotAvailableException(string message) : base(message) {
        }

        public ConnectionNotAvailableException(string message, Exception innerException)
            : base(message, innerException) {
        }
    }
}

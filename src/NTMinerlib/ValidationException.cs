
namespace NTMiner {
    using System;

    [Serializable]
    public class ValidationException : NTMinerException {
        public ValidationException(string message) : base(message) { }
    }
}

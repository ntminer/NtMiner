
namespace NTMiner {
    using System;

    [Serializable]
    public class ValidationException : GeneralException {
        public ValidationException(string message) : base(message) { }
    }
}

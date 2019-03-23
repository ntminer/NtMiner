using System;

namespace NTMiner {
    public interface ISignatureRequest : IGetSignData {
        string LoginName { get; }
        DateTime Timestamp { get; }
        string Sign { get; }
        string GetSign(string password);
    }
}

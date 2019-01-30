using System;

namespace NTMiner.ServiceContracts.ControlCenter.DataObjects {
    public interface ISignatureRequest {
        Guid MessageId { get; }
        string LoginName { get; }
        DateTime Timestamp { get; }
        string Sign { get; }
        string GetSign(string password);
    }
}

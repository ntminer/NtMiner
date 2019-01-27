namespace NTMiner.ServiceContracts.DataObjects {
    public interface ISignatureRequest {
        string LoginName { get; }
        string Sign { get; }
        string GetSign(string password);
    }
}

namespace NTMiner.Controllers {
    public interface IMinerStudioController : IShowMainWindow {
        ResponseBase CloseMinerStudio(SignatureRequest request);
    }
}

namespace NTMiner.Controllers {
    public interface IMinerStudioController : IShowMainWindow {
        ResponseBase CloseMinerStudio(SignRequest request);
    }
}

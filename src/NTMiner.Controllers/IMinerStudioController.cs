namespace NTMiner.Controllers {
    public interface IMinerStudioController {
        bool ShowMainWindow();
        ResponseBase CloseMinerStudio(SignRequest request);
    }
}

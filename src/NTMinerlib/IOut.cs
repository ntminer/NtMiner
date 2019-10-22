namespace NTMiner {
    public interface IOut {
        void ShowError(string message, int? delaySeconds = null);
        void ShowInfo(string message);
        void ShowWarn(string message, int? delaySeconds = null);
        void ShowSuccess(string message, string header = "成功");
    }
}

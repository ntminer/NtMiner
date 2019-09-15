namespace NTMiner {
    public interface IOut {
        void ShowErrorMessage(string message, int? delaySeconds = null);
        void ShowInfo(string message);
        void ShowSuccessMessage(string message, string header = "成功");
    }
}

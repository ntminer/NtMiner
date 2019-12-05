namespace NTMiner {
    public interface IOut {
        void ShowError(string message, int delaySeconds = 0);
        void ShowInfo(string message, int delaySeconds = 4);
        void ShowWarn(string message, int delaySeconds = 0);
        void ShowSuccess(string message, int delaySeconds = 4, string header = "成功");
    }
}

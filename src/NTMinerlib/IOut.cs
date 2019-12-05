namespace NTMiner {
    public interface IOut {
        /// <param name="autoHideSeconds">0表示不自动隐藏，即需要手动点击"知道了"按钮隐藏</param>
        void ShowError(string message, int autoHideSeconds = 0);
        /// <param name="autoHideSeconds">0表示不自动隐藏，即需要手动点击"知道了"按钮隐藏</param>
        void ShowInfo(string message, int autoHideSeconds = 4);
        /// <param name="autoHideSeconds">0表示不自动隐藏，即需要手动点击"知道了"按钮隐藏</param>
        void ShowWarn(string message, int autoHideSeconds = 0);
        /// <param name="autoHideSeconds">0表示不自动隐藏，即需要手动点击"知道了"按钮隐藏</param>
        void ShowSuccess(string message, int autoHideSeconds = 4, string header = "成功");
    }
}

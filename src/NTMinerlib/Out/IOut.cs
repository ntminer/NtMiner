namespace NTMiner.Out {
    public interface IOut {
        /// <param name="autoHideSeconds">0表示不自动隐藏，即需要手动点击"知道了"按钮隐藏</param>
        void ShowError(string message, string header = "错误", int autoHideSeconds = 0);
        /// <param name="autoHideSeconds">0表示不自动隐藏，即需要手动点击"知道了"按钮隐藏</param>
        void ShowInfo(string message, string header = "信息", int autoHideSeconds = 4);
        /// <param name="autoHideSeconds">0表示不自动隐藏，即需要手动点击"知道了"按钮隐藏</param>
        void ShowWarn(string message, string header = "警告", int autoHideSeconds = 0);
        /// <param name="autoHideSeconds">0表示不自动隐藏，即需要手动点击"知道了"按钮隐藏</param>
        void ShowSuccess(string message, string header = "成功", int autoHideSeconds = 4);
    }
}

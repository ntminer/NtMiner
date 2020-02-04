namespace NTMiner.Out {
    public interface IOut {
        /// <summary>
        /// autoHideSeconds 0表示不自动隐藏，即需要手动点击"知道了"按钮隐藏
        /// </summary>
        void ShowError(string message, string header = "错误", int autoHideSeconds = 0);

        /// <summary>
        /// autoHideSeconds 0表示不自动隐藏，即需要手动点击"知道了"按钮隐藏
        /// </summary>
        void ShowInfo(string message, string header = "信息", int autoHideSeconds = 4);

        /// <summary>
        /// autoHideSeconds 0表示不自动隐藏，即需要手动点击"知道了"按钮隐藏
        /// </summary>
        void ShowWarn(string message, string header = "警告", int autoHideSeconds = 0);

        /// <summary>
        /// autoHideSeconds 0表示不自动隐藏，即需要手动点击"知道了"按钮隐藏
        /// </summary>
        void ShowSuccess(string message, string header = "成功", int autoHideSeconds = 4);
    }
}

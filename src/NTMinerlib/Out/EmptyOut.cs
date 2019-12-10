namespace NTMiner.Out {
    public class EmptyOut : IOut {
        public static readonly EmptyOut Instance = new EmptyOut();

        private EmptyOut() { }

        public void ShowError(string message, int autoHideSeconds) {
            // nothing need todo
        }

        public void ShowInfo(string message, int autoHideSeconds) {
            // nothing need todo
        }

        public void ShowSuccess(string message, int autoHideSeconds, string header = "成功") {
            // nothing need todo
        }

        public void ShowWarn(string message, int autoHideSeconds) {
            // nothing need todo
        }
    }
}

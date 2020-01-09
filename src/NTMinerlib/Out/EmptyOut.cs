namespace NTMiner.Out {
    public class EmptyOut : IOut {
        public static readonly EmptyOut Instance = new EmptyOut();

        private EmptyOut() { }

        public void ShowError(string message, string header, int autoHideSeconds) {
            // nothing need todo
        }

        public void ShowInfo(string message, string header, int autoHideSeconds) {
            // nothing need todo
        }

        public void ShowSuccess(string message, string header, int autoHideSeconds) {
            // nothing need todo
        }

        public void ShowWarn(string message, string header, int autoHideSeconds) {
            // nothing need todo
        }
    }
}

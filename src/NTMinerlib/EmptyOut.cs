namespace NTMiner {

    public class EmptyOut : IOut {
        public static readonly EmptyOut Instance = new EmptyOut();

        private EmptyOut() { }

        public void ShowErrorMessage(string message, int? delaySeconds = null) {
            // nothing need todo
        }

        public void ShowInfo(string message) {
            // nothing need todo
        }

        public void ShowSuccessMessage(string message, string header = "成功") {
            // nothing need todo
        }
    }
}

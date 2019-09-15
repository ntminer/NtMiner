namespace NTMiner {
    public interface IShowMessage {
        void ShowErrorMessage(string message, int? delaySeconds = null);
        void ShowInfo(string message);
        void ShowSuccessMessage(string message, string header = "成功");
    }

    public class EmptyShowMessage : IShowMessage {
        public static readonly EmptyShowMessage Instance = new EmptyShowMessage();

        private EmptyShowMessage() { }

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

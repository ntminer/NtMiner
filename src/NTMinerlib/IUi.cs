namespace NTMiner {
    public interface IUi {
        void ShowErrorMessage(string message, int? delaySeconds = null);
        void ShowInfo(string message);
        void ShowSuccessMessage(string message, string header = "成功");
    }

    public class EmptyUi : IUi {
        public static readonly EmptyUi Instance = new EmptyUi();

        private EmptyUi() { }

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

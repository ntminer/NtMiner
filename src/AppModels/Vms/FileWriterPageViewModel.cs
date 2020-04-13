namespace NTMiner.Vms {
    public class FileWriterPageViewModel : ViewModelBase {
        public FileWriterPageViewModel() { }

        public AppRoot.FileWriterViewModels FileWriterVms {
            get {
                return AppRoot.FileWriterViewModels.Instance;
            }
        }
    }
}

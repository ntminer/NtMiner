namespace NTMiner.Vms {
    public class FileWriterPageViewModel : ViewModelBase {
        public FileWriterPageViewModel() { }

        public AppContext.FileWriterViewModels FileWriterVms {
            get {
                return AppContext.FileWriterViewModels.Instance;
            }
        }
    }
}

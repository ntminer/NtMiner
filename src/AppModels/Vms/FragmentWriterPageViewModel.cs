namespace NTMiner.Vms {
    public class FragmentWriterPageViewModel : ViewModelBase {
        public FragmentWriterPageViewModel() { }

        public AppContext.FragmentWriterViewModels FragmentWriterVms {
            get {
                return AppContext.FragmentWriterViewModels.Instance;
            }
        }
    }
}

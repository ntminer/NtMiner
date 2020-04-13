namespace NTMiner.Vms {
    public class FragmentWriterPageViewModel : ViewModelBase {
        public FragmentWriterPageViewModel() { }

        public AppRoot.FragmentWriterViewModels FragmentWriterVms {
            get {
                return AppRoot.FragmentWriterViewModels.Instance;
            }
        }
    }
}

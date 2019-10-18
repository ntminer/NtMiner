namespace NTMiner.Vms {
    using System.Collections.ObjectModel;
    using System.Linq;

    public class WorkerEventsViewModel : ViewModelBase {
        private readonly ObservableCollection<WorkerEventViewModel> _workerEventVms;

        public WorkerEventsViewModel() {
            var data = VirtualRoot.WorkerEvents.GetEvents().OrderByDescending(a => a.Id).Select(a => new WorkerEventViewModel(a));
            _workerEventVms = new ObservableCollection<WorkerEventViewModel>(data);
            _selectedChannel = WorkerEventChannel.Unspecified.GetEnumItem();
        }

        private EnumItem<WorkerEventChannel> _selectedChannel;
        public EnumItem<WorkerEventChannel> SelectedChannel {
            get => _selectedChannel;
            set {
                if (_selectedChannel != value) {
                    _selectedChannel = value;
                    OnPropertyChanged(nameof(SelectedChannel));
                }
            }
        }

        public ObservableCollection<WorkerEventViewModel> WorkerEventVms {
            get {
                return _workerEventVms;
            }
        }
    }
}

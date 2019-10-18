namespace NTMiner.Vms {
    using System.Collections.ObjectModel;
    using System.Linq;

    public class WorkerEventsViewModel : ViewModelBase {
        private readonly ObservableCollection<WorkerEventViewModel> _workerEventVms;

        public WorkerEventsViewModel() {
            var data = VirtualRoot.WorkerEvents.GetEvents(WorkerEventChannel.Undefined, string.Empty).OrderByDescending(a => a.Id).Select(a => new WorkerEventViewModel(a));
            _workerEventVms = new ObservableCollection<WorkerEventViewModel>(data);
        }

        public ObservableCollection<WorkerEventViewModel> WorkerEventVms {
            get {
                return _workerEventVms;
            }
        }
    }
}

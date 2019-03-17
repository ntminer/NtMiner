namespace NTMiner.Vms {
    public class EventPageViewModel : ViewModelBase {
        public EventPageViewModel() {
        }

        public HandlerIdViewModels HandlerIdVms {
            get {
                return HandlerIdViewModels.Current;
            }
        }
    }
}

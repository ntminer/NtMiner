namespace NTMiner.Vms {
    public class EventPageViewModel : ViewModelBase {
        public EventPageViewModel() {
        }

        public Vm Vm {
            get {
                return Vm.Instance;
            }
        }
    }
}

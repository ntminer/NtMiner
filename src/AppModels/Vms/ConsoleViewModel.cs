using System.Windows.Input;

namespace NTMiner.Vms {
    public class ConsoleViewModel : ViewModelBase {
        public ICommand CustomTheme { get; private set; }
        public ConsoleViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.CustomTheme = new DelegateCommand(() => {
                VirtualRoot.Execute(new ShowLogColorCommand());
            });
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return AppContext.Instance.MinerProfileVm;
            }
        }
    }
}

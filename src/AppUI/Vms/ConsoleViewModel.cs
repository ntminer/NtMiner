using NTMiner.Views.Ucs;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ConsoleViewModel : ViewModelBase {
        public static readonly ConsoleViewModel Current = new ConsoleViewModel();

        public ICommand CustomTheme { get; private set; }
        private ConsoleViewModel() {
            this.CustomTheme = new DelegateCommand(() => {
                LogColor.ShowWindow();
            });
        }

        public bool IsTranslate {
            get {
                return NTMinerRoot.IsTranslate;
            }
            set {
                NTMinerRoot.IsTranslate = value;
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }
    }
}

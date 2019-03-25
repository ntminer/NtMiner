using NTMiner.Views.Ucs;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ConsoleViewModel : ViewModelBase {
        public static readonly ConsoleViewModel Current = new ConsoleViewModel();

        public ICommand CustomTheme { get; private set; }
        private ConsoleViewModel() {
            this.CustomTheme = new DelegateCommand(() => {
                LogColor.ShowWindow();
            });
            VirtualRoot.On<MineStartedEvent>("挖矿开始后因此日志窗口的水印", LogEnum.DevConsole,
                action: message => {
                    this.IsWatermarkVisible = Visibility.Collapsed;
                });
        }

        private Visibility _isWatermarkVisible = Visibility.Visible;
        public Visibility IsWatermarkVisible {
            get {
                return _isWatermarkVisible;
            }
            set {
                if (_isWatermarkVisible != value) {
                    _isWatermarkVisible = value;
                    OnPropertyChanged(nameof(IsWatermarkVisible));
                }
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }
    }
}

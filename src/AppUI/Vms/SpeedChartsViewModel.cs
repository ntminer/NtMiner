using System.Windows.Input;

namespace NTMiner.Vms {
    public class SpeedChartsViewModel : ViewModelBase {
        private SpeedChartViewModel _currentSpeedChartVm;
        private int _itemsPanelColumns = 2;
        private readonly SpeedChartViewModels _speedChartViewModels;

        public ICommand ChangeCurrentSpeedChartVm { get; private set; }

        public SpeedChartsViewModel() {
            _speedChartViewModels = new SpeedChartViewModels();
            this.ChangeCurrentSpeedChartVm = new DelegateCommand<SpeedChartViewModel>((speedChartVm) =>
            {
                SetCurrentSpeedChartVm(speedChartVm);
            });
        }

        public void SetCurrentSpeedChartVm(SpeedChartViewModel speedChartVm) {
            if (this.CurrentSpeedChartVm != null) {
                this.CurrentSpeedChartVm.SetDefaultBackground();
            }
            this.CurrentSpeedChartVm = speedChartVm;
            if (speedChartVm != null) {
                this.CurrentSpeedChartVm.SetSelectedBackground();
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }

        public SpeedChartViewModel CurrentSpeedChartVm {
            get => _currentSpeedChartVm;
            set {
                if (_currentSpeedChartVm != value) {
                    _currentSpeedChartVm = value;
                    OnPropertyChanged(nameof(CurrentSpeedChartVm));
                }
            }
        }

        public SpeedChartViewModels SpeedChartVms {
            get {
                return _speedChartViewModels;
            }
        }

        public int ItemsPanelColumns {
            get => _itemsPanelColumns;
            set {
                if (_itemsPanelColumns != value) {
                    _itemsPanelColumns = value;
                    OnPropertyChanged(nameof(ItemsPanelColumns));
                }
            }
        }
    }
}

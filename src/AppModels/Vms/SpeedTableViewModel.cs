using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class SpeedTableViewModel : ViewModelBase {
        private Visibility _isOverClockVisible = Visibility.Collapsed;
        private DataGridRowDetailsVisibilityMode _rowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
        private string _btnOverClockVisibleName = "显示超频";

        public ICommand VisibleOverClock { get; private set; }

        public SpeedTableViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.VisibleOverClock = new DelegateCommand(() => {
                if (RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Collapsed) {
                    BtnOverClockVisibleName = "隐藏超频";
                    IsOverClockVisible = Visibility.Visible;
                    RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Visible;
                }
                else if (RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Visible) {
                    BtnOverClockVisibleName = "显示超频";
                    IsOverClockVisible = Visibility.Collapsed;
                    RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                }
            });
        }

        public Visibility IsACardVisible {
            get {
                if (NTMinerRoot.Instance.GpuSet.GpuType == GpuType.AMD) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public Visibility IsOverClockVisible {
            get { return _isOverClockVisible; }
            set {
                _isOverClockVisible = value;
                OnPropertyChanged(nameof(IsOverClockVisible));
            }
        }

        public DataGridRowDetailsVisibilityMode RowDetailsVisibilityMode {
            get { return _rowDetailsVisibilityMode; }
            set {
                _rowDetailsVisibilityMode = value;
                OnPropertyChanged(nameof(RowDetailsVisibilityMode));
            }
        }

        public string BtnOverClockVisibleName {
            get { return _btnOverClockVisibleName; }
            set {
                _btnOverClockVisibleName = value;
                OnPropertyChanged(nameof(BtnOverClockVisibleName));
            }
        }

        public AppContext.GpuSpeedViewModels GpuSpeedVms {
            get {
                return AppContext.GpuSpeedViewModels.Instance;
            }
        }
    }
}

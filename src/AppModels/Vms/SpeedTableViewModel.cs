using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class SpeedTableViewModel : ViewModelBase {
        private DataGridRowDetailsVisibilityMode _rowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
        private string _btnOverClockVisibleName = "显示单卡超频";

        public ICommand VisibleOverClock { get; private set; }

        public SpeedTableViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.VisibleOverClock = new DelegateCommand(() => {
                if (RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Collapsed) {
                    BtnOverClockVisibleName = "隐藏单卡超频";
                    RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Visible;
                }
                else if (RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Visible) {
                    BtnOverClockVisibleName = "显示单卡超频";
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

using NTMiner.Notifications;
using NTMiner.Views.Ucs;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ChartsWindowViewModel : ViewModelBase {
        public static readonly ChartsWindowViewModel Current = new ChartsWindowViewModel();

        private INotificationMessageManager _manager;
        private double _height;
        private double _width;
        private List<ChartViewModel> _chartVms;
        private int _totalMiningCount;
        private int _totalOnlineCount;

        public ICommand ConfigMinerServerHost { get; private set; }

        private ChartsWindowViewModel() {
            this.Width = SystemParameters.FullPrimaryScreenWidth * 0.95;
            this.Height = SystemParameters.FullPrimaryScreenHeight * 0.95;
            this.ConfigMinerServerHost = new DelegateCommand(() => {
                MinerServerHostConfig.ShowWindow();
            });
        }

        public double Height {
            get => _height;
            set => _height = value;
        }

        public double Width {
            get => _width;
            set => _width = value;
        }

        public LangViewModels LangVms {
            get {
                return LangViewModels.Current;
            }
        }

        public INotificationMessageManager Manager {
            get {
                if (_manager == null) {
                    _manager = new NotificationMessageManager();
                }
                return _manager;
            }
        }

        public int TotalMiningCount {
            get => _totalMiningCount;
            set {
                if (_totalMiningCount != value) {
                    _totalMiningCount = value;
                    OnPropertyChanged(nameof(TotalMiningCount));
                }
            }
        }

        public int TotalOnlineCount {
            get => _totalOnlineCount;
            set {
                if (_totalOnlineCount != value) {
                    _totalOnlineCount = value;
                    OnPropertyChanged(nameof(TotalOnlineCount));
                }
            }
        }

        public List<ChartViewModel> ChartVms {
            get {
                if (_chartVms == null) {
                    _chartVms = new List<ChartViewModel>();
                    foreach (var coinVm in MinerClientsWindowViewModel.Current.MineCoinVms.AllCoins.OrderBy(a => a.SortNumber)) {
                        _chartVms.Add(new ChartViewModel(coinVm));
                    }
                }
                return _chartVms;
            }
        }
    }
}

using NTMiner.Core;
using NTMiner.OverClock;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class OverClockDataViewModel : ViewModelBase, IOverClockData, IEditableViewModel {
        private Guid _id;
        private Guid _coinId;
        private string _name;
        private int _coreClockDelta;
        private int _memoryClockDelta;
        private int _powerCapacity;
        private int _cool;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public OverClockDataViewModel() {
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public OverClockDataViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                IOverClockData group;
                if (NTMinerRoot.Current.OverClockDataSet.TryGetOverClockData(this.Id, out group)) {
                    VirtualRoot.Execute(new UpdateOverClockDataCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddOverClockDataCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand<FormType>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                OverClockDataEdit.ShowWindow(this);
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.Name}吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveOverClockDataCommand(this.Id));
                }, icon: "Icon_Confirm");
            });
        }

        public OverClockDataViewModel(IOverClockData data) : this(data.GetId()) {
            _coinId = data.CoinId;
            _name = data.Name;
            _coreClockDelta = data.CoreClockDelta;
            _memoryClockDelta = data.MemoryClockDelta;
            _powerCapacity = data.PowerCapacity;
            _cool = data.Cool;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public Guid CoinId {
            get => _coinId;
            set {
                _coinId = value;
                OnPropertyChanged(nameof(CoinId));
            }
        }

        public string Name {
            get => _name;
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public int CoreClockDelta {
            get => _coreClockDelta;
            set {
                _coreClockDelta = value;
                OnPropertyChanged(nameof(CoreClockDelta));
            }
        }

        public int MemoryClockDelta {
            get => _memoryClockDelta;
            set {
                _memoryClockDelta = value;
                OnPropertyChanged(nameof(MemoryClockDelta));
            }
        }

        public int PowerCapacity {
            get => _powerCapacity;
            set {
                _powerCapacity = value;
                OnPropertyChanged(nameof(PowerCapacity));
            }
        }

        public int Cool {
            get => _cool;
            set {
                _cool = value;
                OnPropertyChanged(nameof(Cool));
            }
        }

        private CoinViewModel _coinVm;
        public CoinViewModel CoinVm {
            get {
                if (_coinVm == null) {
                    if (!CoinViewModels.Current.TryGetCoinVm(this.CoinId, out _coinVm)) {
                        _coinVm = CoinViewModel.Empty;
                    }
                }
                return _coinVm;
            }
        }
    }
}

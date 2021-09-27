using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class SysDicPageViewModel : ViewModelBase {
        public ICommand ClearAlgo { get; private set; }

        public SysDicPageViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.ClearAlgo = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定清理未使用的算法字典项吗？", title: "确认", onYes: () => {
                    var items = NTMinerContext.Instance.ServerContext.SysDicItemSet.GetSysDicItems("Algo");
                    if (items != null) {
                        var toClearItemIds = items.Where(a => NTMinerContext.Instance.ServerContext.CoinSet.AsEnumerable().All(c => c.AlgoId != a.GetId())).Select(a => a.GetId()).ToArray();
                        foreach (var id in toClearItemIds) {
                            VirtualRoot.Execute(new RemoveSysDicItemCommand(id));
                        }
                    }
                }));
            });
            this._currentSysDic = SysDicVms.List.FirstOrDefault();
        }

        private SysDicViewModel _currentSysDic;
        public SysDicViewModel CurrentSysDic {
            get { return _currentSysDic; }
            set {
                if (_currentSysDic != value) {
                    _currentSysDic = value;
                    OnPropertyChanged(nameof(CurrentSysDic));
                    OnPropertyChanged(nameof(IsAlgoDic));
                }
            }
        }

        public bool IsAlgoDic {
            get {
                if (CurrentSysDic == null) {
                    return false;
                }
                return CurrentSysDic.Code == "Algo";
            }
        }

        private SysDicItemViewModel _currentSysDicItem;
        public SysDicItemViewModel CurrentSysDicItem {
            get { return _currentSysDicItem; }
            set {
                _currentSysDicItem = value;
                OnPropertyChanged(nameof(CurrentSysDicItem));
            }
        }

        public AppRoot.SysDicViewModels SysDicVms {
            get {
                return AppRoot.SysDicVms;
            }
        }
    }
}

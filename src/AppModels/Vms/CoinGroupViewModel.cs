using NTMiner.Core;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class CoinGroupViewModel : ViewModelBase, ICoinGroup, ISortable {
        private Guid _id;
        private Guid _groupId;
        private Guid _coinId;
        private int _sortNumber;

        public ICommand Remove { get; private set; }
        public ICommand SortUp { get; private set; }
        public ICommand SortDown { get; private set; }

        public CoinGroupViewModel(ICoinGroup data) : this(data.GetId()) {
            _groupId = data.GroupId;
            _coinId = data.CoinId;
            _sortNumber = data.SortNumber;
        }

        public CoinGroupViewModel(Guid id) {
            _id = id;
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{CoinVm.Code}吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveCoinGroupCommand(this.Id));
                }));
            });
            this.SortUp = new DelegateCommand(() => {
                CoinGroupViewModel upOne = AppContext.Instance.CoinGroupVms.GetCoinGroupsByGroupId(this.GroupId).GetUpOne(this.SortNumber);
                if (upOne != null) {
                    int sortNumber = upOne.SortNumber;
                    upOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateCoinGroupCommand(upOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateCoinGroupCommand(this));
                    if (AppContext.Instance.GroupVms.TryGetGroupVm(this.GroupId, out GroupViewModel groupVm)) {
                        groupVm.OnPropertyChanged(nameof(groupVm.CoinGroupVms));
                    }
                }
            });
            this.SortDown = new DelegateCommand(() => {
                CoinGroupViewModel nextOne = AppContext.Instance.CoinGroupVms.GetCoinGroupsByGroupId(this.GroupId).GetNextOne(this.SortNumber);
                if (nextOne != null) {
                    int sortNumber = nextOne.SortNumber;
                    nextOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateCoinGroupCommand(nextOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateCoinGroupCommand(this));
                    if (AppContext.Instance.GroupVms.TryGetGroupVm(this.GroupId, out GroupViewModel groupVm)) {
                        groupVm.OnPropertyChanged(nameof(groupVm.CoinGroupVms));
                    }
                }
            });
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            set {
                if (_id != value) {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public Guid GroupId {
            get => _groupId;
            set {
                if (_groupId != value) {
                    _groupId = value;
                    OnPropertyChanged(nameof(GroupId));
                }
            }
        }

        public Guid CoinId {
            get => _coinId;
            set {
                if (_coinId != value) {
                    _coinId = value;
                    OnPropertyChanged(nameof(CoinId));
                }
            }
        }

        public CoinViewModel CoinVm {
            get {
                if (AppContext.Instance.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel vm)) {
                    return vm;
                }
                return CoinViewModel.Empty;
            }
        }

        public int SortNumber {
            get => _sortNumber;
            set {
                if (_sortNumber != value) {
                    _sortNumber = value;
                    OnPropertyChanged(nameof(SortNumber));
                }
            }
        }
    }
}

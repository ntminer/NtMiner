using NTMiner.Core;
using NTMiner.Views;
using System;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class CoinGroupViewModel : ViewModelBase, ICoinGroup {
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
                DialogWindow.ShowDialog(message: $"您确定删除{CoinVm.Code}吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveCoinGroupCommand(this.Id));
                }, icon: IconConst.IconConfirm);
            });
            this.SortUp = new DelegateCommand(() => {
                CoinGroupViewModel upOne = CoinGroupViewModels.Current.GetCoinGroupsByGroupId(this.GroupId).OrderByDescending(a => a.SortNumber).FirstOrDefault(a => a.SortNumber < this.SortNumber);
                if (upOne != null) {
                    int sortNumber = upOne.SortNumber;
                    upOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateCoinGroupCommand(upOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateCoinGroupCommand(this));
                    GroupViewModel groupVm;
                    if (GroupViewModels.Current.TryGetGroupVm(this.GroupId, out groupVm)) {
                        groupVm.OnPropertyChanged(nameof(groupVm.CoinGroupVms));
                    }
                }
            });
            this.SortDown = new DelegateCommand(() => {
                CoinGroupViewModel nextOne = CoinGroupViewModels.Current.GetCoinGroupsByGroupId(this.GroupId).OrderBy(a => a.SortNumber).FirstOrDefault(a => a.SortNumber > this.SortNumber);
                if (nextOne != null) {
                    int sortNumber = nextOne.SortNumber;
                    nextOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateCoinGroupCommand(nextOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateCoinGroupCommand(this));
                    GroupViewModel groupVm;
                    if (GroupViewModels.Current.TryGetGroupVm(this.GroupId, out groupVm)) {
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
                CoinViewModel vm;
                if (CoinViewModels.Current.TryGetCoinVm(this.CoinId, out vm)) {
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

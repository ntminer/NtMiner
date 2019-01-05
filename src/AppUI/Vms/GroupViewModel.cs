using NTMiner.Core;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class GroupViewModel : ViewModelBase, IGroup {
        public static readonly GroupViewModel PleaseSelect = new GroupViewModel(Guid.Empty) {
            _name = "无",
            _sortNumber = 0
        };

        private Guid _id;
        private string _name;
        private int _sortNumber;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand SortUp { get; private set; }
        public ICommand SortDown { get; private set; }
        public ICommand AddCoinGroup { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public GroupViewModel(IGroup data) : this(data.GetId()) {
            _name = data.Name;
            _sortNumber = data.SortNumber;
        }

        public GroupViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (NTMinerRoot.Current.GroupSet.Contains(this.Id)) {
                    Global.Execute(new UpdateGroupCommand(this));
                }
                else {
                    Global.Execute(new AddGroupCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand(() => {
                GroupEdit.ShowEditWindow(this);
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.Name}组吗？", title: "确认", onYes: () => {
                    Global.Execute(new RemoveGroupCommand(this.Id));
                }, icon: "Icon_Confirm");
            });
            this.SortUp = new DelegateCommand(() => {
                GroupViewModel upOne = GroupViewModels.Current.List.OrderByDescending(a => a.SortNumber).FirstOrDefault(a => a.SortNumber < this.SortNumber);
                if (upOne != null) {
                    int sortNumber = upOne.SortNumber;
                    upOne.SortNumber = this.SortNumber;
                    Global.Execute(new UpdateGroupCommand(upOne));
                    this.SortNumber = sortNumber;
                    Global.Execute(new UpdateGroupCommand(this));
                    GroupViewModels.Current.OnPropertyChanged(nameof(GroupViewModels.List));
                }
            });
            this.SortDown = new DelegateCommand(() => {
                GroupViewModel nextOne = GroupViewModels.Current.List.OrderBy(a => a.SortNumber).FirstOrDefault(a => a.SortNumber > this.SortNumber);
                if (nextOne != null) {
                    int sortNumber = nextOne.SortNumber;
                    nextOne.SortNumber = this.SortNumber;
                    Global.Execute(new UpdateGroupCommand(nextOne));
                    this.SortNumber = sortNumber;
                    Global.Execute(new UpdateGroupCommand(this));
                    GroupViewModels.Current.OnPropertyChanged(nameof(GroupViewModels.List));
                }
            });
            this.AddCoinGroup = new DelegateCommand<CoinViewModel>((coinVm) => {
                if (coinVm == null) {
                    return;
                }
                var coinGroupVms = CoinGroupViewModels.Current.GetCoinGroupsByGroupId(this.Id);
                int sortNumber = coinGroupVms.Count == 0 ? 1 : coinGroupVms.Count + 1;
                CoinGroupViewModel coinGroupVm = new CoinGroupViewModel(Guid.NewGuid()) {
                    CoinId = coinVm.Id,
                    GroupId = this.Id,
                    SortNumber = sortNumber
                };
                Global.Execute(new AddCoinGroupCommand(coinGroupVm));
            });
        }

        public List<CoinGroupViewModel> CoinGroupVms {
            get {
                return CoinGroupViewModels.Current.GetCoinGroupsByGroupId(this.Id).OrderBy(a => a.SortNumber).ToList();
            }
        }

        public List<CoinViewModel> CoinVms {
            get {
                List<CoinViewModel> list = new List<CoinViewModel>();
                var coinGroupVms = CoinGroupViewModels.Current.GetCoinGroupsByGroupId(this.Id);
                foreach (var item in CoinViewModels.Current.AllCoins) {
                    if (coinGroupVms.All(a => a.CoinId != item.Id)) {
                        list.Add(item);
                    }
                }
                return list.OrderBy(a => a.SortNumber).ToList();
            }
        }

        public List<CoinViewModel> DualCoinVms {
            get {
                var coinGroupVms = CoinGroupViewModels.Current.GetCoinGroupsByGroupId(this.Id);
                return coinGroupVms.Where(a => a.CoinVm != CoinViewModel.Empty).Select(a => a.CoinVm).OrderBy(a => a.SortNumber).ToList();
            }
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

        public string Name {
            get => _name;
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public int SortNumber {
            get => _sortNumber;
            set {
                _sortNumber = value;
                OnPropertyChanged(nameof(SortNumber));
            }
        }
    }
}

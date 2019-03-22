using NTMiner.Core;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class GroupViewModel : ViewModelBase, IGroup, IEditableViewModel {
        public static readonly GroupViewModel PleaseSelect = new GroupViewModel(Guid.Empty) {
            _name = "由下级决定",
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
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (NTMinerRoot.Current.GroupSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdateGroupCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddGroupCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                GroupEdit.ShowWindow(formType ?? FormType.Edit, this);
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.Name}组吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveGroupCommand(this.Id));
                }, icon: IconConst.IconConfirm);
            });
            this.SortUp = new DelegateCommand(() => {
                GroupViewModel upOne = GroupViewModels.Current.List.OrderByDescending(a => a.SortNumber).FirstOrDefault(a => a.SortNumber < this.SortNumber);
                if (upOne != null) {
                    int sortNumber = upOne.SortNumber;
                    upOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateGroupCommand(upOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateGroupCommand(this));
                    GroupViewModels.Current.OnPropertyChanged(nameof(GroupViewModels.List));
                }
            });
            this.SortDown = new DelegateCommand(() => {
                GroupViewModel nextOne = GroupViewModels.Current.List.OrderBy(a => a.SortNumber).FirstOrDefault(a => a.SortNumber > this.SortNumber);
                if (nextOne != null) {
                    int sortNumber = nextOne.SortNumber;
                    nextOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateGroupCommand(nextOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateGroupCommand(this));
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
                VirtualRoot.Execute(new AddCoinGroupCommand(coinGroupVm));
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
                if (_id != value) {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Name {
            get => _name;
            set {
                if (_name != value) {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("名称不能为空");
                    }
                }
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

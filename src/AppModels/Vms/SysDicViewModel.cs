using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class SysDicViewModel : ViewModelBase, ISysDic, IEditableViewModel {
        private Guid _id;
        private string _code;
        private string _name;
        private string _description;
        private int _sortNumber;
        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand AddSysDicItem { get; private set; }
        public ICommand SortUp { get; private set; }
        public ICommand SortDown { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public Guid GetId() {
            return this.Id;
        }

        public SysDicViewModel(ISysDic data) : this(data.GetId()) {
            _code = data.Code;
            _name = data.Name;
            _description = data.Description;
            _sortNumber = data.SortNumber;
        }

        public SysDicViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (NTMinerRoot.Instance.ServerContext.SysDicSet.ContainsKey(this.Id)) {
                    VirtualRoot.Execute(new UpdateSysDicCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddSysDicCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.AddSysDicItem = new DelegateCommand(() => {
                new SysDicItemViewModel(Guid.NewGuid()) {
                    DicId = id,
                    SortNumber = this.SysDicItems.Count + 1
                }.Edit.Execute(FormType.Add);
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                VirtualRoot.Execute(new SysDicEditCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{this.Code}系统字典吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveSysDicCommand(this.Id));
                }));
            });
            this.SortUp = new DelegateCommand(() => {
                SysDicViewModel upOne = AppContext.Instance.SysDicVms.GetUpOne(this.SortNumber);
                if (upOne != null) {
                    int sortNumber = upOne.SortNumber;
                    upOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateSysDicCommand(upOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateSysDicCommand(this));
                    AppContext.Instance.SysDicVms.OnPropertyChanged(nameof(AppContext.SysDicViewModels.List));
                }
            });
            this.SortDown = new DelegateCommand(() => {
                SysDicViewModel nextOne = AppContext.Instance.SysDicVms.GetNextOne(this.SortNumber);
                if (nextOne != null) {
                    int sortNumber = nextOne.SortNumber;
                    nextOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateSysDicCommand(nextOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateSysDicCommand(this));
                    AppContext.Instance.SysDicVms.OnPropertyChanged(nameof(AppContext.SysDicViewModels.List));
                }
            });
        }

        public Guid Id {
            get => _id;
            private set {
                if (_id != value) {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Code {
            get => _code;
            set {
                if (_code != value) {
                    _code = value;
                    OnPropertyChanged(nameof(Code));
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("编码是必须的");
                    }
                    if (AppContext.Instance.SysDicVms.List.Any(a => a.Code == value && a.Id != this.Id)) {
                        throw new ValidationException("编码重复");
                    }
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
                        throw new ValidationException("名称是必须的");
                    }
                    if (AppContext.Instance.SysDicVms.List.Any(a => a.Name == value && a.Id != this.Id)) {
                        throw new ValidationException("名称重复");
                    }
                }
            }
        }

        public string Description {
            get => _description;
            set {
                if (_description != value) {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
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

        public List<SysDicItemViewModel> SysDicItems {
            get {
                return AppContext.Instance.SysDicItemVms.List.Where(a => a.DicId == this.Id).OrderBy(a => a.SortNumber).ToList();
            }
        }

        private IEnumerable<SysDicItemViewModel> GetSysDicItemsSelect() {
            yield return SysDicItemViewModel.PleaseSelect;
            foreach (var item in AppContext.Instance.SysDicItemVms.List.Where(a => a.DicId == this.Id)) {
                yield return item;
            }
        }
        public List<SysDicItemViewModel> SysDicItemsSelect {
            get {
                return GetSysDicItemsSelect().OrderBy(a => a.SortNumber).ToList();
            }
        }
    }
}

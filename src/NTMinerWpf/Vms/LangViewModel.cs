using NTMiner.Language;
using NTMiner.Views;
using System;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class LangViewModel : ViewModelBase, ILang {
        private Guid _id;
        private string _code;
        private string _name;
        private int _sortNumber;
        private string _selectedView;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand SortUp { get; private set; }
        public ICommand SortDown { get; private set; }

        public LangViewModel(Guid id) {
            _id = id;
            this.Edit = new DelegateCommand(() => {
                LangEdit.ShowWindow(new LangViewModel(this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.Code}语言吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveLangCommand(this.Id));
                }, icon: IconConst.IconConfirm);
            });
            this.SortUp = new DelegateCommand(() => {
                LangViewModel upOne = LangViewModels.Current.LangVms.OrderByDescending(a => a.SortNumber).FirstOrDefault(a => a.SortNumber < this.SortNumber);
                if (upOne != null) {
                    int sortNumber = upOne.SortNumber;
                    upOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateLangCommand(upOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateLangCommand(this));
                    LangViewModels.Current.OnPropertyChanged(nameof(LangViewModels.LangVms));
                }
            });
            this.SortDown = new DelegateCommand(() => {
                LangViewModel nextOne = LangViewModels.Current.LangVms.OrderBy(a => a.SortNumber).FirstOrDefault(a => a.SortNumber > this.SortNumber);
                if (nextOne != null) {
                    int sortNumber = nextOne.SortNumber;
                    nextOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateLangCommand(nextOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateLangCommand(this));
                    LangViewModels.Current.OnPropertyChanged(nameof(LangViewModels.LangVms));
                }
            });
        }

        public LangViewModel(ILang data) : this(data.GetId()) {
            _code = data.Code;
            _name = data.Name;
            _sortNumber = data.SortNumber;
        }

        public string SelectedView {
            get => _selectedView;
            set {
                if (_selectedView != value) {
                    _selectedView = value;
                    OnPropertyChanged(nameof(SelectedView));
                }
            }
        }

        public Guid GetId() {
            return this.Id;
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
                    if (LangViewModels.Current.LangVms.Any(a => a.Code == value && a.Id != this.Id)) {
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
                    if (LangViewModels.Current.LangVms.Any(a => a.Name == value && a.Id != this.Id)) {
                        throw new ValidationException("名称重复");
                    }
                }
            }
        }

        public int SortNumber {
            get { return _sortNumber; }
            set {
                if (_sortNumber != value) {
                    _sortNumber = value;
                    OnPropertyChanged(nameof(SortNumber));
                }
            }
        }
    }
}

using NTMiner.Language;
using NTMiner.Views;
using System;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class LangItemViewModel : ViewModelBase, ILangItem {
        private Guid _id;
        private Guid _langId;
        private string _viewId;
        private string _key;
        private string _value;
        private int _sortNumber;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand SortUp { get; private set; }
        public ICommand SortDown { get; private set; }

        public LangItemViewModel(Guid id) {
            _id = id;
            this.Edit = new DelegateCommand(() => {
                LangItemEdit.ShowWindow(this);
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.Key}系统字典项吗？", title: "确认", onYes: () => {
                    Global.Execute(new RemoveLangItemCommand(this.Id));
                }, icon: "Icon_Confirm");
            });
            this.SortUp = new DelegateCommand(() => {
                LangItemViewModel upOne = LangItemViewModels.Current.GetLangItemVms(this.LangVm, this.ViewId).OrderByDescending(a => a.SortNumber).FirstOrDefault(a => a.SortNumber < this.SortNumber);
                if (upOne != null) {
                    int sortNumber = upOne.SortNumber;
                    upOne.SortNumber = this.SortNumber;
                    Global.Execute(new UpdateLangItemCommand(upOne));
                    this.SortNumber = sortNumber;
                    Global.Execute(new UpdateLangItemCommand(this));
                    if (this.LangVm != null) {
                        this.LangVm.OnPropertyChanged(nameof(LangVm.LangViewItems));
                    }
                }
            });
            this.SortDown = new DelegateCommand(() => {
                LangItemViewModel nextOne = LangItemViewModels.Current.GetLangItemVms(this.LangVm, this.ViewId).OrderBy(a => a.SortNumber).FirstOrDefault(a => a.SortNumber > this.SortNumber);
                if (nextOne != null) {
                    int sortNumber = nextOne.SortNumber;
                    nextOne.SortNumber = this.SortNumber;
                    Global.Execute(new UpdateLangItemCommand(nextOne));
                    this.SortNumber = sortNumber;
                    Global.Execute(new UpdateLangItemCommand(this));
                    if (this.LangVm != null) {
                        LangVm.OnPropertyChanged(nameof(LangVm.LangViewItems));
                    }
                }
            });
        }

        public LangItemViewModel(ILangItem data) : this(data.GetId()) {
            _langId = data.LangId;
            _viewId = data.ViewId;
            _key = data.Key;
            _value = data.Value;
            _sortNumber = data.SortNumber;
        }

        private LangViewModel _lang;
        public LangViewModel LangVm {
            get {
                if (_lang == null) {
                    LangViewModels.Current.TryGetLangVm(this.LangId, out _lang);
                }
                return _lang;
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

        public Guid LangId {
            get => _langId;
            set {
                _langId = value;
                OnPropertyChanged(nameof(LangId));
            }
        }

        public string ViewId {
            get => _viewId;
            set {
                _viewId = value;
                OnPropertyChanged(nameof(ViewId));
            }
        }

        public string Key {
            get => _key;
            set {
                _key = value;
                OnPropertyChanged(nameof(Key));
            }
        }

        public string Value {
            get => _value;
            set {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public int SortNumber {
            get { return _sortNumber; }
            set {
                _sortNumber = value;
                OnPropertyChanged(nameof(SortNumber));
            }
        }
    }
}

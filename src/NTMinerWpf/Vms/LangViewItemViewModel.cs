using NTMiner.Language;
using NTMiner.Views;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class LangViewItemViewModel : ViewModelBase, ILangViewItem {
        private Guid _id;
        private Guid _langId;
        private string _viewId;
        private string _key;
        private string _value;

        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        public LangViewItemViewModel(Guid id) {
            _id = id;
            this.Edit = new DelegateCommand(() => {
                LangViewItemEdit.ShowWindow(new LangViewItemViewModel(this));
            });
            this.Save = new DelegateCommand(() => {
                if (LangViewItemViewModels.Current.Contains(this.Id)) {
                    Global.Execute(new UpdateLangViewItemCommand(this));
                }
                else {
                    Global.Execute(new AddLangViewItemCommand(this));
                }
                TopWindow.GetTopWindow()?.Close();
            });
        }

        public LangViewItemViewModel(ILangViewItem data) : this(data.GetId()) {
            _langId = data.LangId;
            _viewId = data.ViewId;
            _key = data.Key;
            _value = data.Value;
        }

        private LangViewModel _langVm;
        public LangViewModel LangVm {
            get {
                if (_langVm == null) {
                    LangViewModels.Current.TryGetLangVm(this.LangId, out _langVm);
                }
                return _langVm;
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
    }
}

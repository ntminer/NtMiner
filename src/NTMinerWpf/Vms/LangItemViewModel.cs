using NTMiner.Language;
using System;

namespace NTMiner.Vms {
    public class LangItemViewModel : ViewModelBase, ILangItem {
        private Guid _id;
        private Guid _langId;
        private string _viewId;
        private string _key;
        private string _value;

        public LangItemViewModel(Guid id) {
            _id = id;
        }

        public LangItemViewModel(ILangItem data) : this(data.GetId()) {
            _langId = data.LangId;
            _viewId = data.ViewId;
            _key = data.Key;
            _value = data.Value;
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

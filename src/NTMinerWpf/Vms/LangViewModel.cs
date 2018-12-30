using NTMiner.Language;
using System;
using System.Linq;

namespace NTMiner.Vms {
    public class LangViewModel : ViewModelBase, ILang {
        private Guid _id;
        private string _code;
        private string _name;

        public LangViewModel(Guid id) {
            _id = id;
        }

        public LangViewModel(ILang data) : this(data.GetId()) {
            _code = data.Code;
            _name = data.Name;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            private set {
                _id = value;
                OnPropertyChanged(nameof(Id));
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
    }
}

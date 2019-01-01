using System.Collections.Generic;

namespace NTMiner.Vms {
    public class ViewLangViewModel : ViewModelBase {
        private LangViewModel _selectedLangVm;

        public ViewLangViewModel(string viewId) {
            this.ViewId = viewId;
        }

        public string ViewId { get; private set; }

        public LangViewModels LangVms {
            get {
                return LangViewModels.Current;
            }
        }

        public LangViewModel SelectedLanguage {
            get => _selectedLangVm;
            set {
                _selectedLangVm = value;
                OnPropertyChanged(nameof(SelectedLanguage));
            }
        }

        public List<LangViewItemViewModel> LangViewItemVms {
            get {
                return LangViewItemViewModels.Current.GetLangItemVms(SelectedLanguage, ViewId);
            }
        }
    }
}

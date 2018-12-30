using System.Collections.Generic;

namespace NTMiner.Vms {
    public class LanguagesViewModel : ViewModelBase {
        public static readonly LanguagesViewModel Current = new LanguagesViewModel();

        private List<LanguageViewModel> _languageVms;

        private LanguagesViewModel() {
            _languageVms = new List<LanguageViewModel> {
                LanguageViewModel.English,
                LanguageViewModel.Chinese
            };
        }

        public List<LanguageViewModel> LanguageVms {
            get {
                return _languageVms;
            }
            set {
                _languageVms = value;
                OnPropertyChanged(nameof(LanguageVms));
            }
        }
    }
}

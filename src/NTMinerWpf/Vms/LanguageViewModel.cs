namespace NTMiner.Vms {
    public class LanguageViewModel : ViewModelBase {
        public static readonly LanguageViewModel English = new LanguageViewModel() {
            _name = "English",
            _code = "en"
        };
        public static readonly LanguageViewModel Chinese = new LanguageViewModel() {
            _name = "中文简体",
            _code = "cn-zh"
        };

        private string _name;
        private string _code;

        public LanguageViewModel() {

        }

        public string Name {
            get => _name;
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string Code {
            get => _code;
            set {
                _code = value;
                OnPropertyChanged(nameof(Code));
            }
        }
    }
}

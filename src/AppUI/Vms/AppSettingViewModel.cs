namespace NTMiner.Vms {
    public class AppSettingViewModel : ViewModelBase {
        private string _key;
        private object _value;

        public AppSettingViewModel(IAppSetting data) {
            _key = data.Key;
            _value = data.Value;
        }

        public string Key {
            get => _key;
            set {
                _key = value;
                OnPropertyChanged(nameof(Key));
            }
        }
        public object Value {
            get => _value;
            set {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
    }
}

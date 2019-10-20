using System;

namespace NTMiner.Vms {
    public class DialogWindowViewModel : ViewModelBase {
        private string _icon;
        private string _title;
        private string _message;
        private string _helpUrl;
        private Action _onYes;
        private Func<bool> _onNo;
        private string _yesText;
        private string _noText;

        public DialogWindowViewModel(
            string icon = null,
            string title = null,
            string message = null,
            string helpUrl = null,
            Action onYes = null,
            Func<bool> onNo = null,
            string yesText = null,
            string noText = null) {
            _icon = icon;
            _title = title;
            _message = message;
            _helpUrl = helpUrl;
            _onYes = onYes;
            _onNo = onNo;
            _yesText = yesText;
            _noText = noText;
        }

        public string Icon {
            get { return _icon; }
            set {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        public string Title {
            get => _title;
            set {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        public string Message {
            get => _message;
            set {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }
        public string HelpUrl {
            get => _helpUrl;
            set {
                _helpUrl = value;
                OnPropertyChanged(nameof(HelpUrl));
            }
        }
        public Action OnYes {
            get => _onYes;
            set {
                _onYes = value;
                OnPropertyChanged(nameof(OnYes));
            }
        }
        public Func<bool> OnNo {
            get => _onNo;
            set {
                _onNo = value;
                OnPropertyChanged(nameof(OnNo));
            }
        }
        public string YesText {
            get => _yesText;
            set {
                _yesText = value;
                OnPropertyChanged(nameof(YesText));
            }
        }
        public string NoText {
            get => _noText;
            set {
                _noText = value;
                OnPropertyChanged(nameof(NoText));
            }
        }
    }
}

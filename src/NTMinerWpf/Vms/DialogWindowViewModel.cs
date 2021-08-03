using System;
using System.Windows;

namespace NTMiner.Vms {
    public class DialogWindowViewModel : ViewModelBase {
        private string _icon;
        private string _title;
        private string _message;
        private string _helpUrl;
        private Action _onYes;
        private Func<bool> _onNo;
        private string _btnYesText;
        private string _btnNoText;
        private string _btnOkToolTip;
        private string _btnYesToolTip;
        private string _btnNoToolTip;
        private bool _isConfirmNo;

        public string BtnOkToolTip {
            get => _btnOkToolTip;
            set {
                _btnOkToolTip = value;
                OnPropertyChanged(nameof(BtnOkToolTip));
            }
        }

        public Visibility BtnYesNoVisible {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return Visibility.Visible;
                }
                if (_onYes == null && _onNo == null) {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }

        public string BtnYesToolTip {
            get => _btnYesToolTip;
            set {
                _btnYesToolTip = value;
                OnPropertyChanged(nameof(BtnYesToolTip));
            }
        }

        public string BtnNoToolTip {
            get => _btnNoToolTip;
            set {
                _btnNoToolTip = value;
                OnPropertyChanged(nameof(BtnNoToolTip));
            }
        }

        public bool IsConfirmNo {
            get { return _isConfirmNo; }
            set {
                _isConfirmNo = value;
                OnPropertyChanged(nameof(IsConfirmNo));
            }
        }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public DialogWindowViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        public DialogWindowViewModel(
            string icon = "Icon_Confirm",
            string title = null,
            string message = null,
            string helpUrl = null,
            Action onYes = null,
            Func<bool> onNo = null,
            string btnYesText = "是",
            string btnNoText = "否",
            string btnOkToolTip = null,
            string btnYesToolTip = null,
            string btnNoToolTip = null,
            bool isConfirmNo = false) {
            _icon = icon;
            _title = title;
            _message = message;
            _helpUrl = helpUrl;
            _onYes = onYes;
            _onNo = onNo;
            _btnYesText = btnYesText;
            _btnNoText = btnNoText;
            _btnOkToolTip = btnOkToolTip;
            _btnYesToolTip = btnYesToolTip;
            _btnNoToolTip = btnNoToolTip;
            _isConfirmNo = isConfirmNo;
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
            get {
                if (WpfUtil.IsInDesignMode) {
                    return "http://dl.ntminer.top";
                }
                return _helpUrl;
            }
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
            get => _btnYesText;
            set {
                _btnYesText = value;
                OnPropertyChanged(nameof(YesText));
            }
        }
        public string NoText {
            get => _btnNoText;
            set {
                _btnNoText = value;
                OnPropertyChanged(nameof(NoText));
            }
        }
    }
}

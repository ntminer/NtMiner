using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NTMiner.Vms {
    public class ContainerWindowViewModel : ViewModelBase {
        private Visibility _maxVisible = Visibility.Visible;
        private Visibility _closeVisible = Visibility.Visible;
        private Visibility _footerVisible = Visibility.Visible;
        private Visibility _isIconEditVisible = Visibility.Collapsed;
        private Visibility _isIconAddVisible = Visibility.Collapsed;
        private double _width = 0;
        private double _height = 0;
        private Geometry _icon;
        private bool _isMaskTheParent;
        private ImageSource _iconImageSource = null;
        private double _minHeight;
        private double _minWidth;
        private FormType _formType;
        private string _footerText;
        private string _title;
        private string _iconName;
        private string _iconImage;

        public ContainerWindowViewModel() {
        }

        public double MaxWidth {
            get {
                return SystemParameters.WorkArea.Size.Width;
            }
        }

        public double MaxHeight {
            get {
                return SystemParameters.WorkArea.Size.Height;
            }
        }

        public ResourceDictionary UcResourceDic { get; set; }

        public Version CurrentVersion {
            get {
                return EntryAssemblyInfo.CurrentVersion;
            }
        }

        public string VersionTag {
            get {
                return EntryAssemblyInfo.CurrentVersionTag;
            }
        }

        /// <summary>
        /// 是否遮罩住父窗口
        /// </summary>
        public bool IsMaskTheParent {
            get {
                return _isMaskTheParent;
            }
            set {
                if (_isMaskTheParent != value) {
                    _isMaskTheParent = value;
                }
            }
        }

        public FormType FormType {
            get => _formType;
            set {
                _formType = value;
                switch (value) {
                    case FormType.Add:
                        _isIconAddVisible = Visibility.Visible;
                        break;
                    case FormType.Edit:
                        _isIconEditVisible = Visibility.Visible;
                        break;
                    default:
                        break;
                }
                OnPropertyChanged(nameof(FormType));
            }
        }

        public Visibility IsIconEditVisible {
            get {
                return _isIconEditVisible;
            }
        }

        public Visibility IsIconAddVisible {
            get {
                return _isIconAddVisible;
            }
        }

        public string IconName {
            get { return _iconName; }
            set {
                if (_iconName != value) {
                    _iconName = value;
                    if (!string.IsNullOrEmpty(value)) {
                        Icon = AppUtil.GetResource<Geometry>(value);
                    }
                    OnPropertyChanged(nameof(IconName));
                }
            }
        }

        public Geometry Icon {
            get { return _icon; }
            private set {
                if (_icon != value) {
                    _icon = value;
                    OnPropertyChanged(nameof(Icon));
                }
            }
        }

        public string IconImage {
            get => _iconImage;
            set {
                if (_iconImage != value) {
                    _iconImage = value;
                    if (!string.IsNullOrEmpty(value)) {
                        try {
                            IconImageSource = new BitmapImage(new Uri(IconImage, UriKind.RelativeOrAbsolute));
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e);
                        }
                    }
                    OnPropertyChanged(nameof(IconImage));
                }
            }
        }

        public ImageSource IconImageSource {
            get {
                return _iconImageSource;
            }
            set {
                if (_iconImageSource != value) {
                    _iconImageSource = value;
                    OnPropertyChanged(nameof(IconImageSource));
                }
            }
        }

        public string Title {
            get => _title;
            set {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public double Width {
            get => _width;
            set {
                if (_width != value) {
                    _width = value;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }
        public double MinWidth {
            get => _minWidth;
            set {
                _minWidth = value;
                OnPropertyChanged(nameof(MinWidth));
            }
        }
        public double Height {
            get => _height;
            set {
                if (_height != value) {
                    _height = value;
                    OnPropertyChanged(nameof(Height));
                }
            }
        }
        public double MinHeight {
            get => _minHeight;
            set {
                _minHeight = value;
                OnPropertyChanged(nameof(MinHeight));
            }
        }
        public Visibility MaxVisible {
            get => _maxVisible;
            set {
                if (_maxVisible != value) {
                    _maxVisible = value;
                    OnPropertyChanged(nameof(MaxVisible));
                }
            }
        }

        public Visibility FooterVisible {
            get { return _footerVisible; }
            set {
                if (_footerVisible != value) {
                    _footerVisible = value;
                    OnPropertyChanged(nameof(FooterVisible));
                }
            }
        }

        public string FooterText {
            get => _footerText;
            set {
                _footerText = value;
                OnPropertyChanged(nameof(FooterText));
            }
        }

        public Visibility CloseVisible {
            get { return _closeVisible; }
            set {
                if (_closeVisible != value) {
                    _closeVisible = value;
                    OnPropertyChanged(nameof(CloseVisible));
                }
            }
        }
    }
}

using NTMiner.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NTMiner.Vms {
    public class ContainerWindowViewModel : ViewModelBase {
        private string _title = string.Empty;
        private Visibility _minVisible = Visibility.Visible;
        private Visibility _maxVisible = Visibility.Visible;
        private Visibility _saveVisible = Visibility.Collapsed;
        private Visibility _closeVisible = Visibility.Visible;
        private Visibility _footerVisible = Visibility.Visible;
        private double _width = 0;
        private double _height = 0;
        private Geometry _icon;
        private bool _isDialogWindow;
        private bool _isSingleton = true;

        public Func<UserControl, bool> OnOk;
        public Action<UserControl> OnClose;
        public ICommand ShowWindow { get; private set; }

        public ContainerWindowViewModel() {
            this.ShowWindow = new DelegateCommand(() => {
                ContainerWindow window = ContainerWindow.GetWindow(this);
                window?.ShowWindow();
            });
        }

        public string NTMinerTitle {
            get {
                return NTMinerRoot.Title;
            }
        }

        public Version CurrentVersion {
            get {
                return NTMinerRoot.CurrentVersion;
            }
        }

        public string VersionTag {
            get {
                return NTMinerRoot.VersionTag;
            }
        }

        public bool IsSingleton {
            get {
                return _isSingleton;
            }
            set {
                _isSingleton = value;
                OnPropertyChanged(nameof(IsSingleton));
            }
        }

        public bool IsDialogWindow {
            get {
                return _isDialogWindow;
            }
            set {
                _isDialogWindow = value;
                OnPropertyChanged(nameof(IsDialogWindow));
            }
        }

        private string _iconName;
        private string _iconImage;

        public string IconName {
            get { return _iconName; }
            set {
                _iconName = value;
                if (!string.IsNullOrEmpty(value)) {
                    Icon = (Geometry)Application.Current.Resources[value];
                }
                OnPropertyChanged(nameof(IconName));
            }
        }

        public Geometry Icon {
            get { return _icon; }
            private set {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        public string IconImage {
            get => _iconImage;
            set {
                _iconImage = value;
                if (!string.IsNullOrEmpty(value)) {
                    IconImageSource = new BitmapImage(new Uri(IconImage, UriKind.RelativeOrAbsolute));
                }
                OnPropertyChanged(nameof(IconImage));
            }
        }
        private ImageSource _iconImageSource = null;
        public ImageSource IconImageSource {
            get {
                return _iconImageSource;
            }
            set {
                _iconImageSource = value;
                OnPropertyChanged(nameof(IconImageSource));
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
                _width = value;
                OnPropertyChanged(nameof(Width));
            }
        }
        public double Height {
            get => _height;
            set {
                _height = value;
                OnPropertyChanged(nameof(Height));
            }
        }
        public Visibility MinVisible {
            get => _minVisible;
            set {
                _minVisible = value;
                OnPropertyChanged(nameof(MinVisible));
            }
        }
        public Visibility MaxVisible {
            get => _maxVisible;
            set {
                _maxVisible = value;
                OnPropertyChanged(nameof(MaxVisible));
            }
        }

        public Visibility SaveVisible {
            get { return _saveVisible; }
            set {
                _saveVisible = value;
                OnPropertyChanged(nameof(SaveVisible));
            }
        }

        public Visibility FooterVisible {
            get { return _footerVisible; }
            set {
                _footerVisible = value;
                OnPropertyChanged(nameof(FooterVisible));
            }
        }

        public Visibility CloseVisible {
            get { return _closeVisible; }
            set {
                _closeVisible = value;
                OnPropertyChanged(nameof(CloseVisible));
            }
        }
    }
}

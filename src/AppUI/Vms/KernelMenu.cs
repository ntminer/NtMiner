using System.Windows;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class KernelMenu : ViewModelBase {
        private string _name;
        private Geometry _icon;
        private string _iconName;

        public KernelMenu(string name, string iconName) {
            _name = name;
            _iconName = iconName;
            if (!string.IsNullOrEmpty(iconName)) {
                _icon = (Geometry)Application.Current.Resources[iconName];
            }
        }

        public string Name {
            get => _name;
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string IconName {
            get => _iconName;
            set {
                _iconName = value;
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

        private static readonly SolidColorBrush Transparent = new SolidColorBrush(Colors.Transparent);
        private static readonly SolidColorBrush SelectedBackground = new SolidColorBrush(Color.FromRgb(0x04, 0x35, 0x5B));
        private SolidColorBrush _itemBackground = Transparent;
        public SolidColorBrush ItemBackground {
            get {
                return _itemBackground;
            }
            set {
                _itemBackground = value;
                OnPropertyChanged(nameof(ItemBackground));
            }
        }

        private static readonly SolidColorBrush White = new SolidColorBrush(Colors.White);
        private static readonly SolidColorBrush SelectedForeground = new SolidColorBrush(Color.FromRgb(0x2C, 0xA2, 0xFC));
        private SolidColorBrush _itemForeground = White;
        public SolidColorBrush ItemForeground {
            get {
                return _itemForeground;
            }
            set {
                _itemForeground = value;
                OnPropertyChanged(nameof(ItemForeground));
            }
        }

        private static readonly SolidColorBrush SelectedBorderColor = new SolidColorBrush(Color.FromRgb(0x2C, 0xA2, 0xFC));
        private SolidColorBrush _borderBrush = Transparent;
        public SolidColorBrush BorderBrush {
            get {
                return _borderBrush;
            }
            set {
                _borderBrush = value;
                OnPropertyChanged(nameof(BorderBrush));
            }
        }

        public void SetSelectedBackground() {
            ItemBackground = SelectedBackground;
            ItemForeground = SelectedForeground;
            BorderBrush = SelectedBorderColor;
        }

        public void SetDefaultBackground() {
            ItemBackground = Transparent;
            ItemForeground = White;
            BorderBrush = Transparent;
        }
    }
}

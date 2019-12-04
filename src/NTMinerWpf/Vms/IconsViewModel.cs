using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class IconsViewModel : ViewModelBase {
        public class Icon : ViewModelBase {
            private SolidColorBrush _borderBrush;

            public string Key { get; set; }
            public StreamGeometry Data { get; set; }
            public SolidColorBrush BorderBrush {
                get => _borderBrush;
                set {
                    if (_borderBrush != value) {
                        _borderBrush = value;
                        OnPropertyChanged(nameof(BorderBrush));
                    }
                }
            }
        }

        private readonly List<Icon> _icons = new List<Icon>();
        private string _keyword;

        public ICommand ClearKeyword { get; private set; }

        public IconsViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            foreach (var dic in Application.Current.Resources.MergedDictionaries) {
                if (dic.Contains("Icon_Icon")) {
                    var list = new List<Icon>();
                    foreach (var key in dic.Keys) {
                        list.Add(new Icon {
                            Key = key.ToString(),
                            Data = (StreamGeometry)dic[key],
                            BorderBrush = WpfUtil.TransparentBrush
                        });
                    }
                    _icons = list.OrderBy(a => a.Key).ToList();
                    break;
                }
            }
            this.ClearKeyword = new DelegateCommand(() => {
                this.Keyword = string.Empty;
            });
        }

        public string Keyword {
            get => _keyword;
            set {
                _keyword = value;
                OnPropertyChanged(nameof(Keyword));
                if (!string.IsNullOrEmpty(value)) {
                    foreach (var icon in _icons) {
                        if (icon.Key.IgnoreCaseContains(value)) {
                            icon.BorderBrush = WpfUtil.RedBrush;
                        }
                        else {
                            icon.BorderBrush = WpfUtil.TransparentBrush;
                        }
                    }
                }
                else {
                    foreach (var icon in _icons) {
                        icon.BorderBrush = WpfUtil.TransparentBrush;
                    }
                }
            }
        }

        public List<Icon> Icons {
            get { return _icons; }
        }
    }
}

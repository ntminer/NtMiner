using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class LogColorViewModel : ViewModelBase {
        public readonly static LogColorViewModel Current = new LogColorViewModel();

        private LogColorViewModel() {

        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }

        private List<ConsoleColorItem> _consoleColorItems;

        public List<ConsoleColorItem> ConsoleColorItems {
            get {
                if (_consoleColorItems == null) {
                    _consoleColorItems = new List<ConsoleColorItem>();
                    foreach (var item in ConsoleColor.Red.GetEnumItems()) {
                        _consoleColorItems.Add(new ConsoleColorItem(item.Name, item.Value));
                    }
                }
                return _consoleColorItems;
            }
        }

        public ConsoleColorItem CurrentSpeedColor {
            get {
                return ConsoleColorItems.FirstOrDefault(a => a.ConsoleColor == MinerProfile.SpeedColor);
            }
            set {
                if (MinerProfile.SpeedColor != value.ConsoleColor) {
                    MinerProfile.SpeedColor = value.ConsoleColor;
                    OnPropertyChanged(nameof(CurrentSpeedColor));
                }
            }
        }

        public ConsoleColorItem CurrentTFPColor {
            get {
                return ConsoleColorItems.FirstOrDefault(a => a.ConsoleColor == MinerProfile.TFPColor);
            }
            set {
                if (MinerProfile.TFPColor != value.ConsoleColor) {
                    MinerProfile.TFPColor = value.ConsoleColor;
                    OnPropertyChanged(nameof(CurrentTFPColor));
                }
            }
        }

        public ConsoleColorItem CurrentSuccessColor {
            get {
                return ConsoleColorItems.FirstOrDefault(a => a.ConsoleColor == MinerProfile.SuccessColor);
            }
            set {
                if (MinerProfile.SuccessColor != value.ConsoleColor) {
                    MinerProfile.SuccessColor = value.ConsoleColor;
                    OnPropertyChanged(nameof(CurrentSuccessColor));
                }
            }
        }

        public ConsoleColorItem CurrentFailColor {
            get {
                return ConsoleColorItems.FirstOrDefault(a => a.ConsoleColor == MinerProfile.FailColor);
            }
            set {
                if (MinerProfile.FailColor != value.ConsoleColor) {
                    MinerProfile.FailColor = value.ConsoleColor;
                    OnPropertyChanged(nameof(CurrentFailColor));
                }
            }
        }

        public ConsoleColorItem CurrentErrorColor {
            get {
                return ConsoleColorItems.FirstOrDefault(a => a.ConsoleColor == MinerProfile.ErrorColor);
            }
            set {
                if (MinerProfile.ErrorColor != value.ConsoleColor) {
                    MinerProfile.ErrorColor = value.ConsoleColor;
                    OnPropertyChanged(nameof(CurrentErrorColor));
                }
            }
        }

        public ConsoleColorItem CurrentInfoColor {
            get {
                return ConsoleColorItems.FirstOrDefault(a => a.ConsoleColor == MinerProfile.InfoColor);
            }
            set {
                if (MinerProfile.InfoColor != value.ConsoleColor) {
                    MinerProfile.InfoColor = value.ConsoleColor;
                    OnPropertyChanged(nameof(CurrentInfoColor));
                }
            }
        }
    }

    public class ConsoleColorItem : ViewModelBase {
        private string _colorText;
        private ConsoleColor _consoleColor;

        public ConsoleColorItem(string colorText, ConsoleColor consoleColor) {
            _colorText = colorText;
            _consoleColor = consoleColor;
        }

        public string ColorText {
            get => _colorText;
            set {
                if (_colorText != value) {
                    _colorText = value;
                    OnPropertyChanged(nameof(ColorText));
                }
            }
        }

        public ConsoleColor ConsoleColor {
            get {
                return _consoleColor;
            }
        }

        public SolidColorBrush MediaColor {
            get {
                return new SolidColorBrush(ConsoleColor.ToMediaColor());
            }
        }
    }
}

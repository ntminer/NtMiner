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
                MinerProfile.SpeedColor = value.ConsoleColor;
                OnPropertyChanged(nameof(CurrentSpeedColor));
            }
        }

        public ConsoleColorItem CurrentTFPColor {
            get {
                return ConsoleColorItems.FirstOrDefault(a => a.ConsoleColor == MinerProfile.TFPColor);
            }
            set {
                MinerProfile.TFPColor = value.ConsoleColor;
                OnPropertyChanged(nameof(CurrentTFPColor));
            }
        }

        public ConsoleColorItem CurrentSuccessColor {
            get {
                return ConsoleColorItems.FirstOrDefault(a => a.ConsoleColor == MinerProfile.SuccessColor);
            }
            set {
                MinerProfile.SuccessColor = value.ConsoleColor;
                OnPropertyChanged(nameof(CurrentSuccessColor));
            }
        }

        public ConsoleColorItem CurrentFailColor {
            get {
                return ConsoleColorItems.FirstOrDefault(a => a.ConsoleColor == MinerProfile.FailColor);
            }
            set {
                MinerProfile.FailColor = value.ConsoleColor;
                OnPropertyChanged(nameof(CurrentFailColor));
            }
        }

        public ConsoleColorItem CurrentErrorColor {
            get {
                return ConsoleColorItems.FirstOrDefault(a => a.ConsoleColor == MinerProfile.ErrorColor);
            }
            set {
                MinerProfile.ErrorColor = value.ConsoleColor;
                OnPropertyChanged(nameof(CurrentErrorColor));
            }
        }

        public ConsoleColorItem CurrentInfoColor {
            get {
                return ConsoleColorItems.FirstOrDefault(a => a.ConsoleColor == MinerProfile.InfoColor);
            }
            set {
                MinerProfile.InfoColor = value.ConsoleColor;
                OnPropertyChanged(nameof(CurrentInfoColor));
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
                _colorText = value;
                OnPropertyChanged(nameof(ColorText));
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

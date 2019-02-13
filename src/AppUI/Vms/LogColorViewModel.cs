using NTMiner.Core;
using NTMiner.Core.SysDics;
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

        private ConsoleColor SpeedColor {
            get {
                ConsoleColor color = ConsoleColor.White;
                ISysDicItem dicItem;
                if (NTMinerRoot.Current.SysDicItemSet.TryGetDicItem("LogColor", nameof(SpeedColor), out dicItem)) {
                    if (!dicItem.Value.TryParse(out color)) {
                        color = ConsoleColor.White;
                    }
                }
                return color;
            }
            set {
                ISysDicItem dicItem;
                if (NTMinerRoot.Current.SysDicItemSet.TryGetDicItem("LogColor", nameof(SpeedColor), out dicItem)) {
                    if (dicItem.Value != value.GetName()) {
                        VirtualRoot.Execute(new UpdateSysDicItemCommand(new SysDicItemViewModel(dicItem) { Value = value.GetName() }));
                        OnPropertyChanged(nameof(SpeedColor));
                    }
                }
            }
        }

        /// <summary>
        /// TFP: Time Fan Pow
        /// </summary>
        private ConsoleColor TFPColor {
            get {
                ConsoleColor color = ConsoleColor.White;
                ISysDicItem dicItem;
                if (NTMinerRoot.Current.SysDicItemSet.TryGetDicItem("LogColor", nameof(TFPColor), out dicItem)) {
                    if (!dicItem.Value.TryParse(out color)) {
                        color = ConsoleColor.White;
                    }
                }
                return color;
            }
            set {
                ISysDicItem dicItem;
                if (NTMinerRoot.Current.SysDicItemSet.TryGetDicItem("LogColor", nameof(TFPColor), out dicItem)) {
                    if (dicItem.Value != value.GetName()) {
                        VirtualRoot.Execute(new UpdateSysDicItemCommand(new SysDicItemViewModel(dicItem) { Value = value.GetName() }));
                        OnPropertyChanged(nameof(TFPColor));
                    }
                }
            }
        }

        private ConsoleColor SuccessColor {
            get {
                ConsoleColor color = ConsoleColor.White;
                ISysDicItem dicItem;
                if (NTMinerRoot.Current.SysDicItemSet.TryGetDicItem("LogColor", nameof(SuccessColor), out dicItem)) {
                    if (!dicItem.Value.TryParse(out color)) {
                        color = ConsoleColor.White;
                    }
                }
                return color;
            }
            set {
                ISysDicItem dicItem;
                if (NTMinerRoot.Current.SysDicItemSet.TryGetDicItem("LogColor", nameof(SuccessColor), out dicItem)) {
                    if (dicItem.Value != value.GetName()) {
                        VirtualRoot.Execute(new UpdateSysDicItemCommand(new SysDicItemViewModel(dicItem) { Value = value.GetName() }));
                        OnPropertyChanged(nameof(SuccessColor));
                    }
                }
            }
        }
        private ConsoleColor FailColor {
            get {
                ConsoleColor color = ConsoleColor.White;
                ISysDicItem dicItem;
                if (NTMinerRoot.Current.SysDicItemSet.TryGetDicItem("LogColor", nameof(FailColor), out dicItem)) {
                    if (!dicItem.Value.TryParse(out color)) {
                        color = ConsoleColor.White;
                    }
                }
                return color;
            }
            set {
                ISysDicItem dicItem;
                if (NTMinerRoot.Current.SysDicItemSet.TryGetDicItem("LogColor", nameof(FailColor), out dicItem)) {
                    if (dicItem.Value != value.GetName()) {
                        VirtualRoot.Execute(new UpdateSysDicItemCommand(new SysDicItemViewModel(dicItem) { Value = value.GetName() }));
                        OnPropertyChanged(nameof(FailColor));
                    }
                }
            }
        }
        private ConsoleColor ErrorColor {
            get {
                ConsoleColor color = ConsoleColor.White;
                ISysDicItem dicItem;
                if (NTMinerRoot.Current.SysDicItemSet.TryGetDicItem("LogColor", nameof(ErrorColor), out dicItem)) {
                    if (!dicItem.Value.TryParse(out color)) {
                        color = ConsoleColor.White;
                    }
                }
                return color;
            }
            set {
                ISysDicItem dicItem;
                if (NTMinerRoot.Current.SysDicItemSet.TryGetDicItem("LogColor", nameof(ErrorColor), out dicItem)) {
                    if (dicItem.Value != value.GetName()) {
                        VirtualRoot.Execute(new UpdateSysDicItemCommand(new SysDicItemViewModel(dicItem) { Value = value.GetName() }));
                        OnPropertyChanged(nameof(ErrorColor));
                    }
                }
            }
        }
        private ConsoleColor InfoColor {
            get {
                ConsoleColor color = ConsoleColor.White;
                ISysDicItem dicItem;
                if (NTMinerRoot.Current.SysDicItemSet.TryGetDicItem("LogColor", nameof(InfoColor), out dicItem)) {
                    if (!dicItem.Value.TryParse(out color)) {
                        color = ConsoleColor.White;
                    }
                }
                return color;
            }
            set {
                ISysDicItem dicItem;
                if (NTMinerRoot.Current.SysDicItemSet.TryGetDicItem("LogColor", nameof(InfoColor), out dicItem)) {
                    if (dicItem.Value != value.GetName()) {
                        VirtualRoot.Execute(new UpdateSysDicItemCommand(new SysDicItemViewModel(dicItem) { Value = value.GetName() }));
                        OnPropertyChanged(nameof(InfoColor));
                    }
                }
            }
        }

        public ConsoleColorItem CurrentSpeedColor {
            get {
                return ConsoleColorItems.FirstOrDefault(a => a.ConsoleColor == SpeedColor);
            }
            set {
                if (SpeedColor != value.ConsoleColor) {
                    SpeedColor = value.ConsoleColor;
                    OnPropertyChanged(nameof(CurrentSpeedColor));
                }
            }
        }

        public ConsoleColorItem CurrentTFPColor {
            get {
                return ConsoleColorItems.FirstOrDefault(a => a.ConsoleColor == TFPColor);
            }
            set {
                if (TFPColor != value.ConsoleColor) {
                    TFPColor = value.ConsoleColor;
                    OnPropertyChanged(nameof(CurrentTFPColor));
                }
            }
        }

        public ConsoleColorItem CurrentSuccessColor {
            get {
                return ConsoleColorItems.FirstOrDefault(a => a.ConsoleColor == SuccessColor);
            }
            set {
                if (SuccessColor != value.ConsoleColor) {
                    SuccessColor = value.ConsoleColor;
                    OnPropertyChanged(nameof(CurrentSuccessColor));
                }
            }
        }

        public ConsoleColorItem CurrentFailColor {
            get {
                return ConsoleColorItems.FirstOrDefault(a => a.ConsoleColor == FailColor);
            }
            set {
                if (FailColor != value.ConsoleColor) {
                    FailColor = value.ConsoleColor;
                    OnPropertyChanged(nameof(CurrentFailColor));
                }
            }
        }

        public ConsoleColorItem CurrentErrorColor {
            get {
                return ConsoleColorItems.FirstOrDefault(a => a.ConsoleColor == ErrorColor);
            }
            set {
                if (ErrorColor != value.ConsoleColor) {
                    ErrorColor = value.ConsoleColor;
                    OnPropertyChanged(nameof(CurrentErrorColor));
                }
            }
        }

        public ConsoleColorItem CurrentInfoColor {
            get {
                return ConsoleColorItems.FirstOrDefault(a => a.ConsoleColor == InfoColor);
            }
            set {
                if (InfoColor != value.ConsoleColor) {
                    InfoColor = value.ConsoleColor;
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

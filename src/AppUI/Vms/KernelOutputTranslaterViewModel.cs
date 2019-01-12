using NTMiner.Core;
using NTMiner.Core.Kernels;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class KernelOutputTranslaterViewModel : ViewModelBase, IKernelOutputTranslater {
        private Guid _kernelId;
        private string _regexPattern;
        private Guid _id;
        private string _replacement;
        private string _color;
        private int _sortNumber;
        private bool _isPre;

        public Guid GetId() {
            return this.Id;
        }

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand SortUp { get; private set; }
        public ICommand SortDown { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public KernelOutputTranslaterViewModel() {
            if (!DevMode.IsDevMode) {
                throw new InvalidProgramException();
            }
        }

        public KernelOutputTranslaterViewModel(IKernelOutputTranslater data) : this(data.GetId()) {
            _kernelId = data.KernelId;
            _kernelOutputId = data.KernelOutputId;
            _regexPattern = data.RegexPattern;
            _id = data.GetId();
            _replacement = data.Replacement;
            _color = data.Color;
            _sortNumber = data.SortNumber;
            _isPre = data.IsPre;
        }

        public KernelOutputTranslaterViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                int sortNumber = this.SortNumber;
                if (NTMinerRoot.Current.KernelOutputTranslaterSet.Contains(this.Id)) {
                    Global.Execute(new UpdateKernelOutputTranslaterCommand(this));
                }
                else {
                    Global.Execute(new AddKernelOutputTranslaterCommand(this));
                }
                if (sortNumber != this.SortNumber) {
                    if (KernelViewModels.Current.TryGetKernelVm(this.KernelId, out KernelViewModel kernelVm)) {
                        kernelVm.OnPropertyChanged(nameof(kernelVm.KernelOutputTranslaters));
                    }
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand(() => {
                KernelOutputTranslaterEdit.ShowEditWindow(this);
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.RegexPattern}内核输出翻译器吗？", title: "确认", onYes: () => {
                    Global.Execute(new RemoveKernelOutputTranslaterCommand(this.Id));
                }, icon: "Icon_Confirm");
            });
            this.SortUp = new DelegateCommand(() => {
                KernelOutputTranslaterViewModel upOne = KernelOutputTranslaterViewModels.Current.GetListByKernelId(this.KernelId).OrderByDescending(a => a.SortNumber).FirstOrDefault(a => a.SortNumber < this.SortNumber);
                if (upOne != null) {
                    int sortNumber = upOne.SortNumber;
                    upOne.SortNumber = this.SortNumber;
                    Global.Execute(new UpdateKernelOutputTranslaterCommand(upOne));
                    this.SortNumber = sortNumber;
                    Global.Execute(new UpdateKernelOutputTranslaterCommand(this));
                    KernelOutputTranslaterViewModels.Current.OnPropertyChanged(nameof(KernelOutputTranslaterViewModels.AllKernelOutputTranslaterVms));
                    if (KernelViewModels.Current.TryGetKernelVm(this.KernelId, out KernelViewModel kernelVm)) {
                        kernelVm.OnPropertyChanged(nameof(kernelVm.KernelOutputTranslaters));
                    }
                }
            });
            this.SortDown = new DelegateCommand(() => {
                KernelOutputTranslaterViewModel nextOne = KernelOutputTranslaterViewModels.Current.GetListByKernelId(this.KernelId).OrderBy(a => a.SortNumber).FirstOrDefault(a => a.SortNumber > this.SortNumber);
                if (nextOne != null) {
                    int sortNumber = nextOne.SortNumber;
                    nextOne.SortNumber = this.SortNumber;
                    Global.Execute(new UpdateKernelOutputTranslaterCommand(nextOne));
                    this.SortNumber = sortNumber;
                    Global.Execute(new UpdateKernelOutputTranslaterCommand(this));
                    KernelOutputTranslaterViewModels.Current.OnPropertyChanged(nameof(KernelOutputTranslaterViewModels.AllKernelOutputTranslaterVms));
                    if (KernelViewModels.Current.TryGetKernelVm(this.KernelId, out KernelViewModel kernelVm)) {
                        kernelVm.OnPropertyChanged(nameof(kernelVm.KernelOutputTranslaters));
                    }
                }
            });
        }

        public Guid Id {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private Guid _kernelOutputId;
        public Guid KernelOutputId {
            get => _kernelOutputId;
            set {
                _kernelOutputId = value;
                OnPropertyChanged(nameof(KernelOutputId));
            }
        }

        public Guid KernelId {
            get => _kernelId;
            set {
                _kernelId = value;
                OnPropertyChanged(nameof(KernelId));
            }
        }

        public string RegexPattern {
            get => _regexPattern;
            set {
                _regexPattern = value;
                OnPropertyChanged(nameof(RegexPattern));
            }
        }

        public string Replacement {
            get => _replacement;
            set {
                _replacement = value;
                OnPropertyChanged(nameof(Replacement));
            }
        }

        public string Color {
            get => _color;
            set {
                _color = value;
                OnPropertyChanged(nameof(Color));
                OnPropertyChanged(nameof(ColorBrush));
                OnPropertyChanged(nameof(ColorDicItem));
                OnPropertyChanged(nameof(ColorDescription));
            }
        }

        private static readonly SolidColorBrush White = new SolidColorBrush(Colors.White);
        public SolidColorBrush ColorBrush {
            get {
                if (string.IsNullOrEmpty(this.Color)) {
                    return White;
                }
                if (ColorDicItem != null && ColorDicItem.Value.TryParse(out ConsoleColor consoleColor)) {
                    return new SolidColorBrush(consoleColor.ToMediaColor());
                }
                return White;
            }
        }

        private SysDicItemViewModel _colorDicItem;
        public SysDicItemViewModel ColorDicItem {
            get {
                if (_colorDicItem == null || this.Color != _colorDicItem.Code) {
                    _colorDicItem = LogColorDicVm.SysDicItems.FirstOrDefault(a => a.Code == this.Color);
                }
                return _colorDicItem;
            }
        }

        public string ColorDescription {
            get {
                SysDicViewModel colorDic = LogColorDicVm;
                if (colorDic != null) {
                    if (ColorDicItem != null) {
                        return ColorDicItem.Description;
                    }
                }
                return "默认";
            }
        }

        private SysDicViewModel _clorDic;

        public SysDicViewModel LogColorDicVm {
            get {
                if (_clorDic == null) {
                    SysDicViewModels.Current.TryGetSysDicVm("LogColor", out _clorDic);
                }
                return _clorDic;
            }
        }

        public int SortNumber {
            get => _sortNumber;
            set {
                _sortNumber = value;
                OnPropertyChanged(nameof(SortNumber));
            }
        }

        public bool IsPre {
            get { return _isPre; }
            set {
                _isPre = value;
                OnPropertyChanged(nameof(IsPre));
                OnPropertyChanged(nameof(IsPreText));
            }
        }

        public string IsPreText {
            get {
                if (IsPre) {
                    return "是";
                }
                return "否";
            }
        }

        public SysDicItemViewModel SelectedColor {
            get {
                if (string.IsNullOrEmpty(this.Color)) {
                    return SysDicItemViewModel.PleaseSelect;
                }
                SysDicItemViewModel vm = LogColorDicVm.SysDicItemsSelect.FirstOrDefault(a => a.Code == this.Color);
                if (vm != null) {
                    return vm;
                }
                return SysDicItemViewModel.PleaseSelect;
            }
            set {
                if (this.Color != value.Code) {
                    this.Color = value.Code;
                }
            }
        }
    }
}

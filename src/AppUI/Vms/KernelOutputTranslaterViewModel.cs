using NTMiner.Core;
using NTMiner.Core.Kernels;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class KernelOutputTranslaterViewModel : ViewModelBase, IKernelOutputTranslater, IEditableViewModel {
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
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public KernelOutputTranslaterViewModel(IKernelOutputTranslater data) : this(data.GetId()) {
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
                    VirtualRoot.Execute(new UpdateKernelOutputTranslaterCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddKernelOutputTranslaterCommand(this));
                }
                if (sortNumber != this.SortNumber) {
                    if (KernelOutputViewModels.Current.TryGetKernelOutputVm(this.KernelOutputId, out KernelOutputViewModel kernelOutputVm)) {
                        kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputTranslaters));
                    }
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                KernelOutputTranslaterEdit.ShowWindow(formType ?? FormType.Edit, this);
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.RegexPattern}内核输出翻译器吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveKernelOutputTranslaterCommand(this.Id));
                }, icon: IconConst.IconConfirm);
            });
            this.SortUp = new DelegateCommand(() => {
                KernelOutputTranslaterViewModel upOne = KernelOutputTranslaterViewModels.Current.GetListByKernelId(this.KernelOutputId).OrderByDescending(a => a.SortNumber).FirstOrDefault(a => a.SortNumber < this.SortNumber);
                if (upOne != null) {
                    int sortNumber = upOne.SortNumber;
                    upOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateKernelOutputTranslaterCommand(upOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateKernelOutputTranslaterCommand(this));
                    KernelOutputTranslaterViewModels.Current.OnPropertyChanged(nameof(KernelOutputTranslaterViewModels.AllKernelOutputTranslaterVms));
                    if (KernelOutputViewModels.Current.TryGetKernelOutputVm(this.KernelOutputId, out KernelOutputViewModel kernelOutputVm)) {
                        kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputTranslaters));
                    }
                }
            });
            this.SortDown = new DelegateCommand(() => {
                KernelOutputTranslaterViewModel nextOne = KernelOutputTranslaterViewModels.Current.GetListByKernelId(this.KernelOutputId).OrderBy(a => a.SortNumber).FirstOrDefault(a => a.SortNumber > this.SortNumber);
                if (nextOne != null) {
                    int sortNumber = nextOne.SortNumber;
                    nextOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateKernelOutputTranslaterCommand(nextOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateKernelOutputTranslaterCommand(this));
                    KernelOutputTranslaterViewModels.Current.OnPropertyChanged(nameof(KernelOutputTranslaterViewModels.AllKernelOutputTranslaterVms));
                    if (KernelOutputViewModels.Current.TryGetKernelOutputVm(this.KernelOutputId, out KernelOutputViewModel kernelOutputVm)) {
                        kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputTranslaters));
                    }
                }
            });
        }

        public Guid Id {
            get => _id;
            set {
                if (_id != value) {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        private Guid _kernelOutputId;
        public Guid KernelOutputId {
            get => _kernelOutputId;
            set {
                if (_kernelOutputId != value) {
                    _kernelOutputId = value;
                    OnPropertyChanged(nameof(KernelOutputId));
                }
            }
        }

        public string RegexPattern {
            get => _regexPattern;
            set {
                if (_regexPattern != value) {
                    _regexPattern = value;
                    OnPropertyChanged(nameof(RegexPattern));
                }
            }
        }

        public string Replacement {
            get => _replacement;
            set {
                if (_replacement != value) {
                    _replacement = value;
                    OnPropertyChanged(nameof(Replacement));
                }
            }
        }

        public string Color {
            get => _color;
            set {
                if (_color != value) {
                    _color = value;
                    OnPropertyChanged(nameof(Color));
                    OnPropertyChanged(nameof(ColorBrush));
                    OnPropertyChanged(nameof(ColorDicItem));
                    OnPropertyChanged(nameof(ColorDescription));
                }
            }
        }

        private static readonly SolidColorBrush s_white = new SolidColorBrush(Colors.White);
        public SolidColorBrush ColorBrush {
            get {
                if (string.IsNullOrEmpty(this.Color)) {
                    return s_white;
                }
                if (ColorDicItem != null && ColorDicItem.Value.TryParse(out ConsoleColor consoleColor)) {
                    return new SolidColorBrush(consoleColor.ToMediaColor());
                }
                return s_white;
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
                if (_sortNumber != value) {
                    _sortNumber = value;
                    OnPropertyChanged(nameof(SortNumber));
                }
            }
        }

        public bool IsPre {
            get { return _isPre; }
            set {
                if (_isPre != value) {
                    _isPre = value;
                    OnPropertyChanged(nameof(IsPre));
                    OnPropertyChanged(nameof(IsPreText));
                }
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

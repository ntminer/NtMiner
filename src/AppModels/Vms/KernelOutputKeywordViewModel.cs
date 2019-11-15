using NTMiner.Core;
using NTMiner.MinerClient;
using NTMiner.Views;
using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class KernelOutputKeywordViewModel : ViewModelBase, IKernelOutputKeyword, IEditableViewModel {
        private Guid _id;
        private Guid _kernelOutputId;
        private string _messageType;
        private string _keyword;
        private string _description;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public KernelOutputKeywordViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        private LocalMessageType _messageTypeEnum;
        public KernelOutputKeywordViewModel(IKernelOutputKeyword data) : this(data.GetId()) {
            this.DataLevel = data.DataLevel;
            _kernelOutputId = data.KernelOutputId;
            _messageType = data.MessageType;
            data.MessageType.TryParse(out _messageTypeEnum);
            _keyword = data.Keyword;
            _description = data.Description;
        }

        public KernelOutputKeywordViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(this.Keyword)) {
                    VirtualRoot.Out.ShowError("关键字不能为空", delaySeconds: 4);
                    return;
                }
                if (AppContext.Instance.KernelOutputKeywordVms.GetListByKernelId(this.KernelOutputId).Any(a => a.Id != this.Id && a.Keyword == this.Keyword)) {
                    throw new ValidationException($"关键字 {this.Keyword} 已经存在");
                }
                if (DevMode.IsDevMode) {
                    LoginWindow.Login(() => {
                        VirtualRoot.Execute(new AddOrUpdateKernelOutputKeywordCommand(this));
                        CloseWindow?.Invoke();
                    });
                }
                else {
                    VirtualRoot.Execute(new AddOrUpdateKernelOutputKeywordCommand(this));
                    CloseWindow?.Invoke();
                }
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.IsReadOnly) {
                    return;
                }
                VirtualRoot.Execute(new KernelOutputKeywordEditCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (this.IsReadOnly) {
                    return;
                }
                this.ShowDialog(new DialogWindowViewModel(message: $"您确定删除{this.Keyword}内核输出关键字吗？", title: "确认", onYes: () => {
                    if (DevMode.IsDevMode) {
                        LoginWindow.Login(() => {
                            VirtualRoot.Execute(new RemoveKernelOutputKeywordCommand(this.Id));
                        });
                    }
                    else {
                        VirtualRoot.Execute(new RemoveKernelOutputKeywordCommand(this.Id));
                    }
                }));
            });
        }

        public LocalMessageType MessageTypeEnum {
            get { return _messageTypeEnum; }
        }

        public EnumItem<LocalMessageType> MessageTypeEnumItem {
            get {
                return _messageTypeEnum.GetEnumItem();
            }
            set {
                _messageTypeEnum = value.Value;
                _messageType = _messageTypeEnum.GetName();
                OnPropertyChanged(nameof(MessageType));
                OnPropertyChanged(nameof(MessageTypeEnumItem));
                OnPropertyChanged(nameof(MessageTypeText));
                OnPropertyChanged(nameof(MessageTypeIcon));
                OnPropertyChanged(nameof(IconFill));
            }
        }

        public StreamGeometry MessageTypeIcon {
            get {
                return LocalMessageViewModel.GetIcon(_messageTypeEnum);
            }
        }

        public SolidColorBrush IconFill {
            get {
                return LocalMessageViewModel.GetIconFill(_messageTypeEnum);
            }
        }

        public DataLevel DataLevel { get; set; }

        public bool IsReadOnly {
            get {
                return !DevMode.IsDevMode && DataLevel == DataLevel.Global;
            }
        }

        public void SetDataLevel(DataLevel dataLevel) {
            this.DataLevel = dataLevel;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public Guid KernelOutputId {
            get => _kernelOutputId;
            set {
                _kernelOutputId = value;
                OnPropertyChanged(nameof(KernelOutputId));
            }
        }

        public string MessageType {
            get => _messageType;
            set {
                _messageType = value;
                OnPropertyChanged(nameof(MessageType));
            }
        }

        public string MessageTypeText {
            get {
                return _messageTypeEnum.GetDescription();
            }
        }

        public string Keyword {
            get => _keyword;
            set {
                _keyword = value;
                OnPropertyChanged(nameof(Keyword));
                if (string.IsNullOrEmpty(value)) {
                    throw new ValidationException("关键字不能为空");
                }
            }
        }

        public string Description {
            get { return _description; }
            set {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }
}

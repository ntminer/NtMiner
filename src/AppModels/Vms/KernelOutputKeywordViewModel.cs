using NTMiner.Core;
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

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public KernelOutputKeywordViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        private LocalMessageType _messageTypeEnum;
        public KernelOutputKeywordViewModel(IKernelOutputKeyword data) : this(data.GetId(), data.KernelOutputId, data.MessageType, data.GetDataLevel()) {
            _keyword = data.Keyword;
            _description = data.Description;
        }

        public KernelOutputKeywordViewModel(Guid id, Guid kernelOutputId, string messageType, DataLevel dataLevel) {
            _id = id;
            _kernelOutputId = kernelOutputId;
            _messageType = messageType;
            _dataLevel = dataLevel;
            messageType.TryParse(out _messageTypeEnum);
            this.Save = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(this.Keyword)) {
                    VirtualRoot.Out.ShowError("关键字不能为空", autoHideSeconds: 4);
                    return;
                }
                if (AppRoot.KernelOutputKeywordVms.GetListByKernelOutputId(this.KernelOutputId).Any(a => a.Id != this.Id && a.Keyword == this.Keyword)) {
                    throw new ValidationException($"关键字 {this.Keyword} 已经存在");
                }
                if (ClientAppType.IsMinerStudio) {
                    MinerStudio.MinerStudioRoot.Login(() => {
                        VirtualRoot.Execute(new AddOrUpdateKernelOutputKeywordCommand(this));
                        VirtualRoot.Execute(new CloseWindowCommand(this.Id));
                    });
                }
                else if (ClientAppType.IsMinerClient) {
                    VirtualRoot.Execute(new AddOrUpdateKernelOutputKeywordCommand(this));
                    VirtualRoot.Execute(new CloseWindowCommand(this.Id));
                }
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.IsReadOnly) {
                    return;
                }
                VirtualRoot.Execute(new EditKernelOutputKeywordCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (this.IsReadOnly) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{this.Keyword}内核输出关键字吗？", title: "确认", onYes: () => {
                    if (DevMode.IsDevMode) {
                        MinerStudio.MinerStudioRoot.Login(() => {
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

        private DataLevel _dataLevel;
        public DataLevel GetDataLevel() {
            return _dataLevel;
        }

        public bool IsReadOnly {
            get {
                if (ClientAppType.IsMinerClient) {
                    return _dataLevel == DataLevel.Global;
                }
                else if (ClientAppType.IsMinerStudio) {
                    return !MainMenuViewModel.Instance.IsMinerStudioOuterAdmin;
                }
                else {
                    return true;
                }
            }
        }

        public void SetDataLevel(DataLevel dataLevel) {
            this._dataLevel = dataLevel;
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
                value.TryParse(out _messageTypeEnum);
                OnPropertyChanged(nameof(MessageTypeIcon));
                OnPropertyChanged(nameof(IconFill));
                OnPropertyChanged(nameof(MessageTypeText));
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

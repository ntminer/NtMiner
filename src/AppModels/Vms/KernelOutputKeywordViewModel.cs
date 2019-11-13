using NTMiner.Core;
using NTMiner.MinerClient;
using System;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class KernelOutputKeywordViewModel : ViewModelBase, IKernelOutputKeyword {
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

        private readonly LocalMessageType _messageTypeEnum;
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
                if (NTMinerRoot.Instance.KernelOutputKeywordSet.Contains(this.KernelOutputId, this.Keyword)) {
                    VirtualRoot.Execute(new UpdateKernelOutputKeywordCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddKernelOutputKeywordCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                VirtualRoot.Execute(new KernelOutputKeywordEditCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowDialog(new DialogWindowViewModel(message: $"您确定删除{this.Keyword}内核输出关键字吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveKernelOutputKeywordCommand(this.Id));
                }));
            });
        }

        public LocalMessageType MessageTypeEnum {
            get { return _messageTypeEnum; }
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

        public void SetDataLevel(DataLevel dataLevel) {
            this.DataLevel = dataLevel;
        }

        public Guid GetId() {
            throw new NotImplementedException();
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

        public string Keyword {
            get => _keyword;
            set {
                _keyword = value;
                OnPropertyChanged(nameof(Keyword));
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

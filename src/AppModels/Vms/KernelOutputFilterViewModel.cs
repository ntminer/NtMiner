using NTMiner.Core;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelOutputFilterViewModel : ViewModelBase, IKernelOutputFilter, IEditableViewModel {
        private string _regexPattern;
        private Guid _id;

        public Guid GetId() {
            return this.Id;
        }

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public KernelOutputFilterViewModel(IKernelOutputFilter data) : this(data.GetId()) {
            _kernelOutputId = data.KernelOutputId;
            _regexPattern = data.RegexPattern;
            _id = data.GetId();
        }

        public KernelOutputFilterViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (NTMinerRoot.Instance.KernelOutputFilterSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdateKernelOutputFilterCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddKernelOutputFilterCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                VirtualRoot.Execute(new KernelOutputFilterEditCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowDialog(message: $"您确定删除{this.RegexPattern}内核输出过滤器吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveKernelOutputFilterCommand(this.Id));
                }, icon: IconConst.IconConfirm);
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
    }
}

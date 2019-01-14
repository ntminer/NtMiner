using NTMiner.Core;
using NTMiner.Core.Kernels;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelOutputFilterViewModel : ViewModelBase, IKernelOutputFilter {
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
                if (NTMinerRoot.Current.KernelOutputFilterSet.Contains(this.Id)) {
                    Global.Execute(new UpdateKernelOutputFilterCommand(this));
                }
                else {
                    Global.Execute(new AddKernelOutputFilterCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand(() => {
                KernelOutputFilterEdit.ShowEditWindow(this);
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.RegexPattern}内核输出过滤器吗？", title: "确认", onYes: () => {
                    Global.Execute(new RemoveKernelOutputFilterCommand(this.Id));
                }, icon: "Icon_Confirm");
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

        public string RegexPattern {
            get => _regexPattern;
            set {
                _regexPattern = value;
                OnPropertyChanged(nameof(RegexPattern));
            }
        }
    }
}

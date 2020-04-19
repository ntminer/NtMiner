using NTMiner.Core;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class FragmentWriterViewModel : ViewModelBase, IEditableViewModel, IFragmentWriter {
        private Guid _id;
        private string _name;
        private string _body;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public FragmentWriterViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        public FragmentWriterViewModel(IFragmentWriter data) : this(data.GetId()) {
            _name = data.Name;
            _body = data.Body;
        }

        public FragmentWriterViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (NTMinerContext.Instance.ServerContext.FragmentWriterSet.TryGetFragmentWriter(this.Id, out IFragmentWriter writer)) {
                    VirtualRoot.Execute(new UpdateFragmentWriterCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddFragmentWriterCommand(this));
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                VirtualRoot.Execute(new EditFragmentWriterCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{this.Name}组吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveFragmentWriterCommand(this.Id));
                }));
            });
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

        public string Name {
            get { return _name; }
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Body {
            get => _body;
            set {
                _body = value;
                OnPropertyChanged(nameof(Body));
            }
        }
    }
}

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

        public Action CloseWindow { get; set; }

        public FragmentWriterViewModel() {
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
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
                if (NTMinerRoot.Instance.FragmentWriterSet.TryGetFragmentWriter(this.Id, out IFragmentWriter writer)) {
                    VirtualRoot.Execute(new UpdateFragmentWriterCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddFragmentWriterCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                VirtualRoot.Execute(new FragmentWriterEditCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowDialog(message: $"您确定删除{this.Name}组吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveFragmentWriterCommand(this.Id));
                }, icon: IconConst.IconConfirm);
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

using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class PackageViewModel : ViewModelBase, IEditableViewModel, IPackage {
        private Guid _id;
        private string _name;
        private List<Guid> _algoIds;
        private List<AlgoSelectItem> _algoSelectItems = new List<AlgoSelectItem>();

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        public ICommand BrowsePackage { get; private set; }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public PackageViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        public PackageViewModel(IPackage data) : this(data.GetId()) {
            _name = data.Name;
            AlgoIds = data.AlgoIds;
        }

        public PackageViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                _algoIds = this.AlgoSelectItems.Where(a => a.IsChecked).Select(a => a.SysDicItemVm.Id).ToList();
                if (NTMinerRoot.Instance.ServerContext.PackageSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdatePackageCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddPackageCommand(this));
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                VirtualRoot.Execute(new PackageEditCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{this.Name}内核包吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemovePackageCommand(this.Id));
                }));
            });
            this.BrowsePackage = new DelegateCommand(() => {
                OpenFileDialog openFileDialog = new OpenFileDialog {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    Filter = "zip (*.zip)|*.zip",
                    FilterIndex = 1
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    string package = Path.GetFileName(openFileDialog.FileName);
                    this.Name = package;
                }
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
            get => _name;
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public List<Guid> AlgoIds {
            get => _algoIds;
            set {
                _algoIds = value;
                OnPropertyChanged(nameof(AlgoIds));
                var list = new List<AlgoSelectItem>();
                foreach (var item in AppContext.Instance.SysDicItemVms.AlgoItems) {
                    list.Add(new AlgoSelectItem(item, value != null && value.Contains(item.Id)));
                }
                AlgoSelectItems = list;
            }
        }

        public List<AlgoSelectItem> AlgoSelectItems {
            get => _algoSelectItems;
            private set {
                _algoSelectItems = value;
                OnPropertyChanged(nameof(AlgoSelectItems));
                OnPropertyChanged(nameof(AlgosText));
            }
        }

        public string AlgosText {
            get {
                return string.Join(",", AlgoSelectItems.Where(a => a.IsChecked).Select(a => a.SysDicItemVm.Value));
            }
        }
    }
}

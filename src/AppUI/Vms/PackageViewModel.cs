using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class PackageViewModel : ViewModelBase, IPackage {
        private Guid _id;
        private string _name;
        private List<Guid> _algoIds;
        private List<AlgoSelectItem> _algoSelectItems;

        public PackageViewModel() {
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public PackageViewModel(IPackage data) : this(data.GetId()) {
            _name = data.Name;
            AlgoIds = data.AlgoIds;
        }

        public PackageViewModel(Guid id) {
            _id = id;
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
                    list.Add(new AlgoSelectItem(item, value.Contains(item.Id)));
                }
                AlgoSelectItems = list;
            }
        }

        public List<AlgoSelectItem> AlgoSelectItems {
            get => _algoSelectItems;
            private set {
                _algoSelectItems = value;
                OnPropertyChanged(nameof(AlgoSelectItems));
            }
        }
    }
}

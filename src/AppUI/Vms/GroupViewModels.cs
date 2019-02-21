using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class GroupViewModels : ViewModelBase {
        public static readonly GroupViewModels Current = new GroupViewModels();

        private readonly Dictionary<Guid, GroupViewModel> _dicById = new Dictionary<Guid, GroupViewModel>();
        public ICommand Add { get; private set; }
        private GroupViewModels() {            
            this.Add = new DelegateCommand(() => {
                new GroupViewModel(Guid.NewGuid()) {
                    SortNumber = Count + 1
                }.Edit.Execute(null);
            });
            VirtualRoot.On<GroupAddedEvent>(
                "添加了组后调整VM内存",
                LogEnum.Console,
                action: (message) => {
                    if (!_dicById.ContainsKey(message.Source.GetId())) {
                        GroupViewModel groupVm = new GroupViewModel(message.Source);
                        _dicById.Add(message.Source.GetId(), groupVm);
                        OnPropertyChangeds();
                    }
                });
            VirtualRoot.On<GroupUpdatedEvent>(
                "更新了组后调整VM内存",
                LogEnum.Console,
                action: (message) => {
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        GroupViewModel entity = _dicById[message.Source.GetId()];
                        int sortNumber = entity.SortNumber;
                        entity.Update(message.Source);
                        if (sortNumber != entity.SortNumber) {
                            this.OnPropertyChanged(nameof(List));
                            OnPropertyChanged(nameof(SelectionOptions));
                        }
                    }
                });
            VirtualRoot.On<GroupRemovedEvent>(
                "删除了组后调整VM内存",
                LogEnum.Console,
                action: (message) => {
                    _dicById.Remove(message.Source.GetId());
                    OnPropertyChangeds();
                });
            Init();
        }

        private void Init() {
            foreach (var item in NTMinerRoot.Current.GroupSet) {
                GroupViewModel groupVm = new GroupViewModel(item);
                _dicById.Add(item.GetId(), groupVm);
            }
        }

        private void OnPropertyChangeds() {
            OnPropertyChanged(nameof(List));
            OnPropertyChanged(nameof(SelectionOptions));
            OnPropertyChanged(nameof(Count));
        }

        public bool TryGetGroupVm(Guid groupId, out GroupViewModel groupVm) {
            return _dicById.TryGetValue(groupId, out groupVm);
        }

        public int Count {
            get {
                return _dicById.Count;
            }
        }

        public List<GroupViewModel> List {
            get {
                return _dicById.Values.OrderBy(a => a.SortNumber).ToList();
            }
        }

        private IEnumerable<GroupViewModel> GetSelectionItems() {
            yield return GroupViewModel.PleaseSelect;
            foreach (var item in _dicById.Values.OrderBy(a => a.SortNumber)) {
                yield return item;
            }
        }

        public List<GroupViewModel> SelectionOptions {
            get {
                return GetSelectionItems().ToList();
            }
        }
    }
}

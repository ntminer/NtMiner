using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public static partial class AppRoot {
        public class GroupViewModels : ViewModelBase {
            public static GroupViewModels Instance { get; private set; } = new GroupViewModels();
            private readonly Dictionary<Guid, GroupViewModel> _dicById = new Dictionary<Guid, GroupViewModel>();
            public ICommand Add { get; private set; }
            private GroupViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                this.Add = new DelegateCommand(() => {
                    new GroupViewModel(Guid.NewGuid()) {
                        SortNumber = Count + 1
                    }.Edit.Execute(FormType.Add);
                });
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        _dicById.Clear();
                        Init();
                    });
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("刷新视图界面", LogEnum.DevConsole, location: this.GetType(), PathPriority.BelowNormal,
                    path: message => {
                        OnPropertyChangeds();
                    });
                BuildEventPath<GroupAddedEvent>("调整VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: (message) => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            GroupViewModel groupVm = new GroupViewModel(message.Source);
                            _dicById.Add(message.Source.GetId(), groupVm);
                            OnPropertyChangeds();
                        }
                    });
                BuildEventPath<GroupUpdatedEvent>("调整VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: (message) => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out GroupViewModel vm)) {
                            int sortNumber = vm.SortNumber;
                            vm.Update(message.Source);
                            if (sortNumber != vm.SortNumber) {
                                this.OnPropertyChanged(nameof(List));
                                OnPropertyChanged(nameof(SelectionOptions));
                            }
                        }
                    });
                BuildEventPath<GroupRemovedEvent>("调整VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: (message) => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChangeds();
                    });
                Init();
            }

            private void Init() {
                foreach (var item in NTMinerContext.Instance.ServerContext.GroupSet.AsEnumerable().ToArray()) {
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
}

using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public static partial class AppRoot {
        public class GroupViewModels : ViewModelBase {
            public static readonly GroupViewModels Instance = new GroupViewModels();
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
                VirtualRoot.AddEventPath<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        Init();
                    }, location: this.GetType());
                VirtualRoot.AddEventPath<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        OnPropertyChangeds();
                    }, location: this.GetType());
                AddEventPath<GroupAddedEvent>("添加了组后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            GroupViewModel groupVm = new GroupViewModel(message.Source);
                            _dicById.Add(message.Source.GetId(), groupVm);
                            OnPropertyChangeds();
                        }
                    }, location: this.GetType());
                AddEventPath<GroupUpdatedEvent>("更新了组后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out GroupViewModel vm)) {
                            int sortNumber = vm.SortNumber;
                            vm.Update(message.Source);
                            if (sortNumber != vm.SortNumber) {
                                this.OnPropertyChanged(nameof(List));
                                OnPropertyChanged(nameof(SelectionOptions));
                            }
                        }
                    }, location: this.GetType());
                AddEventPath<GroupRemovedEvent>("删除了组后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChangeds();
                    }, location: this.GetType());
                Init();
            }

            private void Init() {
                foreach (var item in NTMinerContext.Instance.ServerContext.GroupSet.AsEnumerable()) {
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

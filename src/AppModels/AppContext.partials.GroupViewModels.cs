using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public partial class AppContext {
        public class GroupViewModels : ViewModelBase {
            public static readonly GroupViewModels Instance = new GroupViewModels();
            private readonly Dictionary<Guid, GroupViewModel> _dicById = new Dictionary<Guid, GroupViewModel>();
            public ICommand Add { get; private set; }
            private GroupViewModels() {
#if DEBUG
                Write.Stopwatch.Start();
#endif
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
                        if (!_dicById.ContainsKey(message.Target.GetId())) {
                            GroupViewModel groupVm = new GroupViewModel(message.Target);
                            _dicById.Add(message.Target.GetId(), groupVm);
                            OnPropertyChangeds();
                        }
                    }, location: this.GetType());
                AddEventPath<GroupUpdatedEvent>("更新了组后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (_dicById.ContainsKey(message.Target.GetId())) {
                            GroupViewModel entity = _dicById[message.Target.GetId()];
                            int sortNumber = entity.SortNumber;
                            entity.Update(message.Target);
                            if (sortNumber != entity.SortNumber) {
                                this.OnPropertyChanged(nameof(List));
                                OnPropertyChanged(nameof(SelectionOptions));
                            }
                        }
                    }, location: this.GetType());
                AddEventPath<GroupRemovedEvent>("删除了组后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Remove(message.Target.GetId());
                        OnPropertyChangeds();
                    }, location: this.GetType());
                Init();
#if DEBUG
                var elapsedMilliseconds = Write.Stopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.ServerContext.GroupSet.AsEnumerable()) {
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

            public GroupViewModel GetNextOne(int sortNumber) {
                return List.OrderBy(a => a.SortNumber).FirstOrDefault(a => a.SortNumber > sortNumber);
            }

            public GroupViewModel GetUpOne(int sortNumber) {
                return List.OrderByDescending(a => a.SortNumber).FirstOrDefault(a => a.SortNumber < sortNumber);
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

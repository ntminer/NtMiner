using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public static partial class AppRoot {
        public class FragmentWriterViewModels : ViewModelBase {
            public static FragmentWriterViewModels Instance { get; private set; } = new FragmentWriterViewModels();
            private readonly Dictionary<Guid, FragmentWriterViewModel> _dicById = new Dictionary<Guid, FragmentWriterViewModel>();
            public ICommand Add { get; private set; }
            private FragmentWriterViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                this.Add = new DelegateCommand(() => {
                    new FragmentWriterViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
                });
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("刷新VM内存", LogEnum.DevConsole,
                    path: message => {
                        _dicById.Clear();
                        Init();
                    }, location: this.GetType());
                VirtualRoot.BuildEventPath<ServerContextReInitedEventHandledEvent>("刷新视图界面", LogEnum.DevConsole,
                    path: message => {
                        OnPropertyChangeds();
                    }, location: this.GetType());
                BuildEventPath<FragmentWriterAddedEvent>("调整VM内存", LogEnum.DevConsole,
                    path: (message) => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            FragmentWriterViewModel vm = new FragmentWriterViewModel(message.Source);
                            _dicById.Add(message.Source.GetId(), vm);
                            OnPropertyChangeds();
                        }
                    }, location: this.GetType());
                BuildEventPath<FragmentWriterUpdatedEvent>("调整VM内存", LogEnum.DevConsole,
                    path: (message) => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out FragmentWriterViewModel vm)) {
                            vm.Update(message.Source);
                        }
                    }, location: this.GetType());
                BuildEventPath<FragmentWriterRemovedEvent>("调整VM内存", LogEnum.DevConsole,
                    path: (message) => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChangeds();
                    }, location: this.GetType());
                Init();
            }

            private void Init() {
                foreach (var item in NTMinerContext.Instance.ServerContext.FragmentWriterSet.AsEnumerable().ToArray()) {
                    FragmentWriterViewModel groupVm = new FragmentWriterViewModel(item);
                    _dicById.Add(item.GetId(), groupVm);
                }
            }

            private void OnPropertyChangeds() {
                OnPropertyChanged(nameof(List));
            }

            public bool TryGetFragmentWriterVm(Guid groupId, out FragmentWriterViewModel groupVm) {
                return _dicById.TryGetValue(groupId, out groupVm);
            }

            public List<FragmentWriterViewModel> List {
                get {
                    return _dicById.Values.ToList();
                }
            }
        }
    }
}

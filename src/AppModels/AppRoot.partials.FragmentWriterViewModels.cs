using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public static partial class AppRoot {
        public class FragmentWriterViewModels : ViewModelBase {
            public static readonly FragmentWriterViewModels Instance = new FragmentWriterViewModels();
            private readonly Dictionary<Guid, FragmentWriterViewModel> _dicById = new Dictionary<Guid, FragmentWriterViewModel>();
            public ICommand Add { get; private set; }
            private FragmentWriterViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                this.Add = new DelegateCommand(() => {
                    new FragmentWriterViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
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
                AddEventPath<FragmentWriterAddedEvent>("添加了命令行片段书写器后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            FragmentWriterViewModel groupVm = new FragmentWriterViewModel(message.Source);
                            _dicById.Add(message.Source.GetId(), groupVm);
                            OnPropertyChangeds();
                        }
                    }, location: this.GetType());
                AddEventPath<FragmentWriterUpdatedEvent>("更新了命令行片段书写器后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out FragmentWriterViewModel vm)) {
                            vm.Update(message.Source);
                        }
                    }, location: this.GetType());
                AddEventPath<FragmentWriterRemovedEvent>("删除了命令行片段书写器后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChangeds();
                    }, location: this.GetType());
                Init();
            }

            private void Init() {
                foreach (var item in NTMinerContext.Instance.ServerContext.FragmentWriterSet.AsEnumerable()) {
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

using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public partial class AppContext {
        public class FragmentWriterViewModels : ViewModelBase {
            public static readonly FragmentWriterViewModels Instance = new FragmentWriterViewModels();
            private readonly Dictionary<Guid, FragmentWriterViewModel> _dicById = new Dictionary<Guid, FragmentWriterViewModel>();
            public ICommand Add { get; private set; }
            private FragmentWriterViewModels() {
#if DEBUG
                VirtualRoot.Stopwatch.Restart();
#endif
                this.Add = new DelegateCommand(() => {
                    new FragmentWriterViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
                });
                VirtualRoot.On<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        Init();
                    });
                VirtualRoot.On<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        OnPropertyChangeds();
                    });
                On<FragmentWriterAddedEvent>("添加了命令行片段书写器后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            FragmentWriterViewModel groupVm = new FragmentWriterViewModel(message.Source);
                            _dicById.Add(message.Source.GetId(), groupVm);
                            OnPropertyChangeds();
                        }
                    });
                On<FragmentWriterUpdatedEvent>("更新了命令行片段书写器后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (_dicById.ContainsKey(message.Source.GetId())) {
                            FragmentWriterViewModel entity = _dicById[message.Source.GetId()];
                            entity.Update(message.Source);
                        }
                    });
                On<FragmentWriterRemovedEvent>("删除了命令行片段书写器后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChangeds();
                    });
                Init();
#if DEBUG
                Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.FragmentWriterSet) {
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

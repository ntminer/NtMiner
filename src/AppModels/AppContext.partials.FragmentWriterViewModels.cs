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
                Write.Stopwatch.Start();
#endif
                this.Add = new DelegateCommand(() => {
                    new FragmentWriterViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
                });
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        Init();
                    });
                VirtualRoot.BuildEventPath<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        OnPropertyChangeds();
                    });
                BuildEventPath<FragmentWriterAddedEvent>("添加了命令行片段书写器后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (!_dicById.ContainsKey(message.Target.GetId())) {
                            FragmentWriterViewModel groupVm = new FragmentWriterViewModel(message.Target);
                            _dicById.Add(message.Target.GetId(), groupVm);
                            OnPropertyChangeds();
                        }
                    });
                BuildEventPath<FragmentWriterUpdatedEvent>("更新了命令行片段书写器后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (_dicById.ContainsKey(message.Target.GetId())) {
                            FragmentWriterViewModel entity = _dicById[message.Target.GetId()];
                            entity.Update(message.Target);
                        }
                    });
                BuildEventPath<FragmentWriterRemovedEvent>("删除了命令行片段书写器后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Remove(message.Target.GetId());
                        OnPropertyChangeds();
                    });
                Init();
#if DEBUG
                var elapsedMilliseconds = Write.Stopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.ServerContext.FragmentWriterSet.AsEnumerable()) {
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

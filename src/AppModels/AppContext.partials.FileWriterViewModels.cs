using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public partial class AppContext {
        public class FileWriterViewModels : ViewModelBase {
            public static readonly FileWriterViewModels Instance = new FileWriterViewModels();
            private readonly Dictionary<Guid, FileWriterViewModel> _dicById = new Dictionary<Guid, FileWriterViewModel>();
            public ICommand Add { get; private set; }
            private FileWriterViewModels() {
#if DEBUG
                Write.Stopwatch.Restart();
#endif
                this.Add = new DelegateCommand(() => {
                    new FileWriterViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
                });
                VirtualRoot.CreateEventPath<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        Init();
                    });
                VirtualRoot.CreateEventPath<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        OnPropertyChangeds();
                    });
                EventPath<FileWriterAddedEvent>("添加了文件书写器后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            FileWriterViewModel groupVm = new FileWriterViewModel(message.Source);
                            _dicById.Add(message.Source.GetId(), groupVm);
                            OnPropertyChangeds();
                        }
                    });
                EventPath<FileWriterUpdatedEvent>("更新了文件书写器后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (_dicById.ContainsKey(message.Source.GetId())) {
                            FileWriterViewModel entity = _dicById[message.Source.GetId()];
                            entity.Update(message.Source);
                        }
                    });
                EventPath<FileWriterRemovedEvent>("删除了文件书写器后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChangeds();
                    });
                Init();
#if DEBUG
                Write.DevTimeSpan($"耗时{Write.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.FileWriterSet) {
                    FileWriterViewModel groupVm = new FileWriterViewModel(item);
                    _dicById.Add(item.GetId(), groupVm);
                }
            }

            private void OnPropertyChangeds() {
                OnPropertyChanged(nameof(List));
            }

            public bool TryGetFileWriterVm(Guid groupId, out FileWriterViewModel groupVm) {
                return _dicById.TryGetValue(groupId, out groupVm);
            }

            public List<FileWriterViewModel> List {
                get {
                    return _dicById.Values.ToList();
                }
            }
        }
    }
}

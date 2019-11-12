using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public partial class AppContext {
        public class SysDicViewModels : ViewModelBase {
            public static readonly SysDicViewModels Instance = new SysDicViewModels();
            private readonly Dictionary<Guid, SysDicViewModel> _dicById = new Dictionary<Guid, SysDicViewModel>();
            private readonly Dictionary<string, SysDicViewModel> _dicByCode = new Dictionary<string, SysDicViewModel>(StringComparer.OrdinalIgnoreCase);

            public ICommand Add { get; private set; }
            private SysDicViewModels() {
#if DEBUG
                Write.Stopwatch.Start();
#endif
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicByCode.Clear();
                        _dicById.Clear();
                        Init();
                    });
                VirtualRoot.BuildEventPath<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        OnPropertyChangeds();
                    });
                this.Add = new DelegateCommand(() => {
                    new SysDicViewModel(Guid.NewGuid()).Edit.Execute(null);
                });
                BuildEventPath<SysDicAddedEvent>("添加了系统字典后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            SysDicViewModel sysDicVm = new SysDicViewModel(message.Source);
                            _dicById.Add(message.Source.GetId(), sysDicVm);
                            if (!_dicByCode.ContainsKey(message.Source.Code)) {
                                _dicByCode.Add(message.Source.Code, sysDicVm);
                            }
                            OnPropertyChangeds();
                        }
                    });
                BuildEventPath<SysDicUpdatedEvent>("更新了系统字典后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (_dicById.ContainsKey(message.Source.GetId())) {
                            SysDicViewModel entity = _dicById[message.Source.GetId()];
                            int sortNumber = entity.SortNumber;
                            entity.Update(message.Source);
                            if (sortNumber != entity.SortNumber) {
                                this.OnPropertyChanged(nameof(List));
                            }
                        }
                    });
                BuildEventPath<SysDicRemovedEvent>("删除了系统字典后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Remove(message.Source.GetId());
                        _dicByCode.Remove(message.Source.Code);
                        OnPropertyChangeds();
                    });
                Init();
#if DEBUG
                var elapsedMilliseconds = Write.Stopwatch.Stop();
                Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.ServerContext.SysDicSet) {
                    SysDicViewModel sysDicVm = new SysDicViewModel(item);
                    _dicById.Add(item.GetId(), sysDicVm);
                    _dicByCode.Add(item.Code, sysDicVm);
                }
            }

            private void OnPropertyChangeds() {
                OnPropertyChanged(nameof(List));
                OnPropertyChanged(nameof(Count));
            }

            public bool TryGetSysDicVm(Guid dicId, out SysDicViewModel sysDicVm) {
                return _dicById.TryGetValue(dicId, out sysDicVm);
            }

            public bool TryGetSysDicVm(string dicCode, out SysDicViewModel sysDicVm) {
                return _dicByCode.TryGetValue(dicCode, out sysDicVm);
            }

            public int Count {
                get {
                    return _dicById.Count;
                }
            }

            public List<SysDicViewModel> List {
                get {
                    return _dicById.Values.OrderBy(a => a.SortNumber).ToList();
                }
            }

            public SysDicViewModel GetUpOne(int sortNumber) {
                return List.OrderByDescending(a => a.SortNumber).FirstOrDefault(a => a.SortNumber < sortNumber); ;
            }

            public SysDicViewModel GetNextOne(int sortNumber) {
                return List.OrderBy(a => a.SortNumber).FirstOrDefault(a => a.SortNumber > sortNumber);
            }
        }
    }
}

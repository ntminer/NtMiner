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
                NTStopwatch.Start();
#endif
                VirtualRoot.AddEventPath<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicByCode.Clear();
                        _dicById.Clear();
                        Init();
                    }, location: this.GetType());
                VirtualRoot.AddEventPath<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        OnPropertyChangeds();
                    }, location: this.GetType());
                this.Add = new DelegateCommand(() => {
                    new SysDicViewModel(Guid.NewGuid()).Edit.Execute(null);
                });
                AddEventPath<SysDicAddedEvent>("添加了系统字典后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (!_dicById.ContainsKey(message.Target.GetId())) {
                            SysDicViewModel sysDicVm = new SysDicViewModel(message.Target);
                            _dicById.Add(message.Target.GetId(), sysDicVm);
                            if (!_dicByCode.ContainsKey(message.Target.Code)) {
                                _dicByCode.Add(message.Target.Code, sysDicVm);
                            }
                            OnPropertyChangeds();
                        }
                    }, location: this.GetType());
                AddEventPath<SysDicUpdatedEvent>("更新了系统字典后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (_dicById.ContainsKey(message.Target.GetId())) {
                            SysDicViewModel entity = _dicById[message.Target.GetId()];
                            int sortNumber = entity.SortNumber;
                            entity.Update(message.Target);
                            if (sortNumber != entity.SortNumber) {
                                this.OnPropertyChanged(nameof(List));
                            }
                        }
                    }, location: this.GetType());
                AddEventPath<SysDicRemovedEvent>("删除了系统字典后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Remove(message.Target.GetId());
                        _dicByCode.Remove(message.Target.Code);
                        OnPropertyChangeds();
                    }, location: this.GetType());
                Init();
#if DEBUG
                var elapsedMilliseconds = NTStopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.ServerContext.SysDicSet.AsEnumerable()) {
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
        }
    }
}

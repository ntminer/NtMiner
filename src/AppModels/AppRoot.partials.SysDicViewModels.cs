using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public static partial class AppRoot {
        public class SysDicViewModels : ViewModelBase {
            public static SysDicViewModels Instance { get; private set; } = new SysDicViewModels();
            private readonly Dictionary<Guid, SysDicViewModel> _dicById = new Dictionary<Guid, SysDicViewModel>();
            private readonly Dictionary<string, SysDicViewModel> _dicByCode = new Dictionary<string, SysDicViewModel>(StringComparer.OrdinalIgnoreCase);

            public ICommand Add { get; private set; }
            private SysDicViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("刷新VM内存", LogEnum.DevConsole,
                    path: message => {
                        _dicByCode.Clear();
                        _dicById.Clear();
                        Init();
                    }, location: this.GetType());
                VirtualRoot.BuildEventPath<ServerContextReInitedEventHandledEvent>("刷新视图界面", LogEnum.DevConsole,
                    path: message => {
                        OnPropertyChangeds();
                    }, location: this.GetType());
                this.Add = new DelegateCommand(() => {
                    new SysDicViewModel(Guid.NewGuid()).Edit.Execute(null);
                });
                BuildEventPath<SysDicAddedEvent>("调整VM内存", LogEnum.DevConsole,
                    path: (message) => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            SysDicViewModel sysDicVm = new SysDicViewModel(message.Source);
                            _dicById.Add(message.Source.GetId(), sysDicVm);
                            if (!_dicByCode.ContainsKey(message.Source.Code)) {
                                _dicByCode.Add(message.Source.Code, sysDicVm);
                            }
                            OnPropertyChangeds();
                        }
                    }, location: this.GetType());
                BuildEventPath<SysDicUpdatedEvent>("调整VM内存", LogEnum.DevConsole,
                    path: (message) => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out SysDicViewModel vm)) {
                            int sortNumber = vm.SortNumber;
                            vm.Update(message.Source);
                            if (sortNumber != vm.SortNumber) {
                                this.OnPropertyChanged(nameof(List));
                            }
                        }
                    }, location: this.GetType());
                BuildEventPath<SysDicRemovedEvent>("调整VM内存", LogEnum.DevConsole,
                    path: (message) => {
                        _dicById.Remove(message.Source.GetId());
                        _dicByCode.Remove(message.Source.Code);
                        OnPropertyChangeds();
                    }, location: this.GetType());
                Init();
            }

            private void Init() {
                foreach (var item in NTMinerContext.Instance.ServerContext.SysDicSet.AsEnumerable()) {
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

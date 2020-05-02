using NTMiner.MinerStudio.Vms;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.MinerStudio {
    public static partial class MinerStudioRoot {
        public class MinerGroupViewModels : ViewModelBase {
            public static readonly MinerGroupViewModels Instance = new MinerGroupViewModels();
            private readonly Dictionary<Guid, MinerGroupViewModel> _dicById = new Dictionary<Guid, MinerGroupViewModel>();

            public ICommand Add { get; private set; }

            private MinerGroupViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                foreach (var item in NTMinerContext.MinerStudioContext.MinerGroupSet.AsEnumerable()) {
                    if (!_dicById.ContainsKey(item.Id)) {
                        _dicById.Add(item.Id, new MinerGroupViewModel(item));
                    }
                }
                AppRoot.AddEventPath<MinerGroupSetInitedEvent>("矿工组集初始化后初始化Vm内存", LogEnum.DevConsole, action: message => {
                    foreach (var item in NTMinerContext.MinerStudioContext.MinerGroupSet.AsEnumerable()) {
                        if (!_dicById.ContainsKey(item.Id)) {
                            _dicById.Add(item.Id, new MinerGroupViewModel(item));
                        }
                    }
                    this.OnPropertyChangeds();
                    MinerClientsWindowViewModel.Instance.RefreshMinerClientsSelectedMinerGroup(MinerClientsWindowViewModel.Instance.MinerClients.ToArray());
                }, this.GetType());
                this.Add = new DelegateCommand(() => {
                    new MinerGroupViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
                });
                AppRoot.AddEventPath<MinerGroupAddedEvent>("添加矿机分组后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (!_dicById.TryGetValue(message.Source.GetId(), out MinerGroupViewModel vm)) {
                            vm = new MinerGroupViewModel(message.Source);
                            _dicById.Add(message.Source.GetId(), vm);
                            OnPropertyChangeds();
                            MinerClientsWindowVm.OnPropertyChanged(nameof(MinerClientsWindowViewModel.SelectedMinerGroup));
                        }
                    }, location: this.GetType());
                AppRoot.AddEventPath<MinerGroupUpdatedEvent>("添加矿机分组后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out MinerGroupViewModel vm)) {
                            vm.Update(message.Source);
                        }
                    }, location: this.GetType());
                AppRoot.AddEventPath<MinerGroupRemovedEvent>("移除了矿机组后刷新Vm内容", LogEnum.DevConsole, action: message => {
                    if (_dicById.TryGetValue(message.Source.Id, out MinerGroupViewModel vm)) {
                        _dicById.Remove(vm.Id);
                        OnPropertyChangeds();
                        MinerClientsWindowVm.OnPropertyChanged(nameof(MinerClientsWindowViewModel.SelectedMinerGroup));
                    }
                }, this.GetType());
            }

            private void OnPropertyChangeds() {
                OnPropertyChanged(nameof(List));
                OnPropertyChanged(nameof(MinerGroupItems));
            }

            public List<MinerGroupViewModel> List {
                get {
                    return _dicById.Values.ToList();
                }
            }

            public bool TryGetMineWorkVm(Guid id, out MinerGroupViewModel minerGroupVm) {
                return _dicById.TryGetValue(id, out minerGroupVm);
            }

            private IEnumerable<MinerGroupViewModel> GetMinerGroupItems() {
                yield return MinerGroupViewModel.PleaseSelect;
                foreach (var item in List) {
                    yield return item;
                }
            }

            public List<MinerGroupViewModel> MinerGroupItems {
                get {
                    return GetMinerGroupItems().ToList();
                }
            }
        }
    }
}

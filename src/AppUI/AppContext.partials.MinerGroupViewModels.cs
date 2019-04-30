using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public partial class AppContext {
        public class MinerGroupViewModels : ViewModelBase {
            private readonly Dictionary<Guid, MinerGroupViewModel> _dicById = new Dictionary<Guid, MinerGroupViewModel>();

            public ICommand Add { get; private set; }

            public MinerGroupViewModels() {
                if (Design.IsInDesignMode) {
                    return;
                }
                foreach (var item in NTMinerRoot.Instance.MinerGroupSet) {
                    _dicById.Add(item.GetId(), new MinerGroupViewModel(item));
                }
                this.Add = new DelegateCommand(() => {
                    new MinerGroupViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
                });
                VirtualRoot.On<MinerGroupAddedEvent>("添加矿机分组后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            _dicById.Add(message.Source.GetId(), new MinerGroupViewModel(message.Source));
                            OnPropertyChanged(nameof(List));
                            OnPropertyChanged(nameof(MinerGroupItems));
                            Current.MinerClientsWindowVm.OnPropertyChanged(nameof(MinerClientsWindowViewModel.SelectedMinerGroup));
                        }
                    }).AddToCollection(ContextHandlers);
                VirtualRoot.On<MinerGroupUpdatedEvent>("更新矿机分组后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById[message.Source.GetId()].Update(message.Source);
                    }).AddToCollection(ContextHandlers);
                VirtualRoot.On<MinerGroupRemovedEvent>("删除矿机分组后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChanged(nameof(List));
                        OnPropertyChanged(nameof(MinerGroupItems));
                        Current.MinerClientsWindowVm.OnPropertyChanged(nameof(MinerClientsWindowViewModel.SelectedMinerGroup));
                    }).AddToCollection(ContextHandlers);
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

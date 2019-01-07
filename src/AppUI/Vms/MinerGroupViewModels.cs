using NTMiner.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MinerGroupViewModels : ViewModelBase, IEnumerable<MinerGroupViewModel> {
        public static readonly MinerGroupViewModels Current = new MinerGroupViewModels();
        private readonly Dictionary<Guid, MinerGroupViewModel> _dicById = new Dictionary<Guid, MinerGroupViewModel>();

        public ICommand Add { get; private set; }

        private MinerGroupViewModels() {
            if (Design.IsInDesignMode) {
                return;
            }
            foreach (var item in NTMinerRoot.Current.MinerGroupSet) {
                _dicById.Add(item.GetId(), new MinerGroupViewModel(item));
            }
            this.Add = new DelegateCommand(() => {
                new MinerGroupViewModel(Guid.NewGuid()).Edit.Execute(null);
            });
            Global.Access<MinerGroupAddedEvent>(
                Guid.Parse("d5673d55-4920-42a7-8f2c-f81f474ff83c"),
                "添加矿工组后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    if (!_dicById.ContainsKey(message.Source.GetId())) {
                        _dicById.Add(message.Source.GetId(), new MinerGroupViewModel(message.Source.GetId()).Update(message.Source));
                        OnPropertyChanged(nameof(List));
                        MinerClientsViewModel.Current.OnPropertyChanged(nameof(MinerClientsViewModel.MinerGroupVmItems));
                        MinerClientsViewModel.Current.OnPropertyChanged(nameof(MinerClientsViewModel.SelectedMinerGroup));
                    }
                });
            Global.Access<MinerGroupUpdatedEvent>(
                Guid.Parse("bfb4f809-bb5d-48ce-8c75-1dffaf771e7e"),
                "更新矿工组后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    _dicById[message.Source.GetId()].Update(message.Source);
                });
            Global.Access<MinerGroupRemovedEvent>(
                Guid.Parse("0b412543-bfb0-47c7-85c9-320819bb5ada"),
                "删除矿工组后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    _dicById.Remove(message.Source.GetId());
                    OnPropertyChanged(nameof(List));
                    MinerClientsViewModel.Current.OnPropertyChanged(nameof(MinerClientsViewModel.MinerGroupVmItems));
                    MinerClientsViewModel.Current.OnPropertyChanged(nameof(MinerClientsViewModel.SelectedMinerGroup));
                });
        }

        public List<MinerGroupViewModel> List {
            get {
                return _dicById.Values.ToList();
            }
        }

        public bool TryGetMineWorkVm(Guid id, out MinerGroupViewModel minerGroupVm) {
            return _dicById.TryGetValue(id, out minerGroupVm);
        }

        public IEnumerator<MinerGroupViewModel> GetEnumerator() {
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _dicById.Values.GetEnumerator();
        }
    }
}

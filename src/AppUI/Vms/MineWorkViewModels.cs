using NTMiner.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MineWorkViewModels : ViewModelBase, IEnumerable<MineWorkViewModel> {
        public static readonly MineWorkViewModels Current = new MineWorkViewModels();

        private readonly Dictionary<Guid, MineWorkViewModel> _dicById = new Dictionary<Guid, MineWorkViewModel>();
        public ICommand Add { get; private set; }

        private MineWorkViewModels() {
            if (Design.IsInDesignMode) {
                return;
            }
            foreach (var item in NTMinerRoot.Current.MineWorkSet) {
                _dicById.Add(item.GetId(), new MineWorkViewModel(item));
            }
            this.Add = new DelegateCommand(() => {
                new MineWorkViewModel(Guid.NewGuid()).Edit.Execute(null);
            });
            VirtualRoot.Access<MineWorkAddedEvent>(
                Guid.Parse("3fcceca1-add2-4a35-9609-a936be6885b2"),
                "添加作业后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    if (!_dicById.ContainsKey(message.Source.GetId())) {
                        _dicById.Add(message.Source.GetId(), new MineWorkViewModel(message.Source));
                        OnPropertyChanged(nameof(List));
                        OnPropertyChanged(nameof(MineWorkVmItems));
                        OnPropertyChanged(nameof(MineWorkItems));
                        if (message.Source.GetId() == MinerClientsViewModel.Current.SelectedMineWork.GetId()) {
                            MinerClientsViewModel.Current.SelectedMineWork = MineWorkViewModel.PleaseSelect;
                        }
                    }
                });
            VirtualRoot.Access<MineWorkUpdatedEvent>(
                Guid.Parse("2ee32a0d-3c39-4910-92c6-ec7b26f43421"),
                "更新作业后刷新VM内存",
                LogEnum.Console, action: message => {
                    _dicById[message.Source.GetId()].Update(message.Source);
                });
            VirtualRoot.Access<MineWorkRemovedEvent>(
                Guid.Parse("19681790-dd17-449e-a386-eb1d317b4acd"),
                "删除作业后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    _dicById.Remove(message.Source.GetId());
                    OnPropertyChanged(nameof(List));
                    OnPropertyChanged(nameof(MineWorkVmItems));
                    OnPropertyChanged(nameof(MineWorkItems));
                    if (message.Source.GetId() == MinerClientsViewModel.Current.SelectedMineWork.GetId()) {
                        MinerClientsViewModel.Current.SelectedMineWork = MineWorkViewModel.PleaseSelect;
                    }
                });
        }

        public List<MineWorkViewModel> List {
            get {
                return _dicById.Values.ToList();
            }
        }

        private IEnumerable<MineWorkViewModel> GetMineWorkVmItems() {
            yield return MineWorkViewModel.PleaseSelect;
            yield return MineWorkViewModel.FreeMineWork;
            foreach (var item in List) {
                yield return item;
            }
        }
        public List<MineWorkViewModel> MineWorkVmItems {
            get {
                return GetMineWorkVmItems().ToList();
            }
        }

        private IEnumerable<MineWorkViewModel> GetMineWorkItems() {
            yield return MineWorkViewModel.FreeMineWork;
            foreach (var item in List) {
                yield return item;
            }
        }
        public List<MineWorkViewModel> MineWorkItems {
            get {
                return GetMineWorkItems().ToList();
            }
        }

        public bool TryGetMineWorkVm(Guid id, out MineWorkViewModel mineWorkVm) {
            return _dicById.TryGetValue(id, out mineWorkVm);
        }

        public IEnumerator<MineWorkViewModel> GetEnumerator() {
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _dicById.Values.GetEnumerator();
        }
    }
}

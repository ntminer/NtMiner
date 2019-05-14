using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public partial class AppContext {
        public class MineWorkViewModels : ViewModelBase {
            public static readonly MineWorkViewModels Instance = new MineWorkViewModels();
            private readonly Dictionary<Guid, MineWorkViewModel> _dicById = new Dictionary<Guid, MineWorkViewModel>();
            public ICommand Add { get; private set; }

            private MineWorkViewModels() {
#if DEBUG
                VirtualRoot.Stopwatch.Restart();
#endif
                if (Design.IsInDesignMode) {
                    return;
                }
                foreach (var item in NTMinerRoot.Instance.MineWorkSet) {
                    _dicById.Add(item.GetId(), new MineWorkViewModel(item));
                }
                this.Add = new DelegateCommand(() => {
                    new MineWorkViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
                });
                On<MineWorkAddedEvent>("添加作业后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            _dicById.Add(message.Source.GetId(), new MineWorkViewModel(message.Source));
                            OnPropertyChanged(nameof(List));
                            OnPropertyChanged(nameof(MineWorkVmItems));
                            if (message.Source.GetId() == AppContext.Instance.MinerClientsWindowVm.SelectedMineWork.GetId()) {
                                AppContext.Instance.MinerClientsWindowVm.SelectedMineWork = MineWorkViewModel.PleaseSelect;
                            }
                        }
                    });
                On<MineWorkUpdatedEvent>("更新作业后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById[message.Source.GetId()].Update(message.Source);
                    });
                On<MineWorkRemovedEvent>("删除作业后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChanged(nameof(List));
                        OnPropertyChanged(nameof(MineWorkVmItems));
                        if (message.Source.GetId() == AppContext.Instance.MinerClientsWindowVm.SelectedMineWork.GetId()) {
                            AppContext.Instance.MinerClientsWindowVm.SelectedMineWork = MineWorkViewModel.PleaseSelect;
                        }
                    });
#if DEBUG
                Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
            }

            public List<MineWorkViewModel> List {
                get {
                    return _dicById.Values.ToList();
                }
            }

            private IEnumerable<MineWorkViewModel> GetMineWorkVmItems() {
                yield return MineWorkViewModel.PleaseSelect;
                foreach (var item in List) {
                    yield return item;
                }
            }
            public List<MineWorkViewModel> MineWorkVmItems {
                get {
                    return GetMineWorkVmItems().ToList();
                }
            }

            public bool TryGetMineWorkVm(Guid id, out MineWorkViewModel mineWorkVm) {
                return _dicById.TryGetValue(id, out mineWorkVm);
            }
        }
    }
}

using NTMiner.Core.MinerStudio;
using NTMiner.MinerStudio.Vms;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.MinerStudio {
    public static partial class MinerStudioRoot {
        public class ColumnsShowViewModels : ViewModelBase {
            public static readonly ColumnsShowViewModels Instance = new ColumnsShowViewModels();

            private readonly Dictionary<Guid, ColumnsShowViewModel> _dicById = new Dictionary<Guid, ColumnsShowViewModel>();

            public ICommand Add { get; private set; }

            private ColumnsShowViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
#if DEBUG
                NTStopwatch.Start();
#endif
                this.Add = new DelegateCommand(() => {
                    WpfUtil.ShowInputDialog("列分组名称", string.Empty, string.Empty, columnsShowName => {
                        if (string.IsNullOrEmpty(columnsShowName)) {
                            return "列分组名称是必须的";
                        }
                        return string.Empty;
                    }, onOk: columnsShowName => {
                        ColumnsShowData entity = new ColumnsShowData {
                            ColumnsShowName = columnsShowName,
                            LastActivedOnText = true,
                            BootTimeSpanText = true,
                            MineTimeSpanText = true,
                            Work = true,
                            MinerGroup = true,
                            MinerName = true,
                            WorkerName = true,
                            LocalIp = true,
                            MinerIp = true,
                            GpuType = true,
                            MainCoinCode = true,
                            MainCoinSpeedText = true,
                            MainCoinRejectPercentText = true
                        };
                        NTMinerContext.Instance.MinerStudioContext.ColumnsShowSet.AddOrUpdate(entity);
                    });
                });
                AppRoot.AddEventPath<ColumnsShowAddedOrUpdatedEvent>("添加或修改了列分组后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (!_dicById.TryGetValue(message.Target.GetId(), out ColumnsShowViewModel vm)) {
                            vm = new ColumnsShowViewModel(message.Target);
                            _dicById.Add(message.Target.GetId(), vm);
                            OnPropertyChanged(nameof(List));
                            MinerClientsWindowVm.ColumnsShow = vm;
                        }
                        else {
                            vm.Update(message.Target);
                        }
                    }, location: this.GetType());
                AppRoot.AddEventPath<ColumnsRemovedEvent>("删除了列分组后刷新Vm内存", LogEnum.DevConsole, action: message => {
                    if (_dicById.ContainsKey(message.Target.Id)) {
                        _dicById.Remove(message.Target.Id);
                        OnPropertyChanged(nameof(List));
                        if (_dicById.TryGetValue(ColumnsShowData.PleaseSelect.Id, out ColumnsShowViewModel vm)) {
                            MinerClientsWindowVm.ColumnsShow = vm;
                        }
                    }
                }, this.GetType());

                foreach (var item in NTMinerContext.Instance.MinerStudioContext.ColumnsShowSet.GetAll()) {
                    _dicById.Add(item.Id, new ColumnsShowViewModel(item));
                }
#if DEBUG
                var elapsedMilliseconds = NTStopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            public List<ColumnsShowViewModel> List {
                get {
                    return _dicById.Values.ToList();
                }
            }
        }
    }
}

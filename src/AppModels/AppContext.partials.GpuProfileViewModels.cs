using NTMiner.Core;
using NTMiner.Profile;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public partial class AppContext {
        public class GpuProfileViewModels : ViewModelBase {
            public static readonly GpuProfileViewModels Instance = new GpuProfileViewModels();

            private readonly Dictionary<Guid, List<GpuProfileViewModel>> _listByCoinId = new Dictionary<Guid, List<GpuProfileViewModel>>();
            private readonly Dictionary<Guid, GpuProfileViewModel> _gpuAllVmDicByCoinId = new Dictionary<Guid, GpuProfileViewModel>();

            private GpuProfileViewModels() {
#if DEBUG
                Write.Stopwatch.Start();
#endif
                VirtualRoot.AddEventPath<GpuProfileSetRefreshedEvent>("Gpu超频集合刷新后刷新附着在当前币种上的超频数据", LogEnum.DevConsole,
                    action: message => {
                        lock (_locker) {
                            _listByCoinId.Clear();
                            _gpuAllVmDicByCoinId.Clear();
                        }
                        var coinVm = AppContext.Instance.MinerProfileVm.CoinVm;
                        if (coinVm != null) {
                            coinVm.OnOverClockPropertiesChanges();
                            VirtualRoot.Execute(new CoinOverClockCommand(coinVm.Id));
                        }
                    }, location: this.GetType());
                AddEventPath<GpuProfileAddedOrUpdatedEvent>("添加或更新了Gpu超频数据后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        lock (_locker) {
                            if (_listByCoinId.TryGetValue(message.Target.CoinId, out List<GpuProfileViewModel> list)) {
                                var vm = list.FirstOrDefault(a => a.Index == message.Target.Index);
                                if (vm != null) {
                                    vm.Update(message.Target);
                                }
                                else {
                                    if (AppContext.Instance.GpuVms.TryGetGpuVm(message.Target.Index, out GpuViewModel gpuVm)) {
                                        var item = new GpuProfileViewModel(message.Target, gpuVm);
                                        list.Add(item);
                                        list.Sort(new CompareByGpuIndex());
                                        if (item.Index == NTMinerRoot.GpuAllId) {
                                            _gpuAllVmDicByCoinId.Add(message.Target.CoinId, item);
                                        }
                                    }
                                }
                            }
                            else {
                                list = new List<GpuProfileViewModel>();
                                if (AppContext.Instance.GpuVms.TryGetGpuVm(message.Target.Index, out GpuViewModel gpuVm)) {
                                    var item = new GpuProfileViewModel(message.Target, gpuVm);
                                    list.Add(item);
                                    list.Sort(new CompareByGpuIndex());
                                    if (item.Index == NTMinerRoot.GpuAllId) {
                                        _gpuAllVmDicByCoinId.Add(message.Target.CoinId, item);
                                    }
                                }
                                _listByCoinId.Add(message.Target.CoinId, list);
                            }
                        }
                    }, location: this.GetType());
#if DEBUG
                var elapsedMilliseconds = Write.Stopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            private readonly object _locker = new object();
            public GpuProfileViewModel GpuAllVm(Guid coinId) {
                if (!_gpuAllVmDicByCoinId.TryGetValue(coinId, out GpuProfileViewModel result)) {
                    lock (_locker) {
                        if (!_gpuAllVmDicByCoinId.TryGetValue(coinId, out result)) {
                            AppContext.Instance.GpuVms.TryGetGpuVm(NTMinerRoot.GpuAllId, out GpuViewModel gpuVm);
                            result = GetGpuProfileVm(coinId, gpuVm);
                            _gpuAllVmDicByCoinId.Add(coinId, result);
                        }
                    }
                }
                return result;
            }

            public List<GpuProfileViewModel> List(Guid coinId) {
                if (!_listByCoinId.TryGetValue(coinId, out List<GpuProfileViewModel> list)) {
                    lock (_locker) {
                        if (!_listByCoinId.TryGetValue(coinId, out list)) {
                            list = new List<GpuProfileViewModel>();
                            foreach (var gpu in AppContext.Instance.GpuVms.Items) {
                                GpuProfileViewModel gpuProfileVm = GetGpuProfileVm(coinId, gpu);
                                list.Add(gpuProfileVm);
                            }
                            list.Sort(new CompareByGpuIndex());
                            _listByCoinId.Add(coinId, list);
                        }
                    }
                }
                return list;
            }

            private GpuProfileViewModel GetGpuProfileVm(Guid coinId, GpuViewModel gpuVm) {
                IGpuProfile data = NTMinerRoot.Instance.GpuProfileSet.GetGpuProfile(coinId, gpuVm.Index);
                return new GpuProfileViewModel(data, gpuVm);
            }

            private class CompareByGpuIndex : IComparer<GpuProfileViewModel> {
                public int Compare(GpuProfileViewModel x, GpuProfileViewModel y) {
                    return x.Index - y.Index;
                }
            }
        }
    }
}

using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static partial class AppRoot {
        public class PackageViewModels : ViewModelBase {
            public static readonly PackageViewModels Instance = new PackageViewModels();
            private readonly Dictionary<Guid, PackageViewModel> _dicById = new Dictionary<Guid, PackageViewModel>();

            private PackageViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
#if DEBUG
                NTStopwatch.Start();
#endif
                VirtualRoot.AddEventPath<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        Init();
                    }, location: this.GetType());
                VirtualRoot.AddEventPath<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        OnPropertyChanged(nameof(AllPackages));
                    }, location: this.GetType());
                AddEventPath<PackageAddedEvent>("添加了包后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Add(message.Source.GetId(), new PackageViewModel(message.Source));
                        OnPropertyChanged(nameof(AllPackages));
                        foreach (var item in KernelVms.AllKernels) {
                            item.OnPropertyChanged(nameof(item.IsPackageValid));
                        }
                    }, location: this.GetType());
                AddEventPath<PackageRemovedEvent>("删除了包后调整VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChanged(nameof(AllPackages));
                        foreach (var item in KernelVms.AllKernels) {
                            item.OnPropertyChanged(nameof(item.IsPackageValid));
                        }
                    }, location: this.GetType());
                AddEventPath<PackageUpdatedEvent>("更新了包后调整VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out PackageViewModel vm)) {
                            vm.Update(message.Source);
                            foreach (var item in KernelVms.AllKernels) {
                                item.OnPropertyChanged(nameof(item.IsPackageValid));
                            }
                        }
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
                foreach (var item in NTMinerContext.Instance.ServerContext.PackageSet.AsEnumerable()) {
                    _dicById.Add(item.GetId(), new PackageViewModel(item));
                }
            }

            public bool TryGetPackageVm(Guid packageId, out PackageViewModel PackageVm) {
                return _dicById.TryGetValue(packageId, out PackageVm);
            }

            public List<PackageViewModel> AllPackages {
                get {
                    return _dicById.Values.OrderBy(a => a.Name).ToList();
                }
            }
        }
    }
}

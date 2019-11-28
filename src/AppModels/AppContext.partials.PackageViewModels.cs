using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public partial class AppContext {
        public class PackageViewModels : ViewModelBase {
            public static readonly PackageViewModels Instance = new PackageViewModels();
            private readonly Dictionary<Guid, PackageViewModel> _dicById = new Dictionary<Guid, PackageViewModel>();

            private PackageViewModels() {
#if DEBUG
                Write.Stopwatch.Start();
#endif
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        Init();
                    });
                VirtualRoot.BuildEventPath<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        OnPropertyChanged(nameof(AllPackages));
                    });
                BuildEventPath<PackageAddedEvent>("添加了包后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Add(message.Target.GetId(), new PackageViewModel(message.Target));
                        OnPropertyChanged(nameof(AllPackages));
                        foreach (var item in AppContext.Instance.KernelVms.AllKernels) {
                            item.OnPropertyChanged(nameof(item.IsPackageValid));
                        }
                    });
                BuildEventPath<PackageRemovedEvent>("删除了包后调整VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Remove(message.Target.GetId());
                        OnPropertyChanged(nameof(AllPackages));
                        foreach (var item in AppContext.Instance.KernelVms.AllKernels) {
                            item.OnPropertyChanged(nameof(item.IsPackageValid));
                        }
                    });
                BuildEventPath<PackageUpdatedEvent>("更新了包后调整VM内存", LogEnum.DevConsole,
                    action: message => {
                        var entity = _dicById[message.Target.GetId()];
                        entity.Update(message.Target);
                        foreach (var item in AppContext.Instance.KernelVms.AllKernels) {
                            item.OnPropertyChanged(nameof(item.IsPackageValid));
                        }
                    });
                Init();
#if DEBUG
                var elapsedMilliseconds = Write.Stopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.ServerContext.PackageSet.AsEnumerable()) {
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

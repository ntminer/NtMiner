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
                VirtualRoot.Stopwatch.Restart();
#endif
                On<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        Init();
                    });
                On<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        OnPropertyChanged(nameof(AllPackages));
                    });
                On<PackageAddedEvent>("添加了包后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Add(message.Source.GetId(), new PackageViewModel(message.Source));
                        OnPropertyChanged(nameof(AllPackages));
                    });
                On<PackageRemovedEvent>("删除了包后调整VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChanged(nameof(AllPackages));
                    });
                On<PackageUpdatedEvent>("更新了包后调整VM内存", LogEnum.DevConsole,
                    action: message => {
                        var entity = _dicById[message.Source.GetId()];
                        entity.Update(message.Source);
                    });
                Init();
#if DEBUG
                Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.PackageSet) {
                    _dicById.Add(item.GetId(), new PackageViewModel(item));
                }
            }

            public bool TryGetPackageVm(Guid packageId, out PackageViewModel PackageVm) {
                return _dicById.TryGetValue(packageId, out PackageVm);
            }

            public List<PackageViewModel> AllPackages {
                get {
                    return _dicById.Values.ToList();
                }
            }
        }
    }
}

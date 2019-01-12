using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class KernelOutputViewModels : ViewModelBase {
        public static readonly KernelOutputViewModels Current = new KernelOutputViewModels();

        private readonly Dictionary<Guid, KernelOutputViewModel> _dicById = new Dictionary<Guid, KernelOutputViewModel>();

        private KernelOutputViewModels() {
            Global.Access<KernelOutputAddedEvent>(
                Guid.Parse("E1864E02-ED87-49D6-AD9F-A54953D6BC7E"),
                "添加了内核输出组后刷新VM内存",
                LogEnum.None,
                action: message => {
                    var vm = new KernelOutputViewModel(message.Source);
                    _dicById.Add(message.Source.GetId(), vm);
                    OnPropertyChanged(nameof(AllKernelOutputVms));
                });
            Global.Access<KernelOutputUpdatedEvent>(
                Guid.Parse("AA98F304-B7E1-4E93-8AFB-55F72EA37689"),
                "更新了内核输出组后刷新VM内存",
                LogEnum.None,
                action: message => {
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        var item = _dicById[message.Source.GetId()];
                        if (item != null) {
                            item.Update(message.Source);
                        }
                    }
                });
            Global.Access<KernelOutputRemovedEvent>(
                Guid.Parse("BE4A8820-AA3E-474B-AF04-9798D4B08DFC"),
                "移除了内核输出组后刷新VM内存",
                LogEnum.None,
                action: message => {
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChanged(nameof(AllKernelOutputVms));
                    }
                });
            foreach (var item in NTMinerRoot.Current.KernelOutputSet) {
                _dicById.Add(item.GetId(), new KernelOutputViewModel(item));
            }
        }

        public IEnumerable<KernelOutputViewModel> AllKernelOutputVms {
            get {
                return _dicById.Values;
            }
        }
    }
}

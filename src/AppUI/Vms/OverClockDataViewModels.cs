using NTMiner.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class OverClockDataViewModels : ViewModelBase, IEnumerable<OverClockDataViewModel> {
        public static readonly OverClockDataViewModels Current = new OverClockDataViewModels();

        private readonly Dictionary<Guid, OverClockDataViewModel> _dicById = new Dictionary<Guid, OverClockDataViewModel>();

        private OverClockDataViewModels() {
            if (Design.IsInDesignMode) {
                return;
            }
            foreach (var item in NTMinerRoot.Current.OverClockDataSet) {
                _dicById.Add(item.GetId(), new OverClockDataViewModel(item));
            }
            VirtualRoot.On<OverClockDataAddedEvent>(
                "添加超频建议后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    if (!_dicById.ContainsKey(message.Source.GetId())) {
                        _dicById.Add(message.Source.GetId(), new OverClockDataViewModel(message.Source));
                        OnPropertyChanged(nameof(List));
                    }
                });
            VirtualRoot.On<OverClockDataUpdatedEvent>(
                "更新超频建议后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    _dicById[message.Source.GetId()].Update(message.Source);
                });
            VirtualRoot.On<OverClockDataRemovedEvent>(
                "删除超频建议后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    _dicById.Remove(message.Source.GetId());
                    OnPropertyChanged(nameof(List));
                });
        }

        public List<OverClockDataViewModel> List {
            get {
                return _dicById.Values.ToList();
            }
        }

        public bool TryGetMineWorkVm(Guid id, out OverClockDataViewModel minerGroupVm) {
            return _dicById.TryGetValue(id, out minerGroupVm);
        }

        public IEnumerator<OverClockDataViewModel> GetEnumerator() {
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _dicById.Values.GetEnumerator();
        }
    }
}

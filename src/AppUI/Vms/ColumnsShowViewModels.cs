using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ColumnsShowViewModels : ViewModelBase {
        public static readonly ColumnsShowViewModels Current = new ColumnsShowViewModels();
        private readonly Dictionary<Guid, ColumnsShowViewModel> _dicById = new Dictionary<Guid, ColumnsShowViewModel>();

        public ICommand Add { get; private set; }

        private ColumnsShowViewModels() {
            this.Add = new DelegateCommand(() => {
                new ColumnsShowViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
            });
            VirtualRoot.On<ColumnsShowAddedEvent>("添加了列显后刷新VM内存", LogEnum.DevConsole,
                action: message => {
                    if (!_dicById.ContainsKey(message.Source.GetId())) {
                        ColumnsShowViewModel vm = new ColumnsShowViewModel(message.Source);
                        _dicById.Add(message.Source.GetId(), vm);
                        OnPropertyChanged(nameof(List));
                        MinerClientsWindowViewModel.Current.ColumnsShow = vm;
                    }
                });
            VirtualRoot.On<ColumnsShowUpdatedEvent>("更新了列显后刷新VM内存", LogEnum.DevConsole,
                action: message => {
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        ColumnsShowViewModel entity = _dicById[message.Source.GetId()];
                        entity.Update(message.Source);
                    }
                });
            VirtualRoot.On<ColumnsShowRemovedEvent>("移除了列显后刷新VM内存", LogEnum.DevConsole,
                action: message => {
                    MinerClientsWindowViewModel.Current.ColumnsShow = _dicById.Values.FirstOrDefault();
                    _dicById.Remove(message.Source.GetId());
                    OnPropertyChanged(nameof(List));
                });
            foreach (var item in NTMinerRoot.Current.ColumnsShowSet) {
                _dicById.Add(item.GetId(), new ColumnsShowViewModel(item));
            }
        }

        public List<ColumnsShowViewModel> List {
            get {
                return _dicById.Values.ToList();
            }
        }
    }
}

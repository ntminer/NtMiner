using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class SysDicViewModels : ViewModelBase {
        public static readonly SysDicViewModels Current = new SysDicViewModels();

        private readonly Dictionary<Guid, SysDicViewModel> _dicById = new Dictionary<Guid, SysDicViewModel>();
        private readonly Dictionary<string, SysDicViewModel> _dicByCode = new Dictionary<string, SysDicViewModel>(StringComparer.OrdinalIgnoreCase);

        public ICommand Add { get; private set; }
        private SysDicViewModels() {
            NTMinerRoot.Current.OnContextReInited += () => {
                _dicByCode.Clear();
                _dicById.Clear();
                Init();
            };
            NTMinerRoot.Current.OnReRendContext += () => {
                AllPropertyChanged();
            };
            this.Add = new DelegateCommand(() => {
                new SysDicViewModel(Guid.NewGuid()).Edit.Execute(null);
            });
            Init();
        }

        private void Init() {
            VirtualRoot.On<SysDicAddedEvent>("添加了系统字典后调整VM内存", LogEnum.DevConsole,
                action: (message) => {
                    if (!_dicById.ContainsKey(message.Source.GetId())) {
                        SysDicViewModel sysDicVm = new SysDicViewModel(message.Source);
                        _dicById.Add(message.Source.GetId(), sysDicVm);
                        if (!_dicByCode.ContainsKey(message.Source.Code)) {
                            _dicByCode.Add(message.Source.Code, sysDicVm);
                        }
                        OnPropertyChanged(nameof(List));
                        OnPropertyChanged(nameof(Count));
                    }
                }).AddToCollection(NTMinerRoot.Current.ContextHandlers);
            VirtualRoot.On<SysDicUpdatedEvent>("更新了系统字典后调整VM内存", LogEnum.DevConsole,
                action: (message) => {
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        SysDicViewModel entity = _dicById[message.Source.GetId()];
                        int sortNumber = entity.SortNumber;
                        entity.Update(message.Source);
                        if (sortNumber != entity.SortNumber) {
                            this.OnPropertyChanged(nameof(List));
                        }
                    }
                }).AddToCollection(NTMinerRoot.Current.ContextHandlers);
            VirtualRoot.On<SysDicRemovedEvent>("删除了系统字典后调整VM内存", LogEnum.DevConsole,
                action: (message) => {
                    _dicById.Remove(message.Source.GetId());
                    _dicByCode.Remove(message.Source.Code);
                    OnPropertyChanged(nameof(List));
                    OnPropertyChanged(nameof(Count));
                }).AddToCollection(NTMinerRoot.Current.ContextHandlers);
            foreach (var item in NTMinerRoot.Current.SysDicSet) {
                SysDicViewModel sysDicVm = new SysDicViewModel(item);
                _dicById.Add(item.GetId(), sysDicVm);
                _dicByCode.Add(item.Code, sysDicVm);
            }
        }

        public bool TryGetSysDicVm(Guid dicId, out SysDicViewModel sysDicVm) {
            return _dicById.TryGetValue(dicId, out sysDicVm);
        }

        public bool TryGetSysDicVm(string dicCode, out SysDicViewModel sysDicVm) {
            return _dicByCode.TryGetValue(dicCode, out sysDicVm);
        }

        public int Count {
            get {
                return _dicById.Count;
            }
        }

        public List<SysDicViewModel> List {
            get {
                return _dicById.Values.OrderBy(a => a.SortNumber).ToList();
            }
        }
    }
}

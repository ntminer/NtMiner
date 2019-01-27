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
            Global.Access<SysDicSetRefreshedEvent>(
                Guid.Parse("62D47241-D680-41B1-BB29-9500A632C919"),
                "系统字典数据集刷新后刷新Vm内存",
                LogEnum.Console,
                action: message => {
                    Init(isRefresh: true);
                });
            Global.Access<SysDicAddedEvent>(
                Guid.Parse("eef03852-17c4-4124-bb64-444f6c4f19ab"),
                "添加了系统字典后调整VM内存",
                LogEnum.Log,
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
                });
            Global.Access<SysDicUpdatedEvent>(
                Guid.Parse("f34d33e9-981b-4513-9425-e8694a4b5b17"),
                "更新了系统字典后调整VM内存",
                LogEnum.Log,
                action: (message) => {
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        SysDicViewModel entity = _dicById[message.Source.GetId()];
                        int sortNumber = entity.SortNumber;
                        entity.Update(message.Source);
                        if (sortNumber != entity.SortNumber) {
                            this.OnPropertyChanged(nameof(List));
                        }
                    }
                });
            Global.Access<SysDicRemovedEvent>(
                Guid.Parse("25c11087-a479-40de-8ddc-9c807a66afab"),
                "删除了系统字典后调整VM内存",
                LogEnum.Log,
                action: (message) => {
                    _dicById.Remove(message.Source.GetId());
                    _dicByCode.Remove(message.Source.Code);
                    OnPropertyChanged(nameof(List));
                    OnPropertyChanged(nameof(Count));
                });
        }

        private void Init(bool isRefresh = false) {
            if (isRefresh) {
                foreach (var item in NTMinerRoot.Current.SysDicSet) {
                    SysDicViewModel vm;
                    if (_dicById.TryGetValue(item.GetId(), out vm)) {
                        Global.Execute(new UpdateSysDicCommand(item));
                    }
                    else {
                        Global.Execute(new AddSysDicCommand(item));
                    }
                }
            }
            else {
                foreach (var item in NTMinerRoot.Current.SysDicSet) {
                    SysDicViewModel sysDicVm = new SysDicViewModel(item);
                    _dicById.Add(item.GetId(), sysDicVm);
                    _dicByCode.Add(item.Code, sysDicVm);
                }
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

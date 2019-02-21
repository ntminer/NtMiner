using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class HandlerIdViewModels : ViewModelBase {
        public static readonly HandlerIdViewModels Current = new HandlerIdViewModels();

        private readonly Dictionary<Guid, HandlerIdViewModel> _dicById = new Dictionary<Guid, HandlerIdViewModel>();

        private HandlerIdViewModels() {
            VirtualRoot.IsPublishHandlerIdAddedEvent = true;
            foreach (var item in HandlerId.GetHandlerIds()) {
                _dicById.Add(item.Id, new HandlerIdViewModel(item));
            }
            VirtualRoot.On<HandlerIdAddedEvent>(
                Guid.Parse("b4dfdf20-d4f3-4a71-b42b-d33c142b7f15"),
                "新的处理器标识出现后更新VM内存数据",
                LogEnum.Console,
                action: message => {
                    if (!_dicById.ContainsKey(message.Source.Id)) {
                        _dicById.Add(message.Source.Id, new HandlerIdViewModel(message.Source));
                        OnPropertyChanged(nameof(List));
                    }
                });
            VirtualRoot.On<HandlerIdUpdatedEvent>(
                Guid.Parse("7fed4d98-12ca-4291-be07-42e75fe87ad1"),
                "处理器标识信息更新后更新VM内存数据",
                LogEnum.Console,
                action: message => {
                    if (_dicById.ContainsKey(message.Source.Id)) {
                        var vm = _dicById[message.Source.Id];
                        vm.Update(message.Source);
                    }
                });
        }

        public List<HandlerIdViewModel> List {
            get {
                return _dicById.Values.OrderBy(a => a.Location.Name + a.MessageType.Name).ToList();
            }
        }

        public IEnumerable<EnumItem<LogEnum>> LogTypeItems {
            get {
                return LogEnum.Console.GetEnumItems();
            }
        }
    }
}

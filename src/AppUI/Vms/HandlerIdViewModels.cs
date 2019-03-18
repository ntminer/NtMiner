using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class HandlerIdViewModels : ViewModelBase {
        public static readonly HandlerIdViewModels Current = new HandlerIdViewModels();

        private readonly Dictionary<string, HandlerIdViewModel> _dicById = new Dictionary<string, HandlerIdViewModel>();

        private HandlerIdViewModels() {
            VirtualRoot.IsPublishHandlerIdAddedEvent = true;
            foreach (var item in HandlerId.GetHandlerIds()) {
                _dicById.Add(item.HandlerPath, new HandlerIdViewModel(item));
            }
            VirtualRoot.On<HandlerIdAddedEvent>(
                "新的处理器标识出现后更新VM内存数据",
                LogEnum.Console,
                action: message => {
                    if (!_dicById.ContainsKey(message.Source.HandlerPath)) {
                        _dicById.Add(message.Source.HandlerPath, new HandlerIdViewModel(message.Source));
                        OnPropertyChanged(nameof(List));
                    }
                });
            VirtualRoot.On<HandlerIdUpdatedEvent>(
                "处理器标识信息更新后更新VM内存数据",
                LogEnum.Console,
                action: message => {
                    if (_dicById.ContainsKey(message.Source.HandlerPath)) {
                        var vm = _dicById[message.Source.HandlerPath];
                        vm.Update(message.Source);
                    }
                });
        }

        public List<HandlerIdViewModel> List {
            get {
                return _dicById.Values.OrderBy(a => a.Location.Name + a.MessageType.Name).ToList();
            }
        }
    }
}

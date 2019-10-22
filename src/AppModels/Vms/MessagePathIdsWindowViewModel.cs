using NTMiner.Bus;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NTMiner.Vms {
    public class MessagePathIdsWindowViewModel : ViewModelBase {
        private readonly ObservableCollection<IMessagePathId> _handlerIds = new ObservableCollection<IMessagePathId>();

        public MessagePathIdsWindowViewModel() {
            VirtualRoot.SMessageDispatcher.Connected += (handlerId)=> {
                UIThread.Execute(() => {
                    _handlerIds.Add(handlerId);
                });
            };
            VirtualRoot.SMessageDispatcher.Disconnected += (handlerId) => {
                UIThread.Execute(() => {
                    _handlerIds.Remove(handlerId);
                });
            };
        }

        public IEnumerable<IMessagePathId> HandlerIds {
            get {
                return _handlerIds;
            }
        }
    }
}

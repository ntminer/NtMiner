using NTMiner.Bus;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NTMiner.Vms {
    public class HandlerIdsWindowViewModel : ViewModelBase {
        private readonly ObservableCollection<IHandlerId> _handlerIds = new ObservableCollection<IHandlerId>();

        public HandlerIdsWindowViewModel() {
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

        public IEnumerable<IHandlerId> HandlerIds {
            get {
                return _handlerIds;
            }
        }
    }
}

using NTMiner.Bus;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NTMiner.Vms {
    public class HandlerIdsWindowViewModel : ViewModelBase {
        private readonly ObservableCollection<IHandlerId> _handlerIds;

        public HandlerIdsWindowViewModel() {
            _handlerIds = new ObservableCollection<IHandlerId>(VirtualRoot.SMessageDispatcher.HandlerIds);
            VirtualRoot.SMessageDispatcher.HandlerIdAdded += (handlerId)=> {
                UIThread.Execute(() => {
                    _handlerIds.Add(handlerId);
                });
            };
            VirtualRoot.SMessageDispatcher.HandlerIdRemoved += (handlerId) => {
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

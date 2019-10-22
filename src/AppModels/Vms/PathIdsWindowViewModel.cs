using NTMiner.Bus;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NTMiner.Vms {
    public class PathIdsWindowViewModel : ViewModelBase {
        private readonly ObservableCollection<IPathId> _handlerIds = new ObservableCollection<IPathId>();

        public PathIdsWindowViewModel() {
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

        public IEnumerable<IPathId> HandlerIds {
            get {
                return _handlerIds;
            }
        }
    }
}

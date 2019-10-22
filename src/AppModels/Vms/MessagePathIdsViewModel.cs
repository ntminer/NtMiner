using NTMiner.Bus;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NTMiner.Vms {
    public class MessagePathIdsViewModel : ViewModelBase {
        private readonly ObservableCollection<IMessagePathId> _pathIds = new ObservableCollection<IMessagePathId>();

        public MessagePathIdsViewModel() {
            VirtualRoot.SMessageDispatcher.Connected += (pathId) => {
                UIThread.Execute(() => {
                    _pathIds.Add(pathId);
                });
            };
            VirtualRoot.SMessageDispatcher.Disconnected += (pathId) => {
                UIThread.Execute(() => {
                    _pathIds.Remove(pathId);
                });
            };
        }

        public IEnumerable<IMessagePathId> PathIds {
            get {
                return _pathIds;
            }
        }
    }
}

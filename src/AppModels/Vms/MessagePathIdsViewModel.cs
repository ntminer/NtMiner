using NTMiner.Router;
using System.Collections.ObjectModel;

namespace NTMiner.Vms {
    public class MessagePathIdsViewModel : ViewModelBase {
        private readonly ObservableCollection<IMessagePathId> _pathIds;

        public MessagePathIdsViewModel() {
            _pathIds = new ObservableCollection<IMessagePathId>(VirtualRoot.MessageDispatcher.GetAllPaths());
        }

        public ObservableCollection<IMessagePathId> PathIds {
            get {
                return _pathIds;
            }
        }
    }
}

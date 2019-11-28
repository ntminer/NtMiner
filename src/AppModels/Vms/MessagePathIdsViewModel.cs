using NTMiner.Hub;
using System.Collections.ObjectModel;

namespace NTMiner.Vms {
    public class MessagePathIdsViewModel : ViewModelBase {
        private readonly ObservableCollection<IMessagePathId> _pathIds;

        public MessagePathIdsViewModel() {
            _pathIds = new ObservableCollection<IMessagePathId>(VirtualRoot.MessageHub.GetAllPaths());
        }

        public ObservableCollection<IMessagePathId> PathIds {
            get {
                return _pathIds;
            }
        }
    }
}

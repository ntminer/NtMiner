using System.Collections.Generic;

namespace NTMiner {
    public class EmptyWsSessionsAdapter : IWsSessionsAdapter {
        private static readonly EmptyWsSessionsAdapter _instance = new EmptyWsSessionsAdapter();
        public static EmptyWsSessionsAdapter Instance {
            get {
                return _instance;
            }
        }

        private EmptyWsSessionsAdapter() { }

        public int Count {
            get { return 0; }
        }

        public IEnumerable<IWsSessionAdapter> Sessions {
            get {
                return new IWsSessionAdapter[0];
            }
        }

        public bool TryGetSession(string sessionId, out IWsSessionAdapter session) {
            session = null;
            return false;
        }
    }
}

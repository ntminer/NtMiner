using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Notifications {
    public class NotificationMessageManagers : IEnumerable<INotificationMessageManager> {
        private List<INotificationMessageManager> _managers = new List<INotificationMessageManager>();

        public NotificationMessageManagers() {

        }

        public void AddManager(INotificationMessageManager manager) {
            if (_managers.Contains(manager)) {
                return;
            }
            _managers.Add(manager);
        }

        public void RemoveManager(INotificationMessageManager manager) {
            _managers.Remove(manager);
        }

        public IEnumerator<INotificationMessageManager> GetEnumerator() {
            return _managers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _managers.GetEnumerator();
        }
    }
}

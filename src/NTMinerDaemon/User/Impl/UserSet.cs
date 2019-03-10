using System.Collections;
using System.Collections.Generic;

namespace NTMiner.User.Impl {
    public class UserSet : IUserSet {
        private readonly Dictionary<string, UserData> _dicByLoginName = new Dictionary<string, UserData>();

        public UserSet() {
        }

        private bool _isInited = false;
        private readonly object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    string json = SpecialPath.ReadDaemonUsersJsonFile();
                    if (!string.IsNullOrEmpty(json)) {
                        List<UserData> users = HostRoot.JsonSerializer.Deserialize<List<UserData>>(json);
                        foreach (var user in users) {
                            if (!_dicByLoginName.ContainsKey(user.LoginName)) {
                                _dicByLoginName.Add(user.LoginName, user);
                            }
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public void Refresh() {
            _dicByLoginName.Clear();
            _isInited = false;
        }

        public IUser GetUser(string loginName) {
            InitOnece();
            UserData userData;
            if (_dicByLoginName.TryGetValue(loginName, out userData)) {
                return userData;
            }
            return null;
        }

        public IEnumerator<IUser> GetEnumerator() {
            InitOnece();
            return _dicByLoginName.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicByLoginName.Values.GetEnumerator();
        }
    }
}

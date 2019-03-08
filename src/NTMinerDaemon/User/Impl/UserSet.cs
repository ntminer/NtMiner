using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NTMiner.User.Impl {
    public class UserSet : IUserSet {
        private Dictionary<string, UserData> _dicByLoginName = new Dictionary<string, UserData>();

        public UserSet() {
        }

        private bool _isInited = false;
        private object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    string userFileFullName = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "users.json");
                    if (File.Exists(userFileFullName)) {
                        string json = File.ReadAllText(userFileFullName);
                        if (!string.IsNullOrEmpty(json)) {
                            List<UserData> users = HostRoot.JsonSerializer.Deserialize<List<UserData>>(json);
                            foreach (var user in users) {
                                if (!_dicByLoginName.ContainsKey(user.LoginName)) {
                                    _dicByLoginName.Add(user.LoginName, user);
                                }
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

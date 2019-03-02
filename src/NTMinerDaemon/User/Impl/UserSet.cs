using System.Collections;
using System.Collections.Generic;

namespace NTMiner.User.Impl {
    public class UserSet : IUserSet {
        private readonly Dictionary<string, UserData> _dicByLoginName = new Dictionary<string, UserData>();

        private readonly IHostRoot _root;

        public UserSet(IHostRoot root) {
            _root = root;
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
                    
                    _isInited = true;
                }
            }
        }

        public bool Contains(string loginName) {
            InitOnece();
            return _dicByLoginName.ContainsKey(loginName);
        }

        public bool TryGetKey(string loginName, out IUser user) {
            InitOnece();
            UserData userData;
            bool result = _dicByLoginName.TryGetValue(loginName, out userData);
            user = userData;
            return result;
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

using LiteDB;
using NTMiner.User;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class UserSet : IUserSet {
        private Dictionary<string, UserData> _dicByLoginName = new Dictionary<string, UserData>();

        private readonly bool _isUseJson;
        public UserSet(bool isUseJson) {
            _isUseJson = isUseJson;
            VirtualRoot.Accept<AddUserCommand>(
                "处理添加用户命令",
                LogEnum.Console,
                action: message => {
                    if (!_dicByLoginName.ContainsKey(message.User.LoginName)) {
                        UserData entity = new UserData(message.User);
                        _dicByLoginName.Add(message.User.LoginName, entity);
                        using (LiteDatabase db = new LiteDatabase(SpecialPath.LocalDbFileFullName)) {
                            var col = db.GetCollection<UserData>();
                            col.Insert(entity);
                        }
                        VirtualRoot.Happened(new UserAddedEvent(entity));
                    }
                });
            VirtualRoot.Accept<UpdateUserCommand>(
                "处理修改用户命令",
                LogEnum.Console,
                action: message => {
                    if (_dicByLoginName.ContainsKey(message.User.LoginName)) {
                        UserData entity = _dicByLoginName[message.User.LoginName];
                        entity.Update(message.User);
                        using (LiteDatabase db = new LiteDatabase(SpecialPath.LocalDbFileFullName)) {
                            var col = db.GetCollection<UserData>();
                            col.Update(entity);
                        }
                        VirtualRoot.Happened(new UserUpdatedEvent(entity));
                    }
                });
            VirtualRoot.Accept<RemoveUserCommand>(
                "处理删除用户命令",
                LogEnum.Console,
                action: message => {
                    if (_dicByLoginName.ContainsKey(message.LoginName)) {
                        UserData entity = _dicByLoginName[message.LoginName];
                        _dicByLoginName.Remove(entity.LoginName);
                        using (LiteDatabase db = new LiteDatabase(SpecialPath.LocalDbFileFullName)) {
                            var col = db.GetCollection<UserData>();
                            col.Delete(entity.Id);
                        }
                        VirtualRoot.Happened(new UserRemovedEvent(entity));
                    }
                });
        }

        private bool _isInited = false;
        private object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        public void Refresh() {
            _dicByLoginName.Clear();
            _isInited = false;
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    UserData[] users;
                    if (_isUseJson) {
                        users = LocalJson.Instance.Users;
                    }
                    else {
                        using (LiteDatabase db = new LiteDatabase(SpecialPath.LocalDbFileFullName)) {
                            var col = db.GetCollection<UserData>();
                            users = col.FindAll().ToArray();
                        }
                    }
                    _dicByLoginName = users.ToDictionary(a => a.LoginName, a => a);
                    _isInited = true;
                }
            }
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

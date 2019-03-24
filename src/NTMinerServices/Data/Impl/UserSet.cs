using LiteDB;
using NTMiner.User;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Data.Impl {
    public class UserSet : IUserSet {
        private Dictionary<string, UserData> _dicByLoginName = new Dictionary<string, UserData>();

        private readonly string _dbFileFullName;
        public UserSet(string dbFileFullName) {
            _dbFileFullName = dbFileFullName;
            VirtualRoot.Window<AddUserCommand>("处理添加用户命令", LogEnum.DevConsole,
                action: message => {
                    if (!_dicByLoginName.ContainsKey(message.User.LoginName)) {
                        UserData entity = new UserData(message.User);
                        _dicByLoginName.Add(message.User.LoginName, entity);
                        using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                            var col = db.GetCollection<UserData>();
                            col.Insert(entity);
                        }
                        VirtualRoot.Happened(new UserAddedEvent(entity));
                    }
                });
            VirtualRoot.Window<UpdateUserCommand>("处理修改用户命令", LogEnum.DevConsole,
                action: message => {
                    if (_dicByLoginName.ContainsKey(message.User.LoginName)) {
                        UserData entity = _dicByLoginName[message.User.LoginName];
                        entity.Update(message.User);
                        using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                            var col = db.GetCollection<UserData>();
                            col.Update(entity);
                        }
                        VirtualRoot.Happened(new UserUpdatedEvent(entity));
                    }
                });
            VirtualRoot.Window<RemoveUserCommand>("处理删除用户命令", LogEnum.DevConsole,
                action: message => {
                    if (_dicByLoginName.ContainsKey(message.LoginName)) {
                        UserData entity = _dicByLoginName[message.LoginName];
                        _dicByLoginName.Remove(entity.LoginName);
                        using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                            var col = db.GetCollection<UserData>();
                            col.Delete(message.LoginName);
                        }
                        VirtualRoot.Happened(new UserRemovedEvent(entity));
                    }
                });
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
                    using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                        var col = db.GetCollection<UserData>();
                        _dicByLoginName = col.FindAll().ToDictionary(a => a.LoginName, a => a);
                    }
                    _isInited = true;
                }
            }
        }

        public IUser GetUser(string loginName) {
            InitOnece();
            if (_dicByLoginName.TryGetValue(loginName, out UserData userData)) {
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

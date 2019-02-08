using LiteDB;
using System;
using System.Collections.Generic;

namespace NTMiner.User.Impl {
    public class UserSet : IUserSet {
        private Dictionary<string, UserData> _dicByLoginName = new Dictionary<string, UserData>();

        private readonly string _dbFileFullName;
        public UserSet(string dbFileFullName) {
            _dbFileFullName = dbFileFullName;
            VirtualRoot.Access<AddUserCommand>(
                Guid.Parse("0E117810-6472-4688-B782-0AA9520B2DE6"),
                "处理添加用户命令",
                LogEnum.Console,
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
            VirtualRoot.Access<UpdateUserCommand>(
                Guid.Parse("5800AE7E-D6FB-490A-AD86-CD585875D87E"),
                "处理修改用户命令",
                LogEnum.Console,
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
            VirtualRoot.Access<RemoveUserCommand>(
                Guid.Parse("73FB4937-7CBE-4C72-8B3F-4003D2A2A321"),
                "处理删除用户命令",
                LogEnum.Console,
                action: message => {
                    if (_dicByLoginName.ContainsKey(message.LoginName)) {
                        UserData entity = _dicByLoginName[message.LoginName];
                        _dicByLoginName.Remove(entity.LoginName);
                        using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                            var col = db.GetCollection<UserData>();
                            col.Delete(entity.LoginName);
                        }
                        VirtualRoot.Happened(new UserRemovedEvent(entity));
                    }
                });
        }

        private object _locker = new object();
        private DateTime _lastInitOn = DateTime.MinValue;

        private void InitOnece() {
            DateTime now = DateTime.Now;
            if (_lastInitOn.AddMinutes(10) < now) {
                lock (_locker) {
                    if (_lastInitOn.AddMinutes(10) < now) {
                        using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                            var col = db.GetCollection<UserData>();
                            foreach (var item in col.FindAll()) {
                                _dicByLoginName.Add(item.LoginName, item);
                            }
                        }
                        _lastInitOn = DateTime.Now;
                    }
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
    }
}

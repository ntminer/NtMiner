using LiteDB;
using NTMiner.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.MinerServer.Impl {
    public class UserSet : IUserSet {
        private Dictionary<string, UserData> _dicByLoginName = new Dictionary<string, UserData>();

        private readonly string _dbFileFullName;
        public UserSet(string dbFileFullName) {
            _dbFileFullName = dbFileFullName;
            VirtualRoot.Accept<AddUserCommand>(
                "处理添加用户命令",
                LogEnum.Console,
                action: message => {
                    if (!_dicByLoginName.ContainsKey(message.User.LoginName)) {
                        Server.ControlCenterService.AddUserAsync(new UserData {
                            Id = ObjectId.NewObjectId(),
                            LoginName = message.User.LoginName,
                            Password = message.User.Password,
                            IsEnabled = message.User.IsEnabled,
                            Description = message.User.Description
                        }, response => {
                            if (response.IsSuccess()) {
                                UserData entity = new UserData(message.User);
                                _dicByLoginName.Add(message.User.LoginName, entity);
                                VirtualRoot.Happened(new UserAddedEvent(entity));
                            }
                            else if (response != null) {
                                Write.UserLine(response.Description, ConsoleColor.Red);
                            }
                        });
                    }
                });
            VirtualRoot.Accept<UpdateUserCommand>(
                "处理修改用户命令",
                LogEnum.Console,
                action: message => {
                    if (_dicByLoginName.ContainsKey(message.User.LoginName)) {
                        UserData entity = _dicByLoginName[message.User.LoginName];
                        Server.ControlCenterService.UpdateUserAsync(new UserData {
                            Id = ObjectId.NewObjectId(),
                            LoginName = message.User.LoginName,
                            Password = message.User.Password,
                            IsEnabled = message.User.IsEnabled,
                            Description = message.User.Description
                        }, response => {
                            if (response.IsSuccess()) {
                                entity.Update(message.User);
                                VirtualRoot.Happened(new UserUpdatedEvent(entity));
                            }
                            else if (response != null) {
                                Write.UserLine(response.Description, ConsoleColor.Red);
                            }
                        });
                    }
                });
            VirtualRoot.Accept<RemoveUserCommand>(
                "处理删除用户命令",
                LogEnum.Console,
                action: message => {
                    if (_dicByLoginName.ContainsKey(message.LoginName)) {
                        UserData entity = _dicByLoginName[message.LoginName];
                        Server.ControlCenterService.RemoveUserAsync(message.LoginName, response => {
                            if (response.IsSuccess()) {
                                _dicByLoginName.Remove(entity.LoginName);
                                VirtualRoot.Happened(new UserRemovedEvent(entity));
                            }
                            else if (response != null) {
                                Write.UserLine(response.Description, ConsoleColor.Red);
                            }
                        });
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
                        var response = Server.ControlCenterService.GetUsers(Guid.NewGuid());
                        if (response != null) {
                            _dicByLoginName = response.Data.ToDictionary(a => a.LoginName, a => a);
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

using NTMiner.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.MinerServer.Impl {
    public class UserSet : IUserSet {
        private Dictionary<string, UserData> _dicByLoginName = new Dictionary<string, UserData>();

        public UserSet() {
            VirtualRoot.AddCmdPath<AddUserCommand>(action: message => {
                if (!_dicByLoginName.ContainsKey(message.User.LoginName)) {
                    RpcRoot.Server.UserService.AddUserAsync(new UserData {
                        LoginName = message.User.LoginName,
                        Password = message.User.Password,
                        IsEnabled = message.User.IsEnabled,
                        Description = message.User.Description
                    }, (response, exception) => {
                        if (response.IsSuccess()) {
                            UserData entity = new UserData(message.User);
                            _dicByLoginName.Add(message.User.LoginName, entity);
                            VirtualRoot.RaiseEvent(new UserAddedEvent(message.Id, entity));
                        }
                        else {
                            Write.UserFail(response.ReadMessage(exception));
                        }
                    });
                }
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<UpdateUserCommand>(action: message => {
                if (_dicByLoginName.ContainsKey(message.User.LoginName)) {
                    UserData entity = _dicByLoginName[message.User.LoginName];
                    UserData oldValue = new UserData(entity);
                    entity.Update(message.User);
                    RpcRoot.Server.UserService.UpdateUserAsync(new UserData {
                        LoginName = message.User.LoginName,
                        Password = message.User.Password,
                        IsEnabled = message.User.IsEnabled,
                        Description = message.User.Description
                    }, (response, exception) => {
                        if (!response.IsSuccess()) {
                            entity.Update(oldValue);
                            VirtualRoot.RaiseEvent(new UserUpdatedEvent(message.Id, entity));
                            Write.UserFail(response.ReadMessage(exception));
                        }
                    });
                    VirtualRoot.RaiseEvent(new UserUpdatedEvent(message.Id, entity));
                }
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<RemoveUserCommand>(action: message => {
                if (_dicByLoginName.ContainsKey(message.LoginName)) {
                    UserData entity = _dicByLoginName[message.LoginName];
                    RpcRoot.Server.UserService.RemoveUserAsync(message.LoginName, (response, exception) => {
                        if (response.IsSuccess()) {
                            _dicByLoginName.Remove(entity.LoginName);
                            VirtualRoot.RaiseEvent(new UserRemovedEvent(message.Id, entity));
                        }
                        else {
                            Write.UserFail(response.ReadMessage(exception));
                        }
                    });
                }
            }, location: this.GetType());
        }

        private readonly object _locker = new object();
        private bool _isInited = false;

        private void InitOnece() {
            DateTime now = DateTime.Now;
            if (!_isInited) {
                lock (_locker) {
                    if (!_isInited) {
                        Guid? clientId = null;
                        if (!VirtualRoot.IsMinerStudio) {
                            clientId = VirtualRoot.Id;
                        }
                        var result = RpcRoot.Server.UserService.GetUsers(clientId);
                        _dicByLoginName = result.ToDictionary(a => a.LoginName, a => a);
                        _isInited = true;
                    }
                }
            }
        }

        public void Refresh() {
            _dicByLoginName.Clear();
            _isInited = false;
        }

        public IUser GetUser(string loginName) {
            InitOnece();
            if (_dicByLoginName.TryGetValue(loginName, out UserData userData)) {
                return userData;
            }
            return null;
        }

        public IEnumerable<IUser> AsEnumerable() {
            InitOnece();
            return _dicByLoginName.Values;
        }
    }
}

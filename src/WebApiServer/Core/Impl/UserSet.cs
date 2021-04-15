using NTMiner.Core.Mq.Senders;
using NTMiner.Core.Redis;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Core.Impl {
    public class UserSet : ReadOnlyUserSet, IUserSet {
        private readonly IUserDataRedis _redis;
        private readonly IUserMqSender _mqSender;
        private readonly Dictionary<string, TryLoginTimes> _tryLoginTimesDic = new Dictionary<string, TryLoginTimes>();
        public UserSet(IUserDataRedis redis, IUserMqSender mqSender) : base(redis) {
            _redis = redis;
            _mqSender = mqSender;
            VirtualRoot.BuildCmdPath<UpdateUserRSAKeyMqCommand>(this.GetType(), LogEnum.DevConsole, path: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                if (string.IsNullOrEmpty(message.LoginName)) {
                    return;
                }
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(nameof(UpdateUserRSAKeyMqCommand) + ":" + MqKeyword.SafeIgnoreMessage);
                    return;
                }
                if (message.Key != null && _dicByLoginName.TryGetValue(message.LoginName, out UserData userData)) {
                    userData.Update(message.Key);
                    redis.SetAsync(userData).ContinueWith(t => {
                        _mqSender.SendUserRSAKeyUpdated(message.LoginName);
                    });
                }
            });
        }

        public List<UserData> QueryUsers(QueryUsersRequest query, out int total) {
            if (!IsReadied) {
                total = 0;
                return new List<UserData>();
            }
            if (query == null) {
                total = 0;
                return new List<UserData>();
            }
            IQueryable<UserData> data = _dicByLoginName.Values.AsQueryable();
            if (!string.IsNullOrEmpty(query.LoginName)) {
                data = data.Where(a => a.LoginName.Contains(query.LoginName));
            }
            switch (query.UserStatus) {
                case UserStatus.All:
                    break;
                case UserStatus.IsEnabled:
                    data = data.Where(a => a.IsEnabled == true);
                    break;
                case UserStatus.IsDisabled:
                    data = data.Where(a => a.IsEnabled == false);
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(query.Email)) {
                data = data.Where(a => !string.IsNullOrEmpty(a.Email) && a.Email.IgnoreCaseContains(query.Email));
            }
            if (!string.IsNullOrEmpty(query.Mobile)) {
                data = data.Where(a => !string.IsNullOrEmpty(a.Mobile) && a.Mobile.Contains(query.Mobile));
            }
            if (!string.IsNullOrEmpty(query.Role)) {
                data = data.Where(a => !string.IsNullOrEmpty(a.Roles) && a.Roles.IgnoreCaseContains(query.Role));
            }
            total = data.Count();
            var results = data.OrderByDescending(a => a.CreatedOn).Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToList();
            return results;
        }

        private void AddRole(UserData user, Role.RoleEnum role) {
            if (user == null) {
                return;
            }
            if (string.IsNullOrEmpty(user.Roles)) {
                user.Roles = role.GetName();
            }
            else {
                user.Roles += "," + role.GetName();
            }
        }

        private void RemoveRole(UserData user, Role.RoleEnum role) {
            if (user == null) {
                return;
            }
            if (string.IsNullOrEmpty(user.Roles)) {
                return;
            }
            else if (user.Roles.IgnoreCaseContains("," + role.GetName())) {
                user.Roles = user.Roles.IgnoreCaseReplace("," + role.GetName(), string.Empty);
            }
            else if (user.Roles.IgnoreCaseContains(role.GetName() + ",")) {
                user.Roles = user.Roles.IgnoreCaseReplace(role.GetName() + ",", string.Empty);
            }
            else if (user.Roles.IgnoreCaseContains(role.GetName())) {
                user.Roles = user.Roles.IgnoreCaseReplace(role.GetName(), string.Empty);
            }
        }

        public void Add(UserData input) {
            if (!IsReadied) {
                return;
            }
            if (input == null || string.IsNullOrEmpty(input.LoginName)) {
                return;
            }
            if (!_dicByLoginName.ContainsKey(input.LoginName)) {
                _dicByLoginName.Add(input.LoginName, input);
            }
            _redis.SetAsync(input).ContinueWith(t => {
                _mqSender.SendUserAdded(input.LoginName);
            });
        }

        public void Update(UserUpdateData input) {
            if (_dicByLoginName.TryGetValue(input.LoginName, out UserData entity)) {
                entity.Update(input);
                _redis.SetAsync(entity).ContinueWith(t => {
                    _mqSender.SendUserUpdated(input.LoginName);
                });
            }
        }

        public void Remove(string loginName) {
            if (!IsReadied) {
                return;
            }
            if (string.IsNullOrEmpty(loginName)) {
                return;
            }
            Task<UserData> task;
            if (_dicByLoginName.TryGetValue(loginName, out UserData entity)) {
                _dicByLoginName.Remove(entity.LoginName);
                task = _redis.DeleteAsync(entity).ContinueWith(t => {
                    return entity;
                });
            }
            else {
                task = _redis.GetByLoginNameAsync(loginName);
            }
            task.ContinueWith(t => {
                if (t.Result != null) {
                    _mqSender.SendUserRemoved(loginName);
                }
            });
        }

        public void Enable(string loginName) {
            if (!IsReadied) {
                return;
            }
            if (string.IsNullOrEmpty(loginName)) {
                return;
            }
            if (_dicByLoginName.TryGetValue(loginName, out UserData entity)) {
                entity.IsEnabled = true;
                _redis.SetAsync(entity).ContinueWith(t => {
                    _mqSender.SendUserEnabled(loginName);
                });
            }
        }

        public void Disable(string loginName) {
            if (!IsReadied) {
                return;
            }
            if (string.IsNullOrEmpty(loginName)) {
                return;
            }
            if (_dicByLoginName.TryGetValue(loginName, out UserData entity)) {
                entity.IsEnabled = false;
                _redis.SetAsync(entity).ContinueWith(t => {
                    _mqSender.SendUserDisabled(loginName);
                });
            }
        }

        public void AddAdminRole(string loginName) {
            if (!IsReadied) {
                return;
            }
            if (string.IsNullOrEmpty(loginName)) {
                return;
            }
            if (_dicByLoginName.TryGetValue(loginName, out UserData entity)) {
                AddRole(entity, Role.RoleEnum.Admin);
                _redis.SetAsync(entity).ContinueWith(t => {
                    _mqSender.SendUserUpdated(loginName);
                });
            }
        }

        public void RemoveAdminRole(string loginName) {
            if (!IsReadied) {
                return;
            }
            if (string.IsNullOrEmpty(loginName)) {
                return;
            }
            if (_dicByLoginName.TryGetValue(loginName, out UserData entity)) {
                RemoveRole(entity, Role.RoleEnum.Admin);
                _redis.SetAsync(entity).ContinueWith(t => {
                    _mqSender.SendUserUpdated(loginName);
                });
            }
        }

        public void UpdateLastLogin(UserData user, DateTime lastLogin) {
            if (user == null) {
                return;
            }
            if (_dicByLoginName.TryGetValue(user.LoginName, out UserData entity)) {
                if (entity != user) {
                    entity.LastLogin = lastLogin;
                }
            }
            else {
                return;
            }
            user.LastLogin = lastLogin;
            _redis.SetAsync(entity);
        }

        public void ChangePassword(string loginName, string newPassword) {
            if (!IsReadied) {
                return;
            }
            if (string.IsNullOrEmpty(loginName) || string.IsNullOrEmpty(newPassword)) {
                return;
            }
            if (_dicByLoginName.TryGetValue(loginName, out UserData entity)) {
                entity.Password = newPassword;
                _redis.SetAsync(entity).ContinueWith(t => {
                    _mqSender.SendUserPasswordChanged(loginName);
                });
            }
        }

        public bool Contains(string loginName) {
            if (!IsReadied) {
                return false;
            }
            if (string.IsNullOrEmpty(loginName)) {
                return false;
            }
            return _dicByLoginName.ContainsKey(loginName);
        }

        public bool CheckLoginTimes(string loginName) {
            if (!IsReadied) {
                return false;
            }
            if (string.IsNullOrEmpty(loginName)) {
                return false;
            }
            if (_tryLoginTimesDic.TryGetValue(loginName, out TryLoginTimes tryLoginTimes)) {
                DateTime lastTryOn = tryLoginTimes.LastTryOn;
                tryLoginTimes.LastTryOn = DateTime.Now;
                tryLoginTimes.Times += 1;
                // 如果距离上次登录的间隔小于1秒
                if ((tryLoginTimes.LastTryOn - lastTryOn).TotalSeconds <= 1) {
                    return false;
                }
                return true;
            }
            else {
                _tryLoginTimesDic.Add(loginName, new TryLoginTimes {
                    LastTryOn = DateTime.Now,
                    LoginName = loginName,
                    Times = 0
                });
                return true;
            }
        }
    }
}

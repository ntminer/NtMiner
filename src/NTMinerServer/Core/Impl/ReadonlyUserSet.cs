using NTMiner.Core.Redis;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class ReadOnlyUserSet : IReadOnlyUserSet {
        protected readonly Dictionary<string, UserData> _dicByLoginName = new Dictionary<string, UserData>(StringComparer.OrdinalIgnoreCase);
        private DateTime _initedOn = DateTime.MinValue;
        public bool IsReadied {
            get; private set;
        }
        public ReadOnlyUserSet(IReadOnlyUserRedis userRedis) {
            userRedis.GetAllAsync().ContinueWith(t => {
                _initedOn = DateTime.Now;
                foreach (var item in t.Result) {
                    _dicByLoginName.Add(item.LoginName, item);
                }
                IsReadied = true;
                NTMinerConsole.UserOk("用户集就绪");
                VirtualRoot.RaiseEvent(new UserSetInitedEvent());
            });
            // 收到Mq消息之前一定已经初始化完成，因为Mq消费者在UserSetInitedEvent事件之后才会创建
            VirtualRoot.BuildEventPath<UserAddedMqMessage>("收到UserAddedMq消息后从Redis加载新用户到内存", LogEnum.None, path: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                if (string.IsNullOrEmpty(message.LoginName)) {
                    return;
                }
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(nameof(UserAddedMqMessage) + ":" + MqKeyword.SafeIgnoreMessage);
                    return;
                }
                userRedis.GetByLoginNameAsync(message.LoginName).ContinueWith(t => {
                    if (t.Result != null) {
                        _dicByLoginName[message.LoginName] = t.Result;
                    }
                });
            }, this.GetType());
            VirtualRoot.BuildEventPath<UserUpdatedMqMessage>("收到UserUpdatedMq消息后从Redis加载用户到内存", LogEnum.None, path: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                if (string.IsNullOrEmpty(message.LoginName)) {
                    return;
                }
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(nameof(UserUpdatedMqMessage) + ":" + MqKeyword.SafeIgnoreMessage);
                    return;
                }
                userRedis.GetByLoginNameAsync(message.LoginName).ContinueWith(t => {
                    if (t.Result != null) {
                        _dicByLoginName[message.LoginName] = t.Result;
                    }
                });
            }, this.GetType());
            VirtualRoot.BuildEventPath<UserRemovedMqMessage>("收到UserRemovedMq消息后移除内存中对应的记录", LogEnum.None, path: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                if (string.IsNullOrEmpty(message.LoginName)) {
                    return;
                }
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(nameof(UserRemovedMqMessage) + ":" + MqKeyword.SafeIgnoreMessage);
                    return;
                }
                _dicByLoginName.Remove(message.LoginName);
            }, this.GetType());
            VirtualRoot.BuildEventPath<UserEnabledMqMessage>("收到UserEnabledMq消息后启用内存中对应记录的状态", LogEnum.None, path: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                if (string.IsNullOrEmpty(message.LoginName)) {
                    return;
                }
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(nameof(UserEnabledMqMessage) + ":" + MqKeyword.SafeIgnoreMessage);
                    return;
                }
                if (_dicByLoginName.TryGetValue(message.LoginName, out UserData userData)) {
                    userData.IsEnabled = true;
                }
            }, this.GetType());
            VirtualRoot.BuildEventPath<UserDisabledMqMessage>("收到UserDisabledMq消息后禁用内存中对应记录的状态", LogEnum.None, path: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                if (string.IsNullOrEmpty(message.LoginName)) {
                    return;
                }
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(nameof(UserDisabledMqMessage) + ":" + MqKeyword.SafeIgnoreMessage);
                    return;
                }
                if (_dicByLoginName.TryGetValue(message.LoginName, out UserData userData)) {
                    userData.IsEnabled = false;
                }
            }, this.GetType());
            VirtualRoot.BuildEventPath<UserPasswordChangedMqMessage>("收到UserPasswordChangedMq消息后从Redis刷新内存中对应的记录", LogEnum.None, path: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                if (string.IsNullOrEmpty(message.LoginName)) {
                    return;
                }
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(nameof(UserPasswordChangedMqMessage) + ":" + MqKeyword.SafeIgnoreMessage);
                    return;
                }
                userRedis.GetByLoginNameAsync(message.LoginName).ContinueWith(t => {
                    if (t.Result != null) {
                        _dicByLoginName[message.LoginName] = t.Result;
                    }
                });
            }, this.GetType());
            VirtualRoot.BuildEventPath<UserRSAKeyUpdatedMqMessage>("收到了UserRSAKeyUpdated Mq消息后更新内存中对应的记录", LogEnum.None, path: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                if (string.IsNullOrEmpty(message.LoginName)) {
                    return;
                }
                if (IsOldMqMessage(message.Timestamp)) {
                    NTMinerConsole.UserOk(nameof(UserRSAKeyUpdatedMqMessage) + ":" + MqKeyword.SafeIgnoreMessage);
                    return;
                }
                if (_dicByLoginName.TryGetValue(message.LoginName, out UserData userData)) {
                    userRedis.GetByLoginNameAsync(message.LoginName).ContinueWith(t => {
                        if (t.Result != null) {
                            _dicByLoginName[message.LoginName] = t.Result;
                        }
                    });
                }
            }, this.GetType());
        }

        protected bool IsOldMqMessage(DateTime mqMessageTimestamp) {
            // 考虑到服务器间时钟可能不完全同步，如果消息发生的时间比_initedOn的时间早了
            // 一分多钟则可以视为Init时已经包含了该Mq消息所表达的事情就不需要再访问Redis了
            if (mqMessageTimestamp.AddMinutes(1) < _initedOn) {
                return true;
            }
            return false;
        }

        public UserData GetUser(UserId userId) {
            if (!IsReadied) {
                return null;
            }
            if (userId == null || string.IsNullOrEmpty(userId.Value)) {
                return null;
            }
            switch (userId.UserIdType) {
                case UserIdType.LoginName:
                    if (_dicByLoginName.TryGetValue(userId.Value, out UserData userData)) {
                        return userData;
                    }
                    break;
                case UserIdType.Email:
                    // TODO:建立基于Email的索引
                    return _dicByLoginName.Values.FirstOrDefault(a => a.Email == userId.Value);
                case UserIdType.Mobile:
                    // TODO:建立基于Mobile的索引
                    return _dicByLoginName.Values.FirstOrDefault(a => a.Mobile == userId.Value);
                default:
                    break;
            }
            return null;
        }
    }
}

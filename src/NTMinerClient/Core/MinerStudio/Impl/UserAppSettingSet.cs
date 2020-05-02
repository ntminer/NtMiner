using NTMiner.User;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerStudio.Impl {
    public class UserAppSettingSet : IUserAppSettingSet {
        private readonly Dictionary<string, UserAppSettingData> _dicByKey = new Dictionary<string, UserAppSettingData>(StringComparer.OrdinalIgnoreCase);
        private List<UserAppSettingData> _userAppSettings;

        public UserAppSettingSet() {
            VirtualRoot.AddCmdPath<SetUserAppSettingCommand>(action: message => {
                if (message.AppSetting == null) {
                    return;
                }
                UserAppSettingData oldValue;
                if (_dicByKey.TryGetValue(message.AppSetting.Key, out UserAppSettingData entity)) {
                    oldValue = new UserAppSettingData {
                        Id = entity.Id,
                        LoginName = entity.LoginName,
                        Key = entity.Key,
                        Value = entity.Value
                    };
                    entity.Value = message.AppSetting.Value;
                }
                else {
                    entity = UserAppSettingData.Create(message.AppSetting);
                    oldValue = null;
                    _dicByKey.Add(message.AppSetting.Key, entity);
                }
                RpcRoot.OfficialServer.UserAppSettingService.SetAppSettingAsync(entity, (response, exception) => {
                    if (!response.IsSuccess()) {
                        if (oldValue == null) {
                            _dicByKey.Remove(message.AppSetting.Key);
                        }
                        else {
                            entity.Value = oldValue.Value;
                        }
                        VirtualRoot.Out.ShowError(response.ReadMessage(exception), autoHideSeconds: 4);
                    }
                });
            }, location: this.GetType());
        }

        public void Init(List<UserAppSettingData> userAppSettings) {
            if (_userAppSettings != userAppSettings) {
                _userAppSettings = userAppSettings;
                if (userAppSettings != null && userAppSettings.Count != 0) {
                    foreach (var item in userAppSettings) {
                        _dicByKey[item.Key] = item;
                    }
                }
                VirtualRoot.RaiseEvent(new UserAppSettingSetInitedEvent());
            }
        }

        public bool TryGetAppSetting(string key, out IUserAppSetting appSetting) {
            bool r = _dicByKey.TryGetValue(key, out UserAppSettingData item);
            appSetting = item;
            return r;
        }

        public IEnumerable<IUserAppSetting> AsEnumerable() {
            return _dicByKey.Values;
        }
    }
}

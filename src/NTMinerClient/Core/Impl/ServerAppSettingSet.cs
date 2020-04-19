using NTMiner.AppSetting;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class ServerAppSettingSet : IAppSettingSet {
        private readonly Dictionary<string, AppSettingData> _dicByKey = new Dictionary<string, AppSettingData>(StringComparer.OrdinalIgnoreCase);
        public ServerAppSettingSet(List<AppSettingData> appSettings) {
            VirtualRoot.AddCmdPath<SetServerAppSettingCommand>(action: message => {
                if (message.AppSetting == null) {
                    return;
                }
                AppSettingData oldValue;
                if (_dicByKey.TryGetValue(message.AppSetting.Key, out AppSettingData entity)) {
                    oldValue = new AppSettingData {
                        Key = entity.Key,
                        Value = entity.Value
                    };
                    entity.Value = message.AppSetting.Value;
                }
                else {
                    entity = AppSettingData.Create(message.AppSetting);
                    oldValue = null;
                    _dicByKey.Add(message.AppSetting.Key, entity);
                }
                RpcRoot.OfficialServer.AppSettingService.SetAppSettingAsync(entity, (response, exception) => {
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

        public bool TryGetAppSetting(string key, out IAppSetting appSetting) {
            var result = _dicByKey.TryGetValue(key, out AppSettingData data);
            appSetting = data;
            return result;
        }

        public IEnumerable<IAppSetting> AsEnumerable() {
            return _dicByKey.Values;
        }
    }
}

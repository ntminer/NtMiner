using NTMiner.AppSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.MinerServer.Impl {
    public class ServerAppSettingSet : IAppSettingSet {
        private readonly Dictionary<string, AppSettingData> _dicByKey = new Dictionary<string, AppSettingData>(StringComparer.OrdinalIgnoreCase);
        public ServerAppSettingSet() {
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
                Server.AppSettingService.SetAppSettingAsync(entity, (response, exception) => {
                    if (!response.IsSuccess()) {
                        if (oldValue == null) {
                            _dicByKey.Remove(message.AppSetting.Key);
                        }
                        else {
                            entity.Value = oldValue.Value;
                        }
                        Write.UserFail(response.ReadMessage(exception));
                        VirtualRoot.RaiseEvent(new ServerAppSettingSetedEvent(message.Id, entity));
                    }
                });
                VirtualRoot.RaiseEvent(new ServerAppSettingSetedEvent(message.Id, entity));
            });
            VirtualRoot.AddCmdPath<SetServerAppSettingsCommand>(action: message => {
                if (message.AppSettings == null) {
                    return;
                }
                foreach (var item in message.AppSettings) {
                    AppSettingData oldValue;
                    if (_dicByKey.TryGetValue(item.Key, out AppSettingData entity)) {
                        oldValue = new AppSettingData {
                            Key = entity.Key,
                            Value = entity.Value
                        };
                        entity.Value = item.Value;
                    }
                    else {
                        entity = AppSettingData.Create(item);
                        oldValue = null;
                        _dicByKey.Add(item.Key, entity);
                    }
                    VirtualRoot.RaiseEvent(new ServerAppSettingSetedEvent(message.Id, entity));
                }
                Server.AppSettingService.SetAppSettingsAsync(message.AppSettings.Select(a => AppSettingData.Create(a)).ToList(), (response, exception) => {
                });
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
                    var list = Server.AppSettingService.GetAppSettings();
                    foreach (var item in list) {
                        _dicByKey.Add(item.Key, item);
                    }
                    _isInited = true;
                }
            }
        }

        public bool TryGetAppSetting(string key, out IAppSetting appSetting) {
            InitOnece();
            var result = _dicByKey.TryGetValue(key, out AppSettingData data);
            appSetting = data;
            return result;
        }

        public IEnumerable<IAppSetting> AsEnumerable() {
            InitOnece();
            return _dicByKey.Values;
        }
    }
}

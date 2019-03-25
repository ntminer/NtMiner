using NTMiner.MinerServer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.MinerServer.Impl {
    public class AppSettingSet : IAppSettingSet {
        private readonly Dictionary<string, AppSettingData> _dicByKey = new Dictionary<string, AppSettingData>();
        private readonly INTMinerRoot _root;
        public AppSettingSet(INTMinerRoot root) {
            _root = root;
            VirtualRoot.Window<ChangeAppSettingCommand>("处理设置AppSetting命令", LogEnum.DevConsole,
                action: message => {
                    if (message.AppSetting == null) {
                        return;
                    }
                    AppSettingData entity;
                    AppSettingData oldValue;
                    if (_dicByKey.TryGetValue(message.AppSetting.Key, out entity)) {
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
                            if (response != null) {
                                Write.UserLine(response.Description, System.ConsoleColor.Red);
                            }
                            VirtualRoot.Happened(new AppSettingChangedEvent(entity));
                        }
                    });
                    VirtualRoot.Happened(new AppSettingChangedEvent(entity));
                });
            VirtualRoot.Window<ChangeAppSettingsCommand>("处理批量设置AppSetting命令", LogEnum.DevConsole,
                action: message => {
                    if (message.AppSettings == null) {
                        return;
                    }
                    foreach (var item in message.AppSettings) {
                        AppSettingData entity;
                        AppSettingData oldValue;
                        if (_dicByKey.TryGetValue(item.Key, out entity)) {
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
                        VirtualRoot.Happened(new AppSettingChangedEvent(entity));
                    }
                    Server.AppSettingService.SetAppSettingsAsync(message.AppSettings.Select(a=>AppSettingData.Create(a)).ToList(), (response, exception) => {
                    });
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
            AppSettingData data;
            var result = _dicByKey.TryGetValue(key, out data);
            appSetting = data;
            return result;
        }

        public IEnumerator<IAppSetting> GetEnumerator() {
            InitOnece();
            foreach (var item in _dicByKey.Values) {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            foreach (var item in _dicByKey.Values) {
                yield return item;
            }
        }
    }
}

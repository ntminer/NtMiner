using LiteDB;
using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.AppSetting {
    public class LocalAppSettingSet : IAppSettingSet {
        private readonly Dictionary<string, AppSettingData> _dicByKey = new Dictionary<string, AppSettingData>(StringComparer.OrdinalIgnoreCase);
        private readonly string _dbFileFullName;

        public LocalAppSettingSet(string dbFileFullName) {
            _dbFileFullName = dbFileFullName;
            VirtualRoot.AddCmdPath<SetLocalAppSettingCommand>(action: message => {
                if (message.AppSetting == null) {
                    return;
                }
                if (_dicByKey.TryGetValue(message.AppSetting.Key, out AppSettingData entity)) {
                    entity.Value = message.AppSetting.Value;
                }
                else {
                    entity = AppSettingData.Create(message.AppSetting);
                    _dicByKey[message.AppSetting.Key] = entity;
                }
                using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                    var col = db.GetCollection<AppSettingData>();
                    col.Upsert(entity);
                }
                VirtualRoot.RaiseEvent(new LocalAppSettingChangedEvent(message.Id, entity));
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
                        var col = db.GetCollection<AppSettingData>();
                        foreach (var item in col.FindAll()) {
                            _dicByKey.Add(item.Key, item);
                        }
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

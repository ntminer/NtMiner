using LiteDB;
using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.AppSetting {
    public class LocalAppSettingSet : SetBase, IAppSettingSet {
        private readonly Dictionary<string, AppSettingData> _dicByKey = new Dictionary<string, AppSettingData>(StringComparer.OrdinalIgnoreCase);
        private readonly string _dbFileFullName;

        public LocalAppSettingSet(string dbFileFullName) {
            _dbFileFullName = dbFileFullName;
            VirtualRoot.BuildCmdPath<SetLocalAppSettingCommand>(location: this.GetType(), LogEnum.DevConsole, path: message => {
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
                VirtualRoot.RaiseEvent(new LocalAppSettingChangedEvent(message.MessageId, entity));
            });
        }

        protected override void Init() {
            using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                var col = db.GetCollection<AppSettingData>();
                foreach (var item in col.FindAll()) {
                    _dicByKey.Add(item.Key, item);
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

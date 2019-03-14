using LiteDB;
using NTMiner.MinerServer;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Data.Impl {
    public class AppSettingSet : IAppSettingSet {
        private Dictionary<string, AppSettingData> _dicByKey = new Dictionary<string, AppSettingData>();
        private readonly string _dbFileFullName;

        public AppSettingSet(string dbFileFullName) {
            _dbFileFullName = dbFileFullName;
            VirtualRoot.Accept<ChangeAppSettingCommand>(
                "处理设置AppSetting命令",
                LogEnum.Console,
                action: message => {
                    if (message.AppSetting == null) {
                        return;
                    }
                    AppSettingData entity;
                    if (_dicByKey.TryGetValue(message.AppSetting.Key, out entity)) {
                        entity.Value = message.AppSetting.Value;
                        using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                            var col = db.GetCollection<AppSettingData>();
                            col.Update(entity);
                        }
                    }
                    else {
                        entity = AppSettingData.Create(message.AppSetting);
                        _dicByKey.Add(message.AppSetting.Key, entity);
                        using (LiteDatabase db = new LiteDatabase(_dbFileFullName)) {
                            var col = db.GetCollection<AppSettingData>();
                            col.Insert(entity);
                        }
                    }
                    VirtualRoot.Happened(new AppSettingChangedEvent(entity));
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

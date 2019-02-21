using LiteDB;
using NTMiner.AppSetting;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Data.Impl {
    public class AppSettingSet : IAppSettingSet {
        private Dictionary<string, AppSettingData> _dicByKey = new Dictionary<string, AppSettingData>();
        private readonly string _dbFileFullName;

        public AppSettingSet(string dbFileFullName) {
            _dbFileFullName = dbFileFullName;
            VirtualRoot.Accept<SetAppSettingCommand>(
                Guid.Parse("21205872-1601-4097-B058-891386FB8125"),
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

        public IAppSetting GetAppSetting(string key) {
            InitOnece();
            AppSettingData data;
            _dicByKey.TryGetValue(key, out data);
            return data;
        }

        public List<IAppSetting> GetAppSettings() {
            InitOnece();
            return _dicByKey.Values.Cast<IAppSetting>().ToList();
        }
    }
}

using LiteDB;
using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Data.Impl {
    public class AppSettingSet : IAppSettingSet {
        private Dictionary<string, AppSettingData> _dicByKey = new Dictionary<string, AppSettingData>(StringComparer.OrdinalIgnoreCase);

        private readonly IHostRoot _root;

        public AppSettingSet(IHostRoot root) {
            _root = root;
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
                    using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                        var col = db.GetCollection<AppSettingData>();
                        foreach (var item in col.FindAll()) {
                            _dicByKey.Add(item.Key, item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public List<AppSettingData> GetAllAppSettings() {
            InitOnece();
            return _dicByKey.Values.ToList();
        }

        public AppSettingData GetAppSetting(string key) {
            InitOnece();
            AppSettingData data;
            _dicByKey.TryGetValue(key, out data);
            return data;
        }

        public List<AppSettingData> GetAppSettings(string[] keys) {
            InitOnece();
            List<AppSettingData> results = new List<AppSettingData>();
            AppSettingData item;
            foreach (var key in keys) {
                if (_dicByKey.TryGetValue(key, out item)) {
                    results.Add(item);
                }
            }
            return results;
        }

        public void SetAppSetting(AppSettingData appSettingData) {
            InitOnece();
            if (_dicByKey.ContainsKey(appSettingData.Key)) {
                _dicByKey[appSettingData.Key].Value = appSettingData.Value;
                using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                    var col = db.GetCollection<AppSettingData>();
                    col.Update(_dicByKey[appSettingData.Key]);
                }
            }
            else {
                _dicByKey.Add(appSettingData.Key, appSettingData);
                using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                    var col = db.GetCollection<AppSettingData>();
                    col.Insert(appSettingData);
                }
            }
        }
    }
}

using LiteDB;
using NTMiner.User;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class UserAppSettingSet : SetBase, IUserAppSettingSet {
        private readonly Dictionary<string, List<UserAppSettingData>> _dicByLoginName = new Dictionary<string, List<UserAppSettingData>>();

        public UserAppSettingSet() {
        }

        protected override void Init() {
            using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                var col = db.GetCollection<UserAppSettingData>();
                foreach (var item in col.FindAll()) {
                    if (!_dicByLoginName.TryGetValue(item.LoginName, out List<UserAppSettingData> list)) {
                        list = new List<UserAppSettingData>();
                        _dicByLoginName.Add(item.LoginName, list);
                    }
                    list.Add(item);
                }
            }
        }

        public List<UserAppSettingData> GetAppSettings(string loginName) {
            InitOnece();
            if (_dicByLoginName.TryGetValue(loginName, out List<UserAppSettingData> list)) {
                return list;
            }
            return new List<UserAppSettingData>();
        }

        public void SetAppSetting(UserAppSettingData appSetting) {
            InitOnece();
            if (appSetting == null || string.IsNullOrEmpty(appSetting.LoginName)) {
                return;
            }
            if (!_dicByLoginName.TryGetValue(appSetting.LoginName, out List<UserAppSettingData> list)) {
                list = new List<UserAppSettingData>();
                _dicByLoginName.Add(appSetting.LoginName, list);
            }
            var exist = list.FirstOrDefault(a => a.Key == appSetting.Key);
            if (exist != null) {
                exist.Value = appSetting.Value;
            }
            else {
                list.Add(appSetting);
            }
            using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                var col = db.GetCollection<UserAppSettingData>();
                col.Upsert(exist);
            }
        }
    }
}

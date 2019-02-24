using NTMiner.MinerServer;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.MinerServer.Impl {
    public class AppSettingSet : IAppSettingSet {
        private Dictionary<string, AppSettingData> _dicByKey = new Dictionary<string, AppSettingData>();
        private readonly INTMinerRoot _root;
        public AppSettingSet(INTMinerRoot root) {
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
                    var list = Server.AppSettingService.GetAppSettings();
                    foreach (var item in list) {
                        _dicByKey.Add(item.Key, item);
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

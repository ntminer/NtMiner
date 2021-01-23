using LiteDB;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class CalcConfigSet : ICalcConfigSet {
        private Dictionary<string, CalcConfigData> _dicByCode = new Dictionary<string, CalcConfigData>(StringComparer.OrdinalIgnoreCase);

        public CalcConfigSet() {
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
                    using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                        var col = db.GetCollection<CalcConfigData>();
                        foreach (var item in col.FindAll()) {
                            _dicByCode.Add(item.CoinCode, item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public List<CalcConfigData> GetAll() {
            InitOnece();
            return _dicByCode.Values.ToList();
        }

        public void SaveCalcConfigs(List<CalcConfigData> data) {
            InitOnece();
            if (data == null || data.Count == 0) {
                return;
            }
            lock (_locker) {
                var dic = new Dictionary<string, CalcConfigData>(StringComparer.OrdinalIgnoreCase);
                foreach (var item in data) {
                    if (dic.TryGetValue(item.CoinCode, out CalcConfigData entity)) {
                        entity.Update(item);
                    }
                    else {
                        dic.Add(item.CoinCode, item);
                    }
                }
                using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                    var col = db.GetCollection<CalcConfigData>();
                    col.Delete(Query.All());
                    col.Insert(dic.Values);
                }
                _dicByCode = dic;
            }
        }
    }
}

using LiteDB;
using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Data.Impl {
    public class CalcConfigSet : ICalcConfigSet {
        private readonly Dictionary<string, CalcConfigData> _dicByCode = new Dictionary<string, CalcConfigData>(StringComparer.OrdinalIgnoreCase);

        private readonly IHostRoot _root;
        public CalcConfigSet(IHostRoot root) {
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
                        var col = db.GetCollection<CalcConfigData>();
                        foreach (var item in col.FindAll()) {
                            _dicByCode.Add(item.CoinCode, item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public List<CalcConfigData> GetCalcConfigs() {
            InitOnece();
            return _dicByCode.Values.ToList();
        }

        public void SaveCalcConfigs(List<CalcConfigData> data) {
            InitOnece();
            if (data == null || data.Count == 0) {
                return;
            }
            lock (_locker) {
                string[] toRemoves = _dicByCode.Where(a => data.All(b => string.Equals(a.Key, b.CoinCode, StringComparison.OrdinalIgnoreCase))).Select(a => a.Key).ToArray();
                foreach (var key in toRemoves) {
                    _dicByCode.Remove(key);
                }
                foreach (var item in data) {
                    if (_dicByCode.ContainsKey(item.CoinCode)) {
                        _dicByCode[item.CoinCode].Update(item);
                    }
                    else {
                        item.CreatedOn = DateTime.Now;
                        _dicByCode.Add(item.CoinCode, item);
                    }
                }
                using (LiteDatabase db = HostRoot.CreateLocalDb()) {
                    var col = db.GetCollection<CalcConfigData>();
                    col.Delete(Query.In(nameof(CalcConfigData.CoinCode), toRemoves.Select(a => new BsonValue(a)).ToArray()));
                    col.Upsert(_dicByCode.Values);
                }
            }
        }
    }
}

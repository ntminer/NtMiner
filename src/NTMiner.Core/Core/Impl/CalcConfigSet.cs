using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class CalcConfigSet : ICalcConfigSet {
        private Dictionary<string, CalcConfigData> _dicByCoinCode = new Dictionary<string, CalcConfigData>(StringComparer.OrdinalIgnoreCase);
        private readonly INTMinerRoot _root;

        public CalcConfigSet(INTMinerRoot root) {
            _root = root;
            BootLog.Log(this.GetType().FullName + "接入总线");
        }

        private DateTime _initedOn = DateTime.MinValue;
        private object _locker = new object();

        private void Init() {
            DateTime now = DateTime.Now;
            if (_initedOn.AddMinutes(10) < now) {
                lock (_locker) {
                    if (_initedOn.AddMinutes(10) < now) {
                        var list = _root.CoinSet.OrderBy(a => a.SortNumber).Select(a => new CalcConfigData {
                            CoinCode = a.Code,
                            CreatedOn = DateTime.Now,
                            IncomePerDay = 0,
                            ModifiedOn = DateTime.Now,
                            Speed = 0,
                            SpeedUnit = "H/s"
                        }).ToList();
                        var response = Server.ControlCenterService.GetCalcConfigs();
                        if (response != null) {
                            foreach (var item in list) {
                                var exist = response.Data.FirstOrDefault(a => string.Equals(a.CoinCode, item.CoinCode, StringComparison.OrdinalIgnoreCase));
                                if (exist != null) {
                                    item.Update(exist);
                                }
                            }
                        }
                        _dicByCoinCode = list.ToDictionary(a => a.CoinCode, a => a, StringComparer.OrdinalIgnoreCase);
                        _initedOn = now;
                    }
                }
            }
        }

        public bool TryGetCalcConfig(ICoin coin, out ICalcConfig calcConfig) {
            Init();
            if (!_dicByCoinCode.ContainsKey(coin.Code)) {
                calcConfig = null;
                return false;
            }
            calcConfig = _dicByCoinCode[coin.Code];
            return true;
        }

        public double GetIncomePerHashPerDay(ICoin coin) {
            Init();
            if (!_dicByCoinCode.ContainsKey(coin.Code)) {
                return 0;
            }
            CalcConfigData item = _dicByCoinCode[coin.Code];
            if (item.Speed == 0) {
                return 0;
            }
            return item.IncomePerDay / item.Speed.ToLong(item.SpeedUnit);
        }

        public void SaveCalcConfigs(List<CalcConfigData> data) {
            lock (_locker) {
                _dicByCoinCode = data.ToDictionary(a => a.CoinCode, a => a, StringComparer.OrdinalIgnoreCase);
                Server.ControlCenterService.SaveCalcConfigs(data);
            }
        }

        public IEnumerator<CalcConfigData> GetEnumerator() {
            Init();
            return _dicByCoinCode.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            Init();
            return _dicByCoinCode.GetEnumerator();
        }
    }
}

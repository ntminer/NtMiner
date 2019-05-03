using NTMiner.MinerServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class CalcConfigSet : ICalcConfigSet {
        private Dictionary<string, CalcConfigData> _dicByCoinCode = new Dictionary<string, CalcConfigData>(StringComparer.OrdinalIgnoreCase);
        private readonly INTMinerRoot _root;

        public CalcConfigSet(INTMinerRoot root) {
            _root = root;
        }

        private DateTime _initedOn = DateTime.MinValue;

        private void Init() {
            if (_initedOn == DateTime.MinValue) {
                // 第一次访问时从磁盘初始化收益计算器设置，如果磁盘上有的话
                _initedOn = DateTime.MinValue.AddMinutes(10);
                if (File.Exists(SpecialPath.CalcJsonFileFullName)) {
                    try {
                        List<CalcConfigData> data = VirtualRoot.JsonSerializer.Deserialize<List<CalcConfigData>>(File.ReadAllText(SpecialPath.CalcJsonFileFullName));
                        Init(data);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                }
            }
            DateTime now = DateTime.Now;
            if (_initedOn.AddMinutes(10) < now) {
                _initedOn = now;
                OfficialServer.GetCalcConfigsAsync(data => {
                    Init(data);
                    VirtualRoot.Happened(new CalcConfigSetInitedEvent());
                    string json = VirtualRoot.JsonSerializer.Serialize(data);
                    // 缓存在磁盘
                    File.WriteAllText(SpecialPath.CalcJsonFileFullName, json);
                });
            }
        }

        private void Init(List<CalcConfigData> data) {
            var list = _root.CoinSet.OrderBy(a => a.SortNumber).Select(a => new CalcConfigData {
                CoinCode = a.Code,
                CreatedOn = DateTime.Now,
                IncomePerDay = 0,
                ModifiedOn = DateTime.Now,
                Speed = 0,
                SpeedUnit = "H/s"
            }).ToList();
            foreach (var item in data) {
                var exist = list.FirstOrDefault(a => string.Equals(a.CoinCode, item.CoinCode, StringComparison.OrdinalIgnoreCase));
                if (exist != null) {
                    exist.Update(item);
                }
                else {
                    list.Add(item);
                }
            }
            _dicByCoinCode = list.ToDictionary(a => a.CoinCode, a => a, StringComparer.OrdinalIgnoreCase);
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

        public IncomePerDay GetIncomePerHashPerDay(string coinCode) {
            Init();
            if (!_dicByCoinCode.ContainsKey(coinCode)) {
                return IncomePerDay.Zero;
            }
            CalcConfigData item = _dicByCoinCode[coinCode];
            if (item.Speed == 0) {
                return IncomePerDay.Zero;
            }
            double speed = item.Speed.FromUnitSpeed(item.SpeedUnit);
            return new IncomePerDay(item.IncomePerDay / speed, item.IncomeUsdPerDay / speed, item.IncomeCnyPerDay / speed);
        }

        public void SaveCalcConfigs(List<CalcConfigData> data) {
            _dicByCoinCode = data.ToDictionary(a => a.CoinCode, a => a, StringComparer.OrdinalIgnoreCase);
            OfficialServer.SaveCalcConfigsAsync(data, null);
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

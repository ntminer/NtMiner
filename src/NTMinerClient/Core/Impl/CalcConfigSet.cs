using NTMiner.Core.MinerServer;
using NTMiner.Mine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class CalcConfigSet : ICalcConfigSet {
        private Dictionary<string, CalcConfigData> _dicByCoinCode = new Dictionary<string, CalcConfigData>(StringComparer.OrdinalIgnoreCase);
        private readonly INTMinerContext _ntminerContext;

        public CalcConfigSet(INTMinerContext ntminerContext) {
            _ntminerContext = ntminerContext;
            if (ClientAppType.IsMinerClient) {
                VirtualRoot.BuildEventPath<MineStartedEvent>("开始挖矿时加载收益计算器数据", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        Init();
                    });
            }
        }

        private List<string> _coinCodes = new List<string>();
        private DateTime _initedOn = DateTime.MinValue;
        public void Init(bool forceRefresh = false) {
            List<string> coinCodes = new List<string>();
            var mineContext = NTMinerContext.Instance.CurrentMineContext;
            if (!forceRefresh && mineContext != null) {
                if (mineContext.MainCoin != null) {
                    coinCodes.Add(mineContext.MainCoin.Code);
                }
                if (mineContext is IDualMineContext dualMineContext) {
                    if (dualMineContext.DualCoin != null) {
                        coinCodes.Add(dualMineContext.DualCoin.Code);
                    }
                }
            }
            DateTime now = DateTime.Now;
            // 如果未显示主界面则收益计算器也不用更新了
            if ((_initedOn == DateTime.MinValue || NTMinerContext.IsUiVisible || ClientAppType.IsMinerStudio)
                && (forceRefresh || (_coinCodes.Count != 0 && coinCodes.Any(a => !_coinCodes.Contains(a))) || _initedOn.AddMinutes(10) < now)) {
                _initedOn = now;
                _coinCodes = coinCodes;
                if (forceRefresh) {
                    // 传空表示获取全部
                    _coinCodes.Clear();
                }
                RpcRoot.OfficialServer.CalcConfigBinaryService.QueryCalcConfigsAsync(coinCodes, data => {
                    if (data != null && data.Count != 0) {
                        Init(data);
                        VirtualRoot.RaiseEvent(new CalcConfigSetInitedEvent());
                    }
                });
            }
        }

        private void Init(List<CalcConfigData> data) {
            if (data == null || data.Count == 0) {
                return;
            }
            var list = _ntminerContext.ServerContext.CoinSet.AsEnumerable().OrderBy(a => a.Code).Select(a => new CalcConfigData {
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
            if (speed == 0) {
                return IncomePerDay.Zero;
            }
            return new IncomePerDay(item.IncomePerDay / speed, item.IncomeUsdPerDay / speed, item.IncomeCnyPerDay / speed, item.ModifiedOn);
        }

        public void SaveCalcConfigs(List<CalcConfigData> data) {
            _dicByCoinCode = data.ToDictionary(a => a.CoinCode, a => a, StringComparer.OrdinalIgnoreCase);
            RpcRoot.OfficialServer.CalcConfigService.SaveCalcConfigsAsync(data, (response, e) => {
                if (!response.IsSuccess()) {
                    VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                }
            });
        }

        public IEnumerable<ICalcConfig> AsEnumerable() {
            Init();
            return _dicByCoinCode.Values;
        }
    }
}

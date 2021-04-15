using NTMiner.Core.MinerServer;
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
                VirtualRoot.BuildOnecePath<HasBoot20SecondEvent>("初始化收益计算器", LogEnum.DevConsole, pathId: PathId.Empty, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        Init();
                    });
            }
        }

        private DateTime _initedOn = DateTime.MinValue;

        public void Init(bool forceRefresh = false) {
            DateTime now = DateTime.Now;
            // 如果未显示主界面则收益计算器也不用更新了
            if ((_initedOn == DateTime.MinValue || NTMinerContext.IsUiVisible || ClientAppType.IsMinerStudio) && (forceRefresh || _initedOn.AddMinutes(10) < now)) {
                _initedOn = now;
                RpcRoot.OfficialServer.CalcConfigService.GetCalcConfigsAsync(data => {
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

using LiteDB;
using System;
using System.Text;

namespace NTMiner.Core.MinerServer {
    public class CalcConfigData : ICalcConfig, IGetSignData {
        public static CalcConfigData Create(ICalcConfig data) {
            return new CalcConfigData {
                CoinCode = data.CoinCode,
                Speed = data.Speed,
                SpeedUnit = data.SpeedUnit,
                NetSpeed = data.NetSpeed,
                BaseNetSpeed = data.BaseNetSpeed,
                BaseNetSpeedUnit = data.BaseNetSpeedUnit,
                DayWave = data.DayWave,
                NetSpeedUnit = data.NetSpeedUnit,
                IncomePerDay = data.IncomePerDay,
                IncomeUsdPerDay = data.IncomeUsdPerDay,
                IncomeCnyPerDay = data.IncomeCnyPerDay,
                CreatedOn = data.CreatedOn,
                ModifiedOn = data.ModifiedOn
            };
        }

        public CalcConfigData() { }

        [BsonId]
        public string CoinCode { get; set; }

        public double Speed { get; set; }

        public string SpeedUnit { get; set; }

        public double NetSpeed { get; set; }

        public string NetSpeedUnit { get; set; }

        public double DayWave { get; set; }

        public double IncomePerDay { get; set; }

        public double IncomeUsdPerDay { get; set; }

        public double IncomeCnyPerDay { get; set; }
        public double BaseNetSpeed { get; set; }
        public string BaseNetSpeedUnit { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}

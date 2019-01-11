using LiteDB;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class CalcConfigData : ICalcConfig {
        public CalcConfigData() { }

        public CalcConfigData(ICalcConfig data) {
            this.CoinCode = data.CoinCode;
            this.Speed = data.Speed;
            this.SpeedUnit = data.SpeedUnit;
            this.IncomePerDay = data.IncomePerDay;
            this.CreatedOn = data.CreatedOn;
            this.ModifiedOn = data.ModifiedOn;
        }

        public void Update(ICalcConfig data) {
            this.Speed = data.Speed;
            this.SpeedUnit = data.SpeedUnit;
            this.IncomePerDay = data.IncomePerDay;
            this.CreatedOn = data.CreatedOn;
            this.ModifiedOn = data.ModifiedOn;
        }

        [BsonId]
        [DataMember]
        public string CoinCode { get; set; }

        [DataMember]
        public double Speed { get; set; }

        [DataMember]
        public string SpeedUnit { get; set; }

        [DataMember]
        public double IncomePerDay { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public DateTime ModifiedOn { get; set; }

        public string GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(CoinCode)).Append(CoinCode)
                .Append(nameof(Speed)).Append(Speed)
                .Append(nameof(SpeedUnit)).Append(SpeedUnit)
                .Append(nameof(IncomePerDay)).Append(IncomePerDay)
                .Append(nameof(CreatedOn)).Append(CreatedOn.ToUlong())
                .Append(nameof(ModifiedOn)).Append(ModifiedOn.ToUlong());
            return sb.ToString();
        }
    }
}

using System;
using System.Text;

namespace NTMiner.Core {
    public class PoolData : IPool, IDbEntity<Guid>, IGetSignData {
        public PoolData() {
        }

        public PoolData(IPool data) {
            this.DataLevel = data.DataLevel;
            this.Id = data.CoinId;
            this.BrandId = data.BrandId;
            this.CoinId = data.CoinId;
            this.Name = data.Name;
            this.Server = data.Server;
            this.Url = data.Url;
            this.Website = data.Website;
            this.SortNumber = data.SortNumber;
            this.IsUserMode = data.IsUserMode;
            this.UserName = data.UserName;
            this.Password = data.Password;
            this.Notice = data.Notice;
            this.TutorialUrl = data.TutorialUrl;
            this.NoPool1 = data.NoPool1;
            this.NotPool1 = data.NotPool1;
            this.MinerNamePrefix = data.MinerNamePrefix;
            this.MinerNamePostfix = data.MinerNamePostfix;
        }

        public Guid GetId() {
            return this.Id;
        }

        [LiteDB.BsonIgnore]
        public DataLevel DataLevel { get; set; }

        public void SetDataLevel(DataLevel dataLevel) {
            this.DataLevel = dataLevel;
        }

        public Guid Id { get; set; }

        public Guid BrandId { get; set; }

        public Guid CoinId { get; set; }

        public string Name { get; set; }

        public string Server { get; set; }

        public string Url { get; set; }

        public string Website { get; set; }

        public int SortNumber { get; set; }

        public bool IsUserMode { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Notice { get; set; }

        public string TutorialUrl { get; set; }

        public bool NoPool1 { get; set; }

        public bool NotPool1 { get; set; }

        public string MinerNamePrefix { get; set; }

        public string MinerNamePostfix { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}

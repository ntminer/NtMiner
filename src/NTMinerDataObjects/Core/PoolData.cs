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

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Id)).Append(Id)
                .Append(nameof(BrandId)).Append(BrandId)
                .Append(nameof(DataLevel)).Append(DataLevel)
                .Append(nameof(CoinId)).Append(CoinId)
                .Append(nameof(Name)).Append(Name)
                .Append(nameof(Server)).Append(Server)
                .Append(nameof(Url)).Append(Url)
                .Append(nameof(Website)).Append(Website)
                .Append(nameof(SortNumber)).Append(SortNumber)
                .Append(nameof(IsUserMode)).Append(IsUserMode)
                .Append(nameof(UserName)).Append(UserName)
                .Append(nameof(Password)).Append(Password)
                .Append(nameof(Notice)).Append(Notice);
            return sb;
        }
    }
}

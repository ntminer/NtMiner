using System;
using System.Text;

namespace NTMiner.Core {
    public class PoolData : IPool, IDbEntity<Guid>, IGetSignData {
        public PoolData() {
        }

        public PoolData(IPool data) {
            this.DataLevel = data.DataLevel;
            this.Id = data.CoinId;
            this.CoinId = data.CoinId;
            this.Name = data.Name;
            this.Server = data.Server;
            this.Url = data.Url;
            this.SortNumber = data.SortNumber;
            this.PublishState = data.PublishState;
            this.Description = data.Description;
            this.IsUserMode = data.IsUserMode;
            this.UserName = data.UserName;
            this.Password = data.Password;
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

        public Guid CoinId { get; set; }

        public string Name { get; set; }

        public string Server { get; set; }

        public string Url { get; set; }

        public int SortNumber { get; set; }

        public PublishStatus PublishState { get; set; }

        public string Description { get; set; }

        public bool IsUserMode { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Id)).Append(Id)
                .Append(nameof(DataLevel)).Append(DataLevel)
                .Append(nameof(CoinId)).Append(CoinId)
                .Append(nameof(Name)).Append(Name)
                .Append(nameof(Server)).Append(Server)
                .Append(nameof(Url)).Append(Url)
                .Append(nameof(SortNumber)).Append(SortNumber)
                .Append(nameof(PublishState)).Append(PublishState)
                .Append(nameof(Description)).Append(Description)
                .Append(nameof(IsUserMode)).Append(IsUserMode)
                .Append(nameof(UserName)).Append(UserName)
                .Append(nameof(Password)).Append(Password);
            return sb;
        }
    }
}

using System;
using System.Text;

namespace NTMiner.Core {
    public class WalletData : IWallet, IDbEntity<Guid>, IGetSignData {
        public WalletData() {
        }

        public WalletData(IWallet data) {
            this.Id = data.CoinId;
            this.CoinId = data.CoinId;
            this.Name = data.Name;
            this.Address = data.Address;
            this.SortNumber = data.SortNumber;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public Guid CoinId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public int SortNumber { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Id)).Append(Id)
                .Append(nameof(CoinId)).Append(CoinId)
                .Append(nameof(Name)).Append(Name)
                .Append(nameof(Address)).Append(Address)
                .Append(nameof(SortNumber)).Append(SortNumber);
            return sb;
        }
    }
}

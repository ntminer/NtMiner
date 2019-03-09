using System;
using System.Text;

namespace NTMiner.Core {
    public class WalletData : IWallet, IDbEntity<Guid> {
        public WalletData() {
            this.CreatedOn = DateTime.Now;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public Guid CoinId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public int SortNumber { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Id)).Append(Id)
                .Append(nameof(CoinId)).Append(CoinId)
                .Append(nameof(Name)).Append(Name)
                .Append(nameof(Address)).Append(Address)
                .Append(nameof(SortNumber)).Append(SortNumber)
                .Append(nameof(CreatedOn)).Append(CreatedOn.ToUlong())
                .Append(nameof(ModifiedOn)).Append(ModifiedOn.ToUlong());
            return sb.ToString();
        }
    }
}

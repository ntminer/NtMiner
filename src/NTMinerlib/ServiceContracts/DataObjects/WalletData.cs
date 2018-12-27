using System;
using System.Runtime.Serialization;
using System.Text;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class WalletData : IWallet, IDbEntity<Guid> {
        public WalletData() {
            this.CreatedOn = DateTime.Now;
        }

        public Guid GetId() {
            return this.Id;
        }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Guid CoinId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public int SortNumber { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public DateTime ModifiedOn { get; set; }

        public string GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Id)).Append(Id)
                .Append(nameof(CoinId)).Append(CoinId)
                .Append(nameof(Name)).Append(Name)
                .Append(nameof(Address)).Append(Address)
                .Append(nameof(SortNumber)).Append(SortNumber)
                .Append(nameof(CreatedOn)).Append(CreatedOn)
                .Append(nameof(ModifiedOn)).Append(ModifiedOn);
            return sb.ToString();
        }
    }
}

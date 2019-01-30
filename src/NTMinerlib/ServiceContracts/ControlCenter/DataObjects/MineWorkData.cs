using System;
using System.Runtime.Serialization;
using System.Text;

namespace NTMiner.ServiceContracts.ControlCenter.DataObjects {
    [DataContract]
    public class MineWorkData : IMineWork, IDbEntity<Guid>, ITimestampEntity<Guid> {
        public MineWorkData() {
            this.CreatedOn = DateTime.Now;
        }

        public Guid GetId() {
            return this.Id;
        }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public DateTime ModifiedOn { get; set; }

        public string GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Id)).Append(Id)
                .Append(nameof(Name)).Append(Name)
                .Append(nameof(Description)).Append(Description)
                .Append(nameof(CreatedOn)).Append(CreatedOn.ToUlong())
                .Append(nameof(ModifiedOn)).Append(ModifiedOn.ToUlong());
            return sb.ToString();
        }
    }
}

using System;
using System.Runtime.Serialization;
using System.Text;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class NTMinerFileData : INTMinerFile, IDbEntity<Guid> {
        public NTMinerFileData() {

        }

        public Guid GetId() {
            return this.Id;
        }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public string VersionTag { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public DateTime PublishOn { get; set; }

        public string GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Id)).Append(Id)
                .Append(nameof(FileName)).Append(FileName)
                .Append(nameof(Version)).Append(Version)
                .Append(nameof(VersionTag)).Append(VersionTag)
                .Append(nameof(CreatedOn)).Append(CreatedOn.ToUlong())
                .Append(nameof(PublishOn)).Append(PublishOn.ToUlong());
            return sb.ToString();
        }
    }
}

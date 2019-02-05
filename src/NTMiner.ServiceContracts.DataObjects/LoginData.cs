using System;
using System.Runtime.Serialization;

namespace NTMiner {
    [DataContract]
    public class LoginData : RequestBase {
        [DataMember]
        public Guid WorkId { get; set; }

        [DataMember]
        public Guid ClientId { get; set; }

        [DataMember]
        public string MinerName { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public string GpuInfo { get; set; }
    }
}

using System;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class RequestBase {
        public RequestBase() {
            this.MessageId = Guid.NewGuid();
        }

        [DataMember]
        public Guid MessageId { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }
    }
}

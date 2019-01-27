using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class LoadClientResponse : ResponseBase {
        [DataMember]
        public ClientData Data { get; set; }
    }
}

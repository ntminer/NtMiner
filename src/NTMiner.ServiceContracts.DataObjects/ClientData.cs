using System;
using System.Runtime.Serialization;

namespace NTMiner {
    [DataContract]
    public class ClientData : IClientData, IDbEntity<Guid>, ITimestampEntity<Guid> {
        public ClientData() {
        }

        public Guid GetId() {
            return this.Id;
        }

        private string _mainCoinCode;
        private string _dualCoinCode;
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Guid WorkId { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public bool IsMining { get; set; }

        [DataMember]
        public string MinerName { get; set; }

        [DataMember]
        public string MinerIp { get; set; }

        [DataMember]
        public string MainCoinCode {
            get => _mainCoinCode ?? string.Empty;
            set => _mainCoinCode = value;
        }

        [DataMember]
        public string MainCoinPool { get; set; }

        [DataMember]
        public string MainCoinWallet { get; set; }

        [DataMember]
        public string Kernel { get; set; }

        [DataMember]
        public bool IsDualCoinEnabled { get; set; }

        [DataMember]
        public string DualCoinCode {
            get => _dualCoinCode ?? string.Empty;
            set => _dualCoinCode = value;
        }

        [DataMember]
        public string DualCoinPool { get; set; }

        [DataMember]
        public string DualCoinWallet { get; set; }

        [DataMember]
        public string GpuInfo { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public DateTime ModifiedOn { get; set; }

        [DataMember]
        public Guid GroupId { get; set; }
    }
}

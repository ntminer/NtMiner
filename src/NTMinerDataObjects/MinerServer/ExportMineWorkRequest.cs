using System;
using System.Text;

namespace NTMiner.MinerServer {
    public class ExportMineWorkRequest : RequestBase, ISignatureRequest {
        public ExportMineWorkRequest() { }
        public Guid MineWorkId { get; set; }
        public string LocalJson { get; set; }
        public string ServerJson { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(MineWorkId)).Append(MineWorkId)
                .Append(nameof(LocalJson)).Append(LocalJson)
                .Append(nameof(ServerJson)).Append(ServerJson)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong());
            return sb;
        }
    }
}

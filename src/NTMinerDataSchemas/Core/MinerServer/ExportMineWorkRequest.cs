using System;
using System.Text;

namespace NTMiner.Core.MinerServer {
    public class ExportMineWorkRequest : IRequest, ISignableData {
        public ExportMineWorkRequest() { }
        public Guid MineWorkId { get; set; }
        public string LocalJson { get; set; }
        public string ServerJson { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}

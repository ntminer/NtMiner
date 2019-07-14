using System;
using System.Text;

namespace NTMiner.MinerServer {
    public class ExportMineWorkRequest : RequestBase, IGetSignData {
        public ExportMineWorkRequest() { }
        public Guid MineWorkId { get; set; }
        public string LocalJson { get; set; }
        public string ServerJson { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}

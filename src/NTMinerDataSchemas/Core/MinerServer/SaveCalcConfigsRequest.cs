using System.Collections.Generic;
using System.Text;

namespace NTMiner.Core.MinerServer {
    public class SaveCalcConfigsRequest : RequestBase, IGetSignData {
        public SaveCalcConfigsRequest() {
            this.Data = new List<CalcConfigData>();
        }
        [ManualSign]
        public List<CalcConfigData> Data { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = this.BuildSign();
            foreach (var item in Data) {
                sb.Append(item.GetSignData().ToString());
            }
            return sb;
        }
    }
}

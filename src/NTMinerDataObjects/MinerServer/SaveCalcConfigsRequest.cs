using System.Collections.Generic;
using System.Text;

namespace NTMiner.MinerServer {
    public class SaveCalcConfigsRequest : RequestBase, IGetSignData {
        public SaveCalcConfigsRequest() { }
        public List<CalcConfigData> Data { get; set; }

        private static StringBuilder GetSignData(StringBuilder sb, List<CalcConfigData> list) {
            foreach (var item in list) {
                sb.Append(item.GetSignData());
            }
            return sb;
        }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetSignData(sb, this.Data));
            return sb;
        }
    }
}

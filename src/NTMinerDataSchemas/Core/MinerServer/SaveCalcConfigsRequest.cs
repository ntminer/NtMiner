using System.Collections.Generic;
using System.Text;

namespace NTMiner.Core.MinerServer {
    public class SaveCalcConfigsRequest : IRequest, ISignableData {
        public SaveCalcConfigsRequest() {
            this.Data = new List<CalcConfigData>();
        }
        [ManualSign]
        public List<CalcConfigData> Data { get; set; }

        public StringBuilder GetSignData() {
            return this.GetActionIdSign("66BB71B0-C0C9-4371-8378-D30245D6BA68");
        }
    }
}

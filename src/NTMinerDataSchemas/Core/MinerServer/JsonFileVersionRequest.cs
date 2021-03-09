using System;

namespace NTMiner.Core.MinerServer {
    public class JsonFileVersionRequest : IRequest {
        public JsonFileVersionRequest() {
            this.MACAddress = new string[0];
        }
        public Guid ClientId { get; set; }
        public string Key { get; set; }
        // 服务端基于MACAddress判断当前矿机是否标识重复
        public string[] MACAddress { get; set; }
    }
}

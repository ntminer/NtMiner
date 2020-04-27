using System;

namespace NTMiner.ServerNode {
    public class GetWsServerNodeAddressRequest {
        public GetWsServerNodeAddressRequest() { }

        /// <summary>
        /// 用于分片
        /// </summary>
        public Guid ClientId { get; set; }
        /// <summary>
        /// 用户用户存在性验证
        /// </summary>
        public string UserId { get; set; }
    }
}

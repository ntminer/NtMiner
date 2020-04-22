using System;

namespace NTMiner.Core.MinerServer {
    public interface IMinerSign {
        string Id { get; }
        Guid ClientId { get; }
        /// <summary>
        /// 服务端填充的，一定是LoginName
        /// </summary>
        string LoginName { get; }
        /// <summary>
        /// 来自客户端：LoginName、Email或Mobile
        /// </summary>
        string OuterUserId { get; }
        string AESPassword { get; }
        /// <summary>
        /// AESPassword生成时的时间
        /// </summary>
        DateTime AESPasswordOn { get; }
    }
}

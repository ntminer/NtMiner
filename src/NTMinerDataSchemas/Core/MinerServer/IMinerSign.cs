using System;

namespace NTMiner.Core.MinerServer {
    public interface IMinerSign {
        string Id { get; }
        Guid ClientId { get; }
        /// <summary>
        /// LoginName、Email或Mobile
        /// </summary>
        string OuterUserId { get; }
        string AESPassword { get; }
        /// <summary>
        /// AESPassword生成时的时间
        /// </summary>
        DateTime AESPasswordOn { get; }
    }
}

using NTMiner.MinerClient;
using System;

namespace NTMiner.MinerServer {
    public interface IClientData : ISpeedData, IEntity<string> {
        Guid WorkId { get; }

        /// <summary>
        /// 矿机名
        /// </summary>
        string ClientName { get; }

        Guid GroupId { get; }

        string WindowsLoginName { get; }

        string WindowsPassword { get; }
        DateTime CreatedOn { get; }
    }
}

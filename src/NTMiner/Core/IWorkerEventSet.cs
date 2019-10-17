using NTMiner.MinerClient;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IWorkerEventSet {
        /// <summary>
        /// 简化，不分页，基于最近1000条
        /// </summary>
        /// <param name="typeId">Guid.Empty表示忽略typeId</param>
        /// <param name="keyword">null or empty表示全部</param>
        IEnumerable<IWorkerEvent> GetEvents(Guid typeId, string keyword);
    }
}

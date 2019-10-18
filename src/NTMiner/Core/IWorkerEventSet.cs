using NTMiner.MinerClient;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IWorkerEventSet {
        int LastWorkerEventId { get; }

        /// <summary>
        /// 简化，不分页，基于最近1000条
        /// </summary>
        /// <param name="typeId">WorkerEventChannel.Undefined表示忽略</param>
        /// <param name="keyword">null or empty表示全部</param>
        IEnumerable<IWorkerEvent> GetEvents(WorkerEventChannel channel, string keyword);
    }
}

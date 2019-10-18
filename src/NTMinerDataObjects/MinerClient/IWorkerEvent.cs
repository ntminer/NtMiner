using System;

namespace NTMiner.MinerClient {
    public interface IWorkerEvent : IEntity<int> {
        // Id will be auto-incremented by litedb
        int Id { get; }
        /// <summary>
        /// 频道是平的，主题是分层的。开源矿工的挖矿事件没有主题需求。
        /// </summary>
        WorkerEventChannel Channel { get; }
        string Content { get; }
        DateTime EventOn { get; }
    }
}

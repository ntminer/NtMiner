namespace NTMiner.Hub {
    using System;

    public interface IEvent : IMessage {
        /// <summary>
        /// 事件的目的路径，该事件将会被泵到该路径去。
        /// </summary>
        PathId TargetPathId { get; }
        /// <summary>
        /// 事件诞生时的时间戳
        /// </summary>
        DateTime BornOn { get; }
    }
}
